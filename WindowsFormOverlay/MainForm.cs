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

using LoLTimers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace WinFormsTimerOverlay
{
    public partial class MainForm : Form
    {
        private LoLMonitor monitor = new LoLMonitor();
        private OverlayForm overlay;
        private bool keepMonitoring = true;

        delegate void SetCurrentActionCallback(string action); // delegate for displaying current action
        delegate void MakeOverlayFormCallback(); // delegate for creating overlay form
        delegate void CloseOverlayFormCallback(); // delegatee for closing the overlay form

        public MainForm()
        {
            InitializeComponent();
            monitor = new LoLMonitor();
        }

        /// <summary>
        /// Intended to be run on a separate thread. Waits for a LoL process to be opened
        /// and provides feedback to the main form. Will continue until the application is closed.
        /// </summary>
        public void MonitorThread()
        {
            MakeOverlayFormCallback createOverlayDelegate = new MakeOverlayFormCallback(ShowOverlay);
            CloseOverlayFormCallback closeOverlayDelegate = new CloseOverlayFormCallback(CloseOverlayAndCleanup);

            while (keepMonitoring)
            {
                SearchForLoLProcess(); // this will block
                this.Invoke(createOverlayDelegate);
                while (keepMonitoring && !monitor.AttachedProcess.HasExited)
                {
                    // Update the process, in case things change on us
                    lock (monitor)
                    {
                        monitor.AttachedProcess.Refresh();
                    }
                    Thread.Sleep(1000);
                }
                if (keepMonitoring)
                {
                    // if the main window has been closed, don't try and invoke
                    // on a dead reference
                    this.Invoke(closeOverlayDelegate);
                }
            }
        }

        /// <summary>
        /// Searches for an open LoL process. Blocks until one is found.
        /// </summary>
        private void SearchForLoLProcess()
        {
            SetCurrentAction("waiting for League of Legends to open.");
            monitor.SearchForLoLProcess(); // this will block
        }

        /// <summary>
        /// Opens the overlay form to show LoL timers.
        /// </summary>
        private void ShowOverlay()
        {
            overlay = new OverlayForm(monitor);
            SetCurrentAction("showing overlay.");
            overlay.Show();

            // Start the timing UI on a separate thread so our UI
            // isn't affected by any blocking
            Thread uiTimer = new Thread(new ThreadStart(overlay.StartTimerUI));
            uiTimer.IsBackground = true;
            uiTimer.Start();
        }

        /// <summary>
        /// Closes the overlay and cleans up its resources.
        /// </summary>
        private void CloseOverlayAndCleanup()
        {
            overlay.Close();
            monitor.DisposeProcess(); // dispose of our polling thread and current league process
        }

        /// <summary>
        /// Changes the text in the current action label. Thread safe.
        /// </summary>
        /// <param name="action">Text to use in the label.</param>
        public void SetCurrentAction(string action)
        {
            if (labelAction.InvokeRequired)
            {
                SetCurrentActionCallback d = new SetCurrentActionCallback(SetCurrentAction);
                try
                {
                    labelAction.Invoke(d, new object[] { action });
                }
                catch (System.ObjectDisposedException)
                {
                    // The form has been closed on us.
                }
            }
            else
            {
                labelAction.Text = String.Format("Current action: {0}", action);
            }
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Thread monitorThread = new Thread(MonitorThread);
            monitorThread.IsBackground = true;
            monitorThread.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            keepMonitoring = false;
            lock (monitor)
            {
                monitor.DisposeProcess();
            }
        }
    }
}
