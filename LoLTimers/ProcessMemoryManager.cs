/*  LoL Jungle Timers - automatic timing of neutral monster camps in League of Legends.
    Copyright (C) 2014  Matthew Whittington

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.

    If you need to contact me please feel free to email at gbbwhittington@gmail.com.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;

namespace LoLTimers
{
    class ProcessMemoryManager
    {
        // Member variables
        private Process process;

        /// <summary>
        /// Stores the base pointer to each jungle camp class.
        /// </summary>
        Dictionary<JungleCamps, IntPtr> jungleOffsets;

        private IntPtr baseJunglePointer; // pointer for jungle camp related operations
        private IntPtr baseGameTimePointer; // pointer for game time related operations

        private int[] gameTimeOffsets = { 0x4 }; // ofsets to follow to access game timer

        // Memory reading and writing functions
        [Flags]
        private enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VMOperation = 0x00000008,
            VMRead = 0x00000010,
            VMWrite = 0x00000020,
            DupHandle = 0x00000040,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            Synchronize = 0x00100000
        }

        // Kernel32 functions
        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, uint nSize, out int lpNumberOfBytesWritten);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, [Out] byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hProcess);

        [DllImport("kernel32.dll")]
        private static extern uint GetLastError();

        // SigScan functions
        [DllImport("SigScan.dll", EntryPoint = "InitializeSigScan", CallingConvention = CallingConvention.Cdecl)]
        public static extern void InitializeSigScan(uint iPID, [MarshalAs(UnmanagedType.LPStr)] string Module);

        [DllImport("SigScan.dll", EntryPoint = "SigScan", CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 SigScan([MarshalAs(UnmanagedType.LPStr)] string Pattern, int Offset);

        [DllImport("SigScan.dll", EntryPoint = "FinalizeSigScan", CallingConvention = CallingConvention.Cdecl)]
        public static extern void FinalizeSigScan();

        // Class functions
        /// <summary>
        /// Constructor which takes a process to perform memory operations on.
        /// </summary>
        /// <param name="process">The process object to perform all operations on.</param>
        public ProcessMemoryManager(Process process)
        {
            if (process == null)
                throw new Exception("Bad process handle passed.");
            this.process = process;

            // find the pointers we need
            baseJunglePointer = GetBaseJunglePointer();
            baseGameTimePointer = GetBaseGameTimePointer();
        }

        /// <summary>
        /// Initialize the structure that holds the jungle pointers with the required values.
        /// This will block until the game timer has started.
        /// This function must be called before using the GetJungleCampStatus function.
        /// </summary>
        public void InitializeJungleOffsets()
        {
            while (GetCurrentGameTime() < 2)
            {
                // we can't initialize the jungle offsets until
                // the classes for the camps have been created

                // the timer being at 1 or higher means that the game
                // has most certainly loaded
                Thread.Sleep(500);
            }
            int offsetToAliveByte = GetOffsetToAliveStatus().ToInt32();
            // setup our jungle offsets
            jungleOffsets = new Dictionary<JungleCamps, IntPtr>();

            // TODO: optimize this code
            // these camps are all generally around the same area in memory
            // so once we find one, we should be able to narrow down the search
            // range for the rest

            // Blue side
            jungleOffsets.Add(JungleCamps.BlueWight, GetBasePointerForCamp("monsterCamp_13") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.BlueBlueBuff, GetBasePointerForCamp("monsterCamp_1") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.BlueWolves, GetBasePointerForCamp("monsterCamp_2") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.BlueWraiths, GetBasePointerForCamp("monsterCamp_3") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.BlueRedBuff, GetBasePointerForCamp("monsterCamp_4") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.BlueGolems, GetBasePointerForCamp("monsterCamp_5") + offsetToAliveByte);

            // Red side
            jungleOffsets.Add(JungleCamps.RedWight, GetBasePointerForCamp("monsterCamp_14") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.RedBlueBuff, GetBasePointerForCamp("monsterCamp_7") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.RedWolves, GetBasePointerForCamp("monsterCamp_8") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.RedWraiths, GetBasePointerForCamp("monsterCamp_9") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.RedRedBuff, GetBasePointerForCamp("monsterCamp_10") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.RedGolems, GetBasePointerForCamp("monsterCamp_11") + offsetToAliveByte);

            // River
            jungleOffsets.Add(JungleCamps.Baron, GetBasePointerForCamp("monsterCamp_12") + offsetToAliveByte);
            jungleOffsets.Add(JungleCamps.Dragon, GetBasePointerForCamp("monsterCamp_6") + offsetToAliveByte);
        }

        /// <summary>
        /// Returns a value indicating whether a jungle camp, specified by a unique identifier string, is alive.
        /// Note that the camp must have been seen while dead for this function to indicate the camp being dead properly.
        /// </summary>
        /// <param name="campName">A string specifier for the camp which must be contained in the jungleOffsets dictionary.</param>
        /// <returns>1 if the camp is alive, 2 if it is dead. Note that the camp must have been seen while dead for this to work.</returns>
        /// <exception cref="NullPointerException">Thrown when a null pointer is encountered at some point when traversing the pointer chain.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during traversal of the pointer chain.</exception>
        /// <exception cref="MissingFieldException">Thrown when no jungle camp matches the identifier provided.</exception>
        public bool GetJungleCampStatus(JungleCamps campID)
        {
            if (jungleOffsets == null)
                throw new MissingFieldException("Jungle camps have not been initialized yet. Have you called InitializeJungleOffsets yet?");
            if (!jungleOffsets.ContainsKey(campID))
                throw new MissingFieldException("No jungle camp reference exists for the id '" + campID + "'.");

            int bytesRead;
            byte[] result = ReadMemory(jungleOffsets[campID], 1, out bytesRead);
            return result[0] == 1 ? true : false;
        }

        /// <summary>
        /// Read memory from the process managed by this class.
        /// </summary>
        /// <param name="address">The address in memory to read.</param>
        /// <param name="numOfBytes">The number of bytes to read from that address.</param>
        /// <param name="bytesRead">The number of bytes that were read.</param>
        /// <returns>The contents of the memory address.</returns>
        private byte[] ReadMemory(IntPtr address, int numOfBytes, out int bytesRead)
        {
            IntPtr hProc = OpenProcess(ProcessAccessFlags.VMRead, false, process.Id);

            byte[] buffer = new byte[numOfBytes];

            bool worked = ReadProcessMemory(hProc, address, buffer, numOfBytes, out bytesRead);
            if (!worked)
            {
                string exceptionMessage = "Error encountered when reading process memory: " + GetLastError();
                CloseHandle(hProc);
                throw new Exception(exceptionMessage);
            }

            CloseHandle(hProc);
            return buffer;
        }

        /// <summary>
        /// Write memory to the process managed by this class.
        /// </summary>
        /// <param name="address">The address in memory to write.</param>
        /// <param name="value">The number of bytes to write to that address.</param>
        /// <param name="bytesWritten">The number of bytes that were written</param>
        /// <returns>True if worked, false otherwise.</returns>
        private bool WriteMemory(IntPtr address, long value, out int bytesWritten)
        {
            IntPtr hProc = OpenProcess(ProcessAccessFlags.All, false, process.Id);

            byte[] bytesToWrite = BitConverter.GetBytes(value);

            bool worked = WriteProcessMemory(hProc, address, bytesToWrite, (UInt32)bytesToWrite.LongLength, out bytesWritten);
            if (!worked)
            {
                string exceptionMessage = "Error encountered when writing process memory: " + GetLastError();
                CloseHandle(hProc);
                throw new Exception(exceptionMessage);
            }
            
            CloseHandle(hProc);
            return worked;
        }

        /// <summary>
        /// Read a chain of pointers, using offsets provided in an array to navigate the chain.
        /// </summary>
        /// <param name="offsets">An array of values to use as offsets at various stages of indirection in the pointer chain.
        /// The first value in the array will be used as an offset from the base pointer, the second value used as an offset from
        /// the dereferenced base pointer, and the nth value used as an offset from the nth dereferenced pointer.</param>
        /// <param name="bytesToRead">The number of bytes to read at the end of the pointer chain.</param>
        /// <param name="basePointer">The pointer to start reading from.</param>
        /// <returns>An array of bytes with the length specified in the bytesToRead parameter.
        /// This will contain the bytes at the memory address found at the end of the pointer chain.</returns>
        /// <exception cref="NullPointerException">Thrown when a null pointer is encountered at some point when traversing the pointer chain.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during traversal of the pointer chain.</exception>
        public byte[] ReadPointerChain(int[] offsets, int bytesToRead, IntPtr basePointer)
        {
            try
            {
                // Read from each pointer in the pointer chain, starting at the base address
                byte[] memoryValue;

                IntPtr currentPtrAddress = basePointer;
                int i = 0, offset = 0;
                bool doneInitial = false;

                // read all but the final offset, the final offset is what we return
                while (i < (offsets.Length - 1) || !doneInitial)
                {
                    // if we haven't dereferenced the original pointer yet
                    // use a 0 offset, instead of one from the offset list
                    offset = doneInitial ? offsets[i] : 0;

                    memoryValue = ReadPointerMemory(currentPtrAddress, offset, 4);

                    // update the next pointer to read baed on what we just read
                    currentPtrAddress = new IntPtr(BitConverter.ToInt32(memoryValue, 0));
                    if (currentPtrAddress.ToInt32() == 0)
                    {
                        throw new NullPointerException("Found null pointer at " + i + "th level of indirection.");
                    }

                    // if we've done the initial indirection, increment i
                    // otherwise we still need to use the first value in the offsets array
                    if (!doneInitial)
                        doneInitial = true;
                    else
                        i++;
                }

                // Read the final value, using the final offset in the list
                memoryValue = ReadPointerMemory(currentPtrAddress, offsets[offsets.Length - 1], bytesToRead);
                return memoryValue;
            }
            catch (NullPointerException e)
            {
                throw e; // handle elsewhere
            }
            catch (Exception e)
            {
                throw e; // handle elsewhere
            }
        }

        /// <summary>
        /// Read a memory address.
        /// </summary>
        /// <param name="offset">Offset from pointer to access.</param>
        /// <param name="bytesToRead">Number of bytes to read.</param>
        /// <param name="pointer">The pointer to read from.</param>
        /// <returns>The bytes found at the memory address.</returns>
        public byte[] ReadPointerMemory(IntPtr pointer, int offset, int bytesToRead)
        {
            byte[] memoryValue;
            int bytesRead;

            IntPtr pointerPlusOffset = IntPtr.Add(pointer, offset);

            memoryValue = ReadMemory(pointerPlusOffset, bytesToRead, out bytesRead);
            if (bytesRead != bytesToRead)
            {
                throw new Exception("Could not read all " + bytesToRead + "bytes at memory address 0x"
                    + pointerPlusOffset.ToInt32() + ". Read " + bytesRead + "bytes.");
            }
            return memoryValue;
        }

        /// <summary>
        /// Overloaded method for reading a memory address, meant to start at the base jungle pointer.
        /// </summary>
        /// <param name="offset">Offset from pointer to access.</param>
        /// <param name="bytesToRead">Number of bytes to read.</param>
        /// <returns>The bytes found at the memory address.</returns>
        public byte[] ReadPointerMemory(int offset, int bytesToRead)
        {
            return ReadPointerMemory(baseJunglePointer, offset, bytesToRead);
        }

        /// <summary>
        /// Reads a 4 byte pointer from a memory location found by scanning the program memory 
        /// (i.e. code) for a specific byte signature.
        /// </summary>
        /// <param name="bytePattern">The signature of bytes as a hex string where any invalid
        /// hex character counts as a wildcard.</param>
        /// <param name="offset">Number of bytes from the END of the matched pattern to
        /// start reading the memory address from.</param>
        /// <returns>The memory address found.</returns>
        private IntPtr ScanProgramAndReadAddress(string bytePattern, int offset)
        {
            IntPtr hProc = OpenProcess(ProcessAccessFlags.VMRead, false, process.Id);
            InitializeSigScan((uint)process.Id, process.MainModule.ModuleName);
            uint memloc = SigScan(bytePattern, offset);
            FinalizeSigScan();
            CloseHandle(hProc);

            return new IntPtr(memloc);
        }

        /// <summary>
        /// Scans ALL program memory looking for a specific byte pattern. This includes memory granted by
        /// calls to malloc and related system calls.
        /// </summary>
        /// <param name="bytePattern">The byte pattern to search for.</param>
        /// <returns>The memory address where the start of the byte pattern was found.</returns>
        private IntPtr ScanMemoryAndReadAddress(string bytePattern)
        {
            IntPtr position;
            using (MemoryScanner scanner = new MemoryScanner(process))
            {
                //position = scanner.ScanForBytes("F01FCF1EA71C1F197615F31262111711");
                position = scanner.ScanForBytes(bytePattern);
            }
            return position;
        }

        /// <summary>
        /// Helper function to transform a string like "hello" into its byte string
        /// equivalent, i.e. "68656C6C6F".
        /// </summary>
        /// <param name="input">The string to transform.</param>
        /// <returns>The transformed string</returns>
        private string AsciiToHexString(string input)
        {
            string output = "";
            foreach (char c in input)
            {
                output += System.Convert.ToInt32(c).ToString("X");
            }
            return output;
        }

        /// <summary>
        /// Gets the pointer to the base of the class that is used to describe jungle
        /// camps. Each camp has a name beginning with monsterCamp_ followed by a number
        /// which denotes the actual camp.
        /// </summary>
        /// <param name="campName">The name of the camp to get.</param>
        /// <returns>The base pointer to the jungle camp class.</returns>
        private IntPtr GetBasePointerForCamp(string campName)
        {
            // e.g. turn monsterCamp_13 into "6D6F6E7374657243616D705F3133"
            string searchString = AsciiToHexString(campName);
            IntPtr result =  ScanMemoryAndReadAddress(searchString);
            return result - 0x28;
            // at the time of coding, the name was 0x28 (40) bytes ahead of the start of the class
            // but this might need updating if some patches touch the base object class
            // of the game. Unlikely though
        }

        /// <summary>
        /// Gets the offset to use to access the byte in a neutral camp class that shows whether
        /// a camp is alive.
        /// </summary>
        /// <returns>The offset to access the class member showing whether a camp is alive.</returns>
        private IntPtr GetOffsetToAliveStatus()
        {
            // Scans for the instructions:
            // lea ebx, [ebx] (not used, skipped over by a jump but useful for scanning)
            // mov ebx, [esi + 14] (load ebx with a struct at +14, this makes ebx point to the neutral camp)
            // mov al, [ebx + OFFSET] (where al will now be the alive byte, OFFSET is the value we are finding)
            return ScanProgramAndReadAddress("8D9B000000008B5E??8A83", 0);
        }

        /// <summary>
        /// Gets the base pointer that is used to access all other jungle entities.
        /// </summary>
        /// <returns>The base pointer to the jungle entities.</returns>
        private IntPtr GetBaseJunglePointer()
        {
            //return ScanProgramAndReadAddress("508D8720090000508B11FF526480BFF1000000000F84????????8B0D", 0);
            //return IntPtr.Add(process.MainModule.BaseAddress, 0x1151098);
            return new IntPtr(0); // function deprecated - using different method to find neutral camps now
        }

        /// <summary>
        /// Gets the base pointeer that is used to access the in game clock.
        /// </summary>
        /// <returns>The base pointer to the in game clock.</returns>
        private IntPtr GetBaseGameTimePointer()
        {
            return ScanProgramAndReadAddress("83B9D800000000A1", 0);
        }

        /// <summary>
        /// Get the current game time in seconds as a float.
        /// </summary>
        /// <returns>The current game time in seconds.</returns>
        /// <exception cref="NullPointerException">Thrown when a null pointer is encountered at some point when traversing the pointer chain.</exception>
        /// <exception cref="Exception">Thrown when an error occurs during traversal of the pointer chain.</exception>
        public float GetCurrentGameTime()
        {
            return BitConverter.ToSingle(ReadPointerChain(gameTimeOffsets, 4, baseGameTimePointer), 0);
        }
    }
}
