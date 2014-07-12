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

//
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.Remoting;
using System.Windows.Forms;
using System.Threading;
using LoLTimers;
//

namespace LoLTimersConsole
{
    public class Program
    {
        static void Main(string[] args)
        {
            LoLMonitor monitor;
            Console.CursorVisible = false;
            monitor = new LoLMonitor();

            // Search for LoL processes while we are open
            while (!monitor.HasProcess)
            {
                Console.Out.WriteLine("Searching for LoL process...");
                monitor.SearchForLoLProcess();
                monitor.StartPolling(false);

                while (!monitor.AttachedProcess.HasExited) 
                {
                    monitor.PrintTimersToConsole();
                    Thread.Sleep(1000);
                }

                monitor.DisposeProcess();
            }
        }
    }
}