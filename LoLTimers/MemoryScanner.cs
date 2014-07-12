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
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace LoLTimers
{
    class MemoryScanner : IDisposable
    {
        Process process;
        SYSTEM_INFO sysInfo;
        MEMORY_BASIC_INFORMATION memInfo;
        private IntPtr hProc = new IntPtr(0);

        [DllImport("kernel32.dll")]
        static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);

        public struct SYSTEM_INFO
        {
            public ushort processorArchitecture;
            ushort reserved;
            public uint pageSize;
            public IntPtr minimumApplicationAddress;  // minimum address
            public IntPtr maximumApplicationAddress;  // maximum address
            public IntPtr activeProcessorMask;
            public uint numberOfProcessors;
            public uint processorType;
            public uint allocationGranularity;
            public ushort processorLevel;
            public ushort processorRevision;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

        public struct MEMORY_BASIC_INFORMATION
        {
            public int BaseAddress;
            public int AllocationBase;
            public int AllocationProtect;
            public int RegionSize;   // size of the region allocated by the program
            public int State;   // check if allocated (MEM_COMMIT)
            public int Protect; // page protection (must be PAGE_READWRITE)
            public int lType;
        }

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(int hProcess, int lpBaseAddress, byte[] lpBuffer, int dwSize, ref int lpNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        private static extern IntPtr OpenProcess(ProcessAccessFlags dwDesiredAccess, [MarshalAs(UnmanagedType.Bool)] bool bInheritHandle, int dwProcessId);

        [DllImport("kernel32.dll")]
        private static extern Int32 CloseHandle(IntPtr hProcess);

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

        // Memory access flags
        const int PROCESS_QUERY_INFORMATION = 0x0400;
        const int MEM_COMMIT = 0x00001000;
        const int PAGE_READWRITE = 0x04;
        const int PROCESS_WM_READ = 0x0010;

        /// <summary>
        /// Create a new memory scanner to scan the given process.
        /// </summary>
        /// <param name="process">The process to scan.</param>
        public MemoryScanner(Process process)
        {
            this.process = process;
            sysInfo = new SYSTEM_INFO();
            memInfo = new MEMORY_BASIC_INFORMATION();
            GetSystemInfo(out sysInfo);
            hProc = OpenProcess(ProcessAccessFlags.VMRead | ProcessAccessFlags.QueryInformation, false, process.Id);
        }

        /// <summary>
        /// Helper function to transform a string like "hello" into its byte string
        /// equivalent, i.e. "68656C6C6F" where each pair of ASCII characters is a HEX byte.
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

        public int SearchBytesForPattern(byte[] haystack, byte[] needle) {
            int offset = 0;
            byte[] badByteSkip = new byte[Byte.MaxValue + 1];

            // Boyer-Moore-Horspool algorithm used to search
            // Adapted from http://en.wikipedia.org/wiki/Boyer%E2%80%93Moore_string_search_algorithm

            // Preprocessing
            // Initialize bytes analysis
            // When a character is encountered that does not occur in the
            // search pattern, we can safely skip ahead for the whole length
            // of the needle.
            // Otherwise, we only skip ahead enough characters to match the
            // next character in our pattern with the bytes.
            for (offset = 0; offset <= Byte.MaxValue; offset++)
            {
                badByteSkip[offset] = Convert.ToByte(needle.Length);
            }

            int lastByte = needle.Length - 1; // last byte of the needle array

            // Analyse the bytes
            // For each possible byte value, we store the amount
            // that we need to skip ahead in the string
            for (offset = 0; offset < lastByte; offset++)
            {
                badByteSkip[needle[offset]] = Convert.ToByte(lastByte - offset);
            }

            // Matching
            offset = haystack.Length;
            int hPos = 0;

            // search while the needle can still possibly be present in the haystack
            while (offset >= needle.Length)
            {
                // scan from the end of the needle
                for (int scan = lastByte; haystack[scan + hPos] == needle[scan]; scan--)
                {
                    if (scan == 0)
                        return hPos; // if the first byte matches we've found it
                }

                /* otherwise, we need to skip some bytes and start again.
                      Note that here we are getting the skip value based on the last byte
                      of needle, no matter where we didn't match. So if needle is: "abcd"
                      then we are skipping based on 'd' and that value will be 4, and
                      for "abcdd" we again skip on 'd' but the value will be only 1.
                      The alternative of pretending that the mismatched character was
                      the last character is slower in the normal case (E.g. finding
                      "abcd" in "...azcd..." gives 4 by using 'd' but only
                      4-2==2 using 'z'. */
                offset -= badByteSkip[haystack[hPos + lastByte]];
                hPos += badByteSkip[haystack[hPos + lastByte]];
            }

            return 0;
        }

        private byte[] StringToByteArray(string bytes)
        {
            byte[] pattern = new byte[bytes.Length / 2];
            int patternPos = 0;
            for (int i = 0; i < bytes.Length; i += 2)
            {
                pattern[patternPos] = System.Convert.ToByte(bytes.Substring(i, 2), 16);
                patternPos++;
            }
            return pattern;
        }

        public IntPtr ScanForBytes(string bytes)
        {
            byte[] pattern = StringToByteArray(bytes);
            long minAppAddr = (long)sysInfo.minimumApplicationAddress;
            long maxAppAddr = (long)sysInfo.maximumApplicationAddress;
            long curAppAddr = minAppAddr;

            int bytesRead = 0;
            int offset = 0;
            while (curAppAddr < maxAppAddr)
            {
                // 28 = sizeof(MEMORY_BASIC_INFORMATION), just hardcoded for ease
                VirtualQueryEx(hProc, new IntPtr(curAppAddr), out memInfo, 28);
                // TODO: if the region is excessively large and wont fit into contiguous memory, subdivide it
                byte[] buffer = new byte[memInfo.RegionSize];

                if (memInfo.Protect == PAGE_READWRITE && memInfo.State == MEM_COMMIT)
                {
                    ReadProcessMemory(
                        (int)hProc,
                        memInfo.BaseAddress,
                        buffer, memInfo.RegionSize,
                        ref bytesRead
                    );

                    // search the buffer for our memory pattern
                    offset = SearchBytesForPattern(buffer, pattern);
                    if (offset != 0) {
                        // add the base page address to the offset to get the final offset
                        offset += (int)curAppAddr;
                        break;
                    }
                }
                // move to the next memory page
                curAppAddr += memInfo.RegionSize;
            }
            return new IntPtr(offset);
        }

        ~MemoryScanner()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!IntPtr.Equals(hProc, new IntPtr(0)))
                CloseHandle(hProc);
            hProc = new IntPtr(0);
        }
    }
}
