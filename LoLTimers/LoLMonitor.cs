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
    public enum JungleCamps
    {
        BlueWight,
        BlueBlueBuff,
        BlueWolves,
        BlueWraiths,
        BlueRedBuff,
        BlueGolems,
        RedWight,
        RedBlueBuff,
        RedWolves,
        RedWraiths,
        RedRedBuff,
        RedGolems,
        Baron,
        Dragon
    }

    /// <summary>
    /// Finds an instance of a League of Legends process.
    /// </summary>
    public class LoLProcessFinder
    {
        private static string processName = "League of Legends";

        private Process process = null;
        public Process Process {
            get
            {
                Process retProcess;
                lock (process)
                {
                    retProcess = process;
                }
                return process;
            }
        }

        private bool foundProcess;
        public bool FoundProcess { get { return foundProcess; } }

        public LoLProcessFinder()
        {
            foundProcess = false;
        }
        

        /// <summary>
        /// Find the process.
        /// </summary>
        public void FindProcess()
        {
            Process lolProcess = null;
            
            while (lolProcess == null) {
                lolProcess = Process.GetProcessesByName(processName).FirstOrDefault();

                if (lolProcess != null)
                {
                    process = lolProcess;
                    foundProcess = true;
                    return;
                }

                Thread.Sleep(500);
            }
        }
    }

    /// <summary>
    /// Monitors a League of Legends process.
    /// This class must have the SearchForLolProcess method run to find a League of Legends
    /// process before it can be used to poll the process.
    /// </summary>
    public class LoLMonitor : IDisposable
    {
        private Process process;
        public Process AttachedProcess { get { return process; } }
        public bool HasProcess { get { return !(process == null); } }

        private ProcessMemoryManager memoryManager;
        private Dictionary<JungleCamps, JungleTimerInfo> jungleStatus;
        private object jungleStatusLock = new Object();
        private int currentGameTime = 0;
        public int CurrentGameTime { get { return currentGameTime; } }

        private Thread pollThread;
        private bool stopPolling = false;

        public LoLMonitor()
        {
            InitializeJungleTimers();
        }

        /// <summary>
        /// Starts a blocking search for a league of legends process. Records the process when found, and returns.
        /// </summary>
        public void SearchForLoLProcess()
        {
            LoLProcessFinder pFinder = new LoLProcessFinder();

            Thread searchThread = new Thread(new ThreadStart(pFinder.FindProcess));
            searchThread.IsBackground = true;
            searchThread.Start();

            while (!pFinder.FoundProcess)
            {
                // give the search thread some execution time
                Thread.Sleep(500);
            }

            process = pFinder.Process;
            memoryManager = new ProcessMemoryManager(process);
        }

        public void Dispose()
        {
            DisposeProcess();
        }

        /// <summary>
        /// Cleans up the current polling thread and de-attaches the current process
        /// from this monitor.
        /// </summary>
        public void DisposeProcess()
        {
            StopPolling();
            if (process != null)
            {
                process.Dispose();
                process = null;
            }
        }

        /// <summary>
        /// Stops the polling thread from executing, and disposes of it.
        /// </summary>
        public void StopPolling()
        {
            if (pollThread != null)
            {
                stopPolling = true;
                pollThread = null;
            }
        }

        /// <summary>
        /// Starts the active monitoring of the LoL process. Monitoring is done on a new thread, managed by this class.
        /// <param name="freezeThread">Whether to block the calling thread until the attached LoL process has exited.</param>
        /// </summary>
        public void StartPolling(bool freezeThread)
        {
            StartPolling();
            if (freezeThread)
            {
                process.WaitForExit(); // freeze the main thread until league has been closed
                //Console.WriteLine("League of Legends process has been closed.");
            }
        }

        /// <summary>
        /// Starts the active monitoring of the LoL process. Monitoring is done on a new thread, managed by this class.
        /// </summary>
        public void StartPolling()
        {
            if (memoryManager == null)
                throw new NoProcessFoundException("No process has been attached to this monitor yet.");
            stopPolling = false; // this value can persist between multiple uses of the class

            memoryManager.InitializeJungleOffsets(); // this will block until the game clock starts

            pollThread = new Thread(this.PollJungleTimers);
            pollThread.Start();
        }

        /// <summary>
        /// Initializer function for the jungle timer data structure.
        /// </summary>
        private void InitializeJungleTimers()
        {
            jungleStatus = new Dictionary<JungleCamps, JungleTimerInfo>();

            // Blue side
            jungleStatus.Add(JungleCamps.BlueWight, new JungleTimerInfo(60, 125, "Blue Wight", JungleCamps.BlueWight));
            jungleStatus.Add(JungleCamps.BlueBlueBuff, new JungleTimerInfo(60 * 5, 115, "Blue BlueBuff", JungleCamps.BlueBlueBuff));
            jungleStatus.Add(JungleCamps.BlueWolves, new JungleTimerInfo(60, 125, "Blue Wolves", JungleCamps.BlueWolves));
            jungleStatus.Add(JungleCamps.BlueWraiths, new JungleTimerInfo(50, 125, "Blue Wraiths", JungleCamps.BlueWraiths));
            jungleStatus.Add(JungleCamps.BlueRedBuff, new JungleTimerInfo(60 * 5, 115, "Blue RedBuff", JungleCamps.BlueRedBuff));
            jungleStatus.Add(JungleCamps.BlueGolems, new JungleTimerInfo(60, 125, "Blue Golems", JungleCamps.BlueGolems));

            // Red side
            jungleStatus.Add(JungleCamps.RedWight, new JungleTimerInfo(60, 125, "Red Wight", JungleCamps.RedWight));
            jungleStatus.Add(JungleCamps.RedBlueBuff, new JungleTimerInfo(60 * 5, 115, "Red BlueBuff", JungleCamps.RedBlueBuff));
            jungleStatus.Add(JungleCamps.RedWolves, new JungleTimerInfo(60, 125, "Red Wolves", JungleCamps.RedWolves));
            jungleStatus.Add(JungleCamps.RedWraiths, new JungleTimerInfo(50, 125, "Red Wraiths", JungleCamps.RedWraiths));
            jungleStatus.Add(JungleCamps.RedRedBuff, new JungleTimerInfo(60 * 5, 115, "Red RedBuff", JungleCamps.RedRedBuff));
            jungleStatus.Add(JungleCamps.RedGolems, new JungleTimerInfo(60, 125, "Red Golems", JungleCamps.RedGolems));

            // River
            jungleStatus.Add(JungleCamps.Baron, new JungleTimerInfo(60 * 7, 60 * 15, "Baron", JungleCamps.Baron));
            jungleStatus.Add(JungleCamps.Dragon, new JungleTimerInfo(60 * 6, 150, "Dragon", JungleCamps.Dragon));
        }

        /// <summary>
        /// Returns a copy of the current jungle timers.
        /// </summary>
        /// <returns>A copy of the jungle timers.</returns>
        public List<JungleTimerInfo> GetJungleTimers()
        {
            List<JungleTimerInfo> timersCopy = new List<JungleTimerInfo>();
            lock (jungleStatusLock)
            {
                foreach (KeyValuePair<JungleCamps, JungleTimerInfo> entry in jungleStatus)
                {
                    // Make sure we clone here so that the objects in our collection stay safe from access
                    timersCopy.Add((JungleTimerInfo)entry.Value.Clone());
                }
            }
            return timersCopy;
        }

        /// <summary>
        /// Timer function to control polling the memory of the League of Legends client.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        /// <param name="memoryManager">The memory manager to use for the monitoring.</param>
        private void PollJungleTimers()
        {
            while (!stopPolling)
            {
                try
                {
                    currentGameTime = (int)Math.Floor(memoryManager.GetCurrentGameTime());
                }
                catch (NullPointerException)
                {
                    // We couldn't read the game time
                    // because it likely hadn't been initialised yet
                    return;
                }
                catch (Exception)
                {
                    // A more serious memory reading error has occurred
                    // this is likely due to the process being closed,
                    // or a misaligned pointer.
                    return;
                }

                // TODO: turn the error messages into an error log
                lock (jungleStatusLock)
                {
                    foreach (KeyValuePair<JungleCamps, JungleTimerInfo> entry in jungleStatus)
                    {
                        bool isAlive = false;
                        bool jungleHasSpawned = true;
                        try
                        {
                            isAlive = memoryManager.GetJungleCampStatus(entry.Key);
                        }
                        catch (NullPointerException)
                        {
                            // We couldn't read the camp entity
                            // because there was a null pointer on the chain.
                            // Likely means that the jungle camp hasn't had its
                            // memory initialised yet.
                            jungleHasSpawned = false;
                        }
                        catch (MissingFieldException e)
                        {
                            // The jungle camp specified does not exist
                            Console.Error.WriteLine("Error: " + e.Message);
                            continue;
                        }
                        catch (Exception e)
                        {
                            // A more serious memory exception was encountered.
                            // Reading memory protected areas or null pointers for example.
                            // Log it and continue
                            Console.Error.WriteLine("Error: " + e.Message);
                            continue;
                        }

                        // If the camp hasn't spawned yet, make a note
                        if (entry.Value.initialSpawnTime > currentGameTime)
                        {
                            jungleHasSpawned = false;
                        }

                        // If the camp status has changed, record the time of death if appropriate.
                        if (isAlive != entry.Value.currentlyAlive)
                        {
                            if (isAlive == false && entry.Value.currentlyAlive == true)
                                entry.Value.timeWhenKilled = currentGameTime; // if it just died, record the current time
                        }

                        // Update our record
                        entry.Value.currentlyAlive = isAlive;

                        // Make an update to the display field
                        if (!jungleHasSpawned)
                        {
                            entry.Value.timeUntilRespawn = entry.Value.initialSpawnTime - currentGameTime;
                        }
                        else if (!entry.Value.currentlyAlive)
                        {
                            entry.Value.timeUntilRespawn = (entry.Value.timeWhenKilled + entry.Value.respawnTime) - currentGameTime;
                        }
                        else
                        {
                            entry.Value.timeUntilRespawn = 0;
                        }
                    }
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Prints the current jungle timers to the console.
        /// </summary>
        public void PrintTimersToConsole()
        {
            Console.Clear();
            lock (jungleStatusLock)
            {
                foreach (KeyValuePair<JungleCamps, JungleTimerInfo> entry in jungleStatus)
                {
                    bool jungleHasSpawned = true;
                    if (entry.Value.initialSpawnTime > currentGameTime)
                    {
                        jungleHasSpawned = false;
                    }

                    Console.Out.Write("{0}: ", entry.Value.campName);
                    if (!jungleHasSpawned)
                    {
                        int min = entry.Value.timeUntilRespawn / 60;
                        int sec = entry.Value.timeUntilRespawn % 60;
                        Console.Out.WriteLine("Spawns in {0:D2}:{1:D2} ({2:D2}:{3:D2})", min, sec, entry.Value.initialSpawnTime / 60, entry.Value.initialSpawnTime % 60);
                    }
                    else if (!entry.Value.currentlyAlive)
                    {
                        int min = entry.Value.timeUntilRespawn / 60;
                        int sec = entry.Value.timeUntilRespawn % 60;
                        int minsAtRespawn = (entry.Value.timeWhenKilled + entry.Value.respawnTime) / 60;
                        int secsAtRespawn = (entry.Value.timeWhenKilled + entry.Value.respawnTime) % 60;
                        Console.Out.WriteLine("Respawns in {0:D2}:{1:D2} ({2:D2}:{3:D2})", min, sec, minsAtRespawn, secsAtRespawn);
                    }
                    else
                    {
                        Console.Out.WriteLine("Alive");
                    }
                }
            }
        }


    }
}
