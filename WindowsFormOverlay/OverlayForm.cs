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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using LoLTimers;
using System.Drawing.Imaging;

namespace WinFormsTimerOverlay
{
    public partial class OverlayForm : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern UInt32 GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetWindowRect(IntPtr hWnd, ref RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        // RECT struct used with some Windows API calls
        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        // Flags for window configuration
        public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int WS_EX_NOACTIVATE = 0x8000000;
        public const int WS_EX_TOOLWINDOW = 0x80;
        public const int WS_EX_TRANSPARENT = 0x20;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;
        public const int WM_NCHITTEST = 0x84;
        public const int HTTRANSPARENT = -1;

        // Lookup table for timer positions at different resolution widths
        

        LoLMonitor monitor; // The LoLMonitor to monitor the process with
        Thread matchLeagueWindow, updateTimers; // Thread handles

        public OverlayForm()
        {
            InitializeComponent();

            List<Control> controls = new List<Control>();
            controls.Add(this);

            // Display "loading..." to each label on the form until
            // the monitor actually starts sending updates
            while (controls.Count > 0)
            {
                Control current = controls.ElementAt(0);
                controls.RemoveAt(0); // pop the first element

                if (current is Label)
                    SetLabelTextProperty((Label)current, "loading...");
                else
                    foreach (Control child in current.Controls)
                        controls.Add(child); // add each child to our list
            }
        }

        public OverlayForm(LoLMonitor monitor)
            : this()
        {
            this.monitor = monitor;
        }

        /// <summary>
        /// Starts the UI updating for the timers. Uses multiple threads to achieve its results.
        /// The UI based threads will terminate when the form is closed, but the polling will
        /// remain until the LoLMonitor is disposed of, or explicitly told to stop polling.
        /// </summary>
        public void StartTimerUI() {
            // Start the matching thread first so that the UI is put into the right place
            // even if we're waiting for memory addresses
            matchLeagueWindow = new Thread(new ThreadStart(this.WindowMatchThread));
            matchLeagueWindow.IsBackground = true;
            matchLeagueWindow.Start();

            // Start polling next, because this will block until it has all the required
            // memory addresses
            monitor.StartPolling();

            // Start updating our UI elements once the polling function has returned
            // and we know that our polling thread has begun.
            updateTimers = new Thread(new ThreadStart(this.UpdateTimersThread));
            updateTimers.IsBackground = true;
            updateTimers.Start();
        }

        /// <summary>
        /// Updates the timer controls in our form so that they count down.
        /// </summary>
        /// <param name="monitor">The monitor to get our data from.</param>
        private void UpdateTimerControls(LoLMonitor monitor)
        {
            List<JungleTimerInfo> jungleTimers = monitor.GetJungleTimers();
            int currentGameTime = monitor.CurrentGameTime;

            JungleTimerInfo baron = jungleTimers.Find(x => x.campType == JungleCamps.Baron);
            UpdateTimerControl(labelBaron, baron, currentGameTime);

            JungleTimerInfo dragon = jungleTimers.Find(x => x.campType == JungleCamps.Dragon);
            UpdateTimerControl(labelDragon, dragon, currentGameTime);

            JungleTimerInfo blueBlue = jungleTimers.Find(x => x.campType == JungleCamps.BlueBlueBuff);
            UpdateTimerControl(labelBlueBlue, blueBlue, currentGameTime);

            JungleTimerInfo blueRed = jungleTimers.Find(x => x.campType == JungleCamps.BlueRedBuff);
            UpdateTimerControl(labelBlueRed, blueRed, currentGameTime);

            JungleTimerInfo redBlue = jungleTimers.Find(x => x.campType == JungleCamps.RedBlueBuff);
            UpdateTimerControl(labelRedBlue, redBlue, currentGameTime);

            JungleTimerInfo redRed = jungleTimers.Find(x => x.campType == JungleCamps.RedRedBuff);
            UpdateTimerControl(labelRedRed, redRed, currentGameTime);
        }

        /// <summary>
        /// Updates a single form label, given the provided timing info.
        /// </summary>
        /// <param name="label">The label to update.</param>
        /// <param name="timerInfo">Timing info with which to update the label.</param>
        /// <param name="currentGameTime">The current in game time.</param>
        private void UpdateTimerControl(Label label, JungleTimerInfo timerInfo, int currentGameTime)
        {
            bool timerHasSpawned = true;
            if (timerInfo.initialSpawnTime > currentGameTime)
            {
                timerHasSpawned = false;
            }
            if (!timerHasSpawned)
            {
                int min = timerInfo.timeUntilRespawn / 60;
                int sec = timerInfo.timeUntilRespawn % 60;
                SetLabelTextProperty(label, String.Format("spawns in {0:D2}:{1:D2} ({2:D2}:{3:D2})", min, sec, timerInfo.initialSpawnTime / 60, timerInfo.initialSpawnTime % 60));
            }
            else if (!timerInfo.currentlyAlive)
            {
                int min = timerInfo.timeUntilRespawn / 60;
                int sec = timerInfo.timeUntilRespawn % 60;
                int minsAtRespawn = (timerInfo.timeWhenKilled + timerInfo.respawnTime) / 60;
                int secsAtRespawn = (timerInfo.timeWhenKilled + timerInfo.respawnTime) % 60;
                SetLabelTextProperty(label, String.Format("respawns in {0:D2}:{1:D2} ({2:D2}:{3:D2})", min, sec, minsAtRespawn, secsAtRespawn));
            }
            else
            {
                SetLabelTextProperty(label, "alive");
            }
        }

        /// <summary>
        /// Thread to keep updating the timer controls.
        /// </summary>
        private void UpdateTimersThread()
        {
            while (!isClosed && !monitor.AttachedProcess.HasExited)
            {
                lock (monitor)
                {
                    UpdateTimerControls(monitor);
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Thread to keep matching our window size with the target processes main
        /// window size so that we keep up with any resolution changes or window
        /// movement.
        /// </summary>
        private void WindowMatchThread()
        {
            RECT rectBounds = new RECT();
            RECT lastRect = new RECT();
            System.Drawing.Point topLeft = new Point(0, 0);

            while (!isClosed)
            {
                topLeft.X = 0;
                topLeft.Y = 0;

                lock (monitor)
                {
                    // Get the size of the attached process window and match to it.
                    GetClientRect(monitor.AttachedProcess.MainWindowHandle, out rectBounds);
                    ClientToScreen(monitor.AttachedProcess.MainWindowHandle, ref topLeft);
                }

                // Don't change anything if the window hasn't changed
                if (lastRect.Bottom == rectBounds.Bottom && lastRect.Top == rectBounds.Top &&
                        lastRect.Left == rectBounds.Left && lastRect.Right == rectBounds.Right)
                {
                    lastRect.Bottom = rectBounds.Bottom;
                    lastRect.Left = rectBounds.Left;
                    lastRect.Top = rectBounds.Top;
                    lastRect.Right = rectBounds.Right;

                    Thread.Sleep(500);
                    continue;
                }

                // Match the form to the window rect
                int rectWidth = rectBounds.Right - rectBounds.Left;
                int rectHeight = rectBounds.Bottom - rectBounds.Top;
                this.SetFormSizeProperty(FormProperty.FormWidth, rectWidth);
                this.SetFormSizeProperty(FormProperty.FormHeight, rectHeight);
                this.SetFormSizeProperty(FormProperty.FormTop, topLeft.Y);
                this.SetFormSizeProperty(FormProperty.FormLeft, topLeft.X);

                // Store this rect as the last rect so we know next time if the
                // rect was updated and whether to move form controls or not.
                lastRect.Bottom = rectBounds.Bottom;
                lastRect.Left = rectBounds.Left;
                lastRect.Top = rectBounds.Top;
                lastRect.Right = rectBounds.Right;

                // Move the timers depending on the resolution
                Tuple<double, double> resInfo = ResolutionLookupTable.GetResolutionOffsets(rectWidth);
                this.SetFormSizeProperty(FormProperty.LeftLabelTop, (int)Math.Floor(rectHeight * 0.93));
                this.SetFormSizeProperty(FormProperty.LeftLabelLeft, (int)Math.Floor(rectWidth * resInfo.Item1));

                this.SetFormSizeProperty(FormProperty.RightLabelTop, (int)Math.Floor(rectHeight * 0.93));
                this.SetFormSizeProperty(FormProperty.RightLabelLeft, (int)Math.Floor(rectWidth * resInfo.Item2));
            }
        }

        delegate void SetLabelTextPropertyCallback(Label label, string text);

        /// <summary>
        /// Thread safe way to change label text.
        /// </summary>
        /// <param name="label">The label control to change.</param>
        /// <param name="text">What to change the label's text property to.</param>
        private void SetLabelTextProperty(Label label, string text)
        {
            if (label.InvokeRequired)
            {
                SetLabelTextPropertyCallback d = new SetLabelTextPropertyCallback(SetLabelTextProperty);
                try
                {
                    label.Invoke(d, new object[] { label, text });
                }
                catch (System.ObjectDisposedException)
                {
                    // The form has been closed on us.
                }
            }
            else
            {
                label.Text = text;
            }
        }

        /// <summary>
        /// Used for thread safe property setting of the form. Properties
        /// are referred to by their enum and dispatched through handlers.
        /// </summary>
        private enum FormProperty {
            // Form width, height and position specifiers
            FormWidth,
            FormHeight,
            FormTop,
            FormLeft,
            // Label group position specifiers
            LeftLabelTop,
            LeftLabelLeft,
            RightLabelTop,
            RightLabelLeft
        }

        delegate void SetFormSizePropertyCallback(FormProperty proeprty, int value);

        /// <summary>
        /// Thread safe way to change form properties.
        /// </summary>
        /// <param name="property">Property id to change.</param>
        /// <param name="value">Value to change to.</param>
        private void SetFormSizeProperty(FormProperty property, int value)
        {
            if (this.InvokeRequired)
            {
                SetFormSizePropertyCallback d = new SetFormSizePropertyCallback(SetFormSizeProperty);
                try
                {
                    this.Invoke(d, new object[] { property, value });
                }
                catch (System.ObjectDisposedException)
                {
                    // The form has been closed on us.
                }
            }
            else
            {
                switch (property)
                {
                    case FormProperty.FormWidth:
                        this.Width = value;
                        break;
                    case FormProperty.FormHeight:
                        this.Height = value;
                        break;
                    case FormProperty.FormLeft:
                        this.Left = value;
                        break;
                    case FormProperty.FormTop:
                        this.Top = value;
                        break;
                    case FormProperty.LeftLabelLeft:
                        panelDragon.Left = value;
                        panelBaron.Left = value;
                        panelBlueBlue.Left = value;
                        panelBlueRed.Left = value;
                        break;
                    case FormProperty.LeftLabelTop:
                        panelDragon.Top = value;
                        panelBaron.Top = panelDragon.Top - panelDragon.Height - 5;
                        panelBlueRed.Top = panelBaron.Top - panelBaron.Height - 5;
                        panelBlueBlue.Top = panelBlueRed.Top - panelBlueRed.Height - 5;
                        break;
                    case FormProperty.RightLabelLeft:
                        panelRedRed.Left = value;
                        panelRedBlue.Left = value;
                        break;
                    case FormProperty.RightLabelTop:
                        panelRedBlue.Top = value;
                        panelRedRed.Top = value - panelRedBlue.Height - 5;
                        break;
                }
            }
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {

        }

        private bool isClosed = false;
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            isClosed = true;
        }

        // ===== These don't work yet =====
        private void HidePictureBox(object sender, EventArgs e)
        {
            ((PictureBox)sender).Hide();
            MessageBox.Show("hiding");
        }

        private void ShowPictureBox(object sender, EventArgs e)
        {
            ((PictureBox)sender).Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //List<Control> list = new List<Control>();
            //GetAllPictureBoxes(this, ref list);

            //foreach (Control c in list)
            //{
            //    c.MouseEnter += HidePictureBox;
            //    c.MouseLeave += ShowPictureBox;
            //}
        }
        // ================================

        /// <summary>
        /// Get all controls that we wish to hide on mouseover.
        /// </summary>
        /// <param name="container">The container to begin searching in.</param>
        /// <param name="list">The list to populate.</param>
        private void GetAllPictureBoxes(Control container, ref List<Control> list)
        {
            foreach (Control c in container.Controls)
            {
                GetAllPictureBoxes(c, ref list);
                //if (c is Label) list.Add(c);
                if (c is PictureBox) list.Add(c);
            }
        }

        /// <summary>
        /// Override CreateParams to change the parameters for our overlay window.
        /// This is much more clean and efficient than using pinvoke to call
        /// SetWindowLong, and has the added bonus of ensuring that all the flags
        /// are set at the time of the window's creation.
        /// 
        /// The NOACTIVATE and TOOLWINDOW flags would not work using SetWindowLong.
        /// This is likely because SetWindowLong was setting the parameters too late
        /// to be applied to the window.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams baseParams = base.CreateParams;

                baseParams.ExStyle |= (int)(
                   WS_EX_LAYERED |
                   WS_EX_TRANSPARENT |
                   WS_EX_NOACTIVATE |
                   WS_EX_TOOLWINDOW);

                return baseParams;
            }
        }
    }
}
