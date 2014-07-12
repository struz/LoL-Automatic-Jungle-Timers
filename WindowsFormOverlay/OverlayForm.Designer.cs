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

namespace WinFormsTimerOverlay
{
    partial class OverlayForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panelDragon = new System.Windows.Forms.Panel();
            this.pictureDragon = new System.Windows.Forms.PictureBox();
            this.labelDragon = new System.Windows.Forms.Label();
            this.panelBaron = new System.Windows.Forms.Panel();
            this.pictureBaron = new System.Windows.Forms.PictureBox();
            this.labelBaron = new System.Windows.Forms.Label();
            this.panelBlueBlue = new System.Windows.Forms.Panel();
            this.pictureBlueBlue = new System.Windows.Forms.PictureBox();
            this.labelBlueBlue = new System.Windows.Forms.Label();
            this.panelBlueRed = new System.Windows.Forms.Panel();
            this.pictureBlueRed = new System.Windows.Forms.PictureBox();
            this.labelBlueRed = new System.Windows.Forms.Label();
            this.panelRedBlue = new System.Windows.Forms.Panel();
            this.pictureRedBlue = new System.Windows.Forms.PictureBox();
            this.labelRedBlue = new System.Windows.Forms.Label();
            this.panelRedRed = new System.Windows.Forms.Panel();
            this.pictureRedRed = new System.Windows.Forms.PictureBox();
            this.labelRedRed = new System.Windows.Forms.Label();
            this.panelDragon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDragon)).BeginInit();
            this.panelBaron.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBaron)).BeginInit();
            this.panelBlueBlue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBlueBlue)).BeginInit();
            this.panelBlueRed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBlueRed)).BeginInit();
            this.panelRedBlue.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureRedBlue)).BeginInit();
            this.panelRedRed.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureRedRed)).BeginInit();
            this.SuspendLayout();
            // 
            // panelDragon
            // 
            this.panelDragon.Controls.Add(this.pictureDragon);
            this.panelDragon.Controls.Add(this.labelDragon);
            this.panelDragon.Location = new System.Drawing.Point(12, 12);
            this.panelDragon.Name = "panelDragon";
            this.panelDragon.Size = new System.Drawing.Size(263, 51);
            this.panelDragon.TabIndex = 2;
            // 
            // pictureDragon
            // 
            this.pictureDragon.Image = global::WinFormsTimerOverlay.Properties.Resources._120px_DragonSquare;
            this.pictureDragon.Location = new System.Drawing.Point(2, 1);
            this.pictureDragon.Name = "pictureDragon";
            this.pictureDragon.Size = new System.Drawing.Size(48, 48);
            this.pictureDragon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureDragon.TabIndex = 3;
            this.pictureDragon.TabStop = false;
            // 
            // labelDragon
            // 
            this.labelDragon.AutoSize = true;
            this.labelDragon.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelDragon.ForeColor = System.Drawing.Color.Yellow;
            this.labelDragon.Location = new System.Drawing.Point(56, 14);
            this.labelDragon.Name = "labelDragon";
            this.labelDragon.Size = new System.Drawing.Size(95, 20);
            this.labelDragon.TabIndex = 2;
            this.labelDragon.Text = "labelDragon";
            // 
            // panelBaron
            // 
            this.panelBaron.Controls.Add(this.pictureBaron);
            this.panelBaron.Controls.Add(this.labelBaron);
            this.panelBaron.Location = new System.Drawing.Point(12, 69);
            this.panelBaron.Name = "panelBaron";
            this.panelBaron.Size = new System.Drawing.Size(263, 51);
            this.panelBaron.TabIndex = 3;
            // 
            // pictureBaron
            // 
            this.pictureBaron.BackColor = System.Drawing.Color.Black;
            this.pictureBaron.Image = global::WinFormsTimerOverlay.Properties.Resources._120px_Baron_Nashor;
            this.pictureBaron.Location = new System.Drawing.Point(2, 1);
            this.pictureBaron.Name = "pictureBaron";
            this.pictureBaron.Size = new System.Drawing.Size(48, 48);
            this.pictureBaron.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBaron.TabIndex = 3;
            this.pictureBaron.TabStop = false;
            // 
            // labelBaron
            // 
            this.labelBaron.AutoSize = true;
            this.labelBaron.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelBaron.ForeColor = System.Drawing.Color.Yellow;
            this.labelBaron.Location = new System.Drawing.Point(56, 14);
            this.labelBaron.Name = "labelBaron";
            this.labelBaron.Size = new System.Drawing.Size(85, 20);
            this.labelBaron.TabIndex = 2;
            this.labelBaron.Text = "labelBaron";
            // 
            // panelBlueBlue
            // 
            this.panelBlueBlue.Controls.Add(this.pictureBlueBlue);
            this.panelBlueBlue.Controls.Add(this.labelBlueBlue);
            this.panelBlueBlue.Location = new System.Drawing.Point(12, 126);
            this.panelBlueBlue.Name = "panelBlueBlue";
            this.panelBlueBlue.Size = new System.Drawing.Size(263, 51);
            this.panelBlueBlue.TabIndex = 4;
            // 
            // pictureBlueBlue
            // 
            this.pictureBlueBlue.Image = global::WinFormsTimerOverlay.Properties.Resources._120px_GolemSquare;
            this.pictureBlueBlue.Location = new System.Drawing.Point(2, 1);
            this.pictureBlueBlue.Name = "pictureBlueBlue";
            this.pictureBlueBlue.Size = new System.Drawing.Size(48, 48);
            this.pictureBlueBlue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBlueBlue.TabIndex = 3;
            this.pictureBlueBlue.TabStop = false;
            // 
            // labelBlueBlue
            // 
            this.labelBlueBlue.AutoSize = true;
            this.labelBlueBlue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelBlueBlue.ForeColor = System.Drawing.Color.Yellow;
            this.labelBlueBlue.Location = new System.Drawing.Point(56, 14);
            this.labelBlueBlue.Name = "labelBlueBlue";
            this.labelBlueBlue.Size = new System.Drawing.Size(106, 20);
            this.labelBlueBlue.TabIndex = 2;
            this.labelBlueBlue.Text = "labelBlueBlue";
            // 
            // panelBlueRed
            // 
            this.panelBlueRed.Controls.Add(this.pictureBlueRed);
            this.panelBlueRed.Controls.Add(this.labelBlueRed);
            this.panelBlueRed.Location = new System.Drawing.Point(12, 183);
            this.panelBlueRed.Name = "panelBlueRed";
            this.panelBlueRed.Size = new System.Drawing.Size(263, 51);
            this.panelBlueRed.TabIndex = 5;
            // 
            // pictureBlueRed
            // 
            this.pictureBlueRed.Image = global::WinFormsTimerOverlay.Properties.Resources._120px_LizardSquare;
            this.pictureBlueRed.Location = new System.Drawing.Point(2, 1);
            this.pictureBlueRed.Name = "pictureBlueRed";
            this.pictureBlueRed.Size = new System.Drawing.Size(48, 48);
            this.pictureBlueRed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBlueRed.TabIndex = 3;
            this.pictureBlueRed.TabStop = false;
            // 
            // labelBlueRed
            // 
            this.labelBlueRed.AutoSize = true;
            this.labelBlueRed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelBlueRed.ForeColor = System.Drawing.Color.Yellow;
            this.labelBlueRed.Location = new System.Drawing.Point(56, 14);
            this.labelBlueRed.Name = "labelBlueRed";
            this.labelBlueRed.Size = new System.Drawing.Size(104, 20);
            this.labelBlueRed.TabIndex = 2;
            this.labelBlueRed.Text = "labelBlueRed";
            // 
            // panelRedBlue
            // 
            this.panelRedBlue.Controls.Add(this.pictureRedBlue);
            this.panelRedBlue.Controls.Add(this.labelRedBlue);
            this.panelRedBlue.Location = new System.Drawing.Point(281, 69);
            this.panelRedBlue.Name = "panelRedBlue";
            this.panelRedBlue.Size = new System.Drawing.Size(263, 51);
            this.panelRedBlue.TabIndex = 5;
            // 
            // pictureRedBlue
            // 
            this.pictureRedBlue.Image = global::WinFormsTimerOverlay.Properties.Resources._120px_GolemSquare;
            this.pictureRedBlue.Location = new System.Drawing.Point(2, 1);
            this.pictureRedBlue.Name = "pictureRedBlue";
            this.pictureRedBlue.Size = new System.Drawing.Size(48, 48);
            this.pictureRedBlue.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureRedBlue.TabIndex = 3;
            this.pictureRedBlue.TabStop = false;
            // 
            // labelRedBlue
            // 
            this.labelRedBlue.AutoSize = true;
            this.labelRedBlue.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelRedBlue.ForeColor = System.Drawing.Color.Yellow;
            this.labelRedBlue.Location = new System.Drawing.Point(56, 14);
            this.labelRedBlue.Name = "labelRedBlue";
            this.labelRedBlue.Size = new System.Drawing.Size(104, 20);
            this.labelRedBlue.TabIndex = 2;
            this.labelRedBlue.Text = "labelRedBlue";
            // 
            // panelRedRed
            // 
            this.panelRedRed.Controls.Add(this.pictureRedRed);
            this.panelRedRed.Controls.Add(this.labelRedRed);
            this.panelRedRed.Location = new System.Drawing.Point(281, 12);
            this.panelRedRed.Name = "panelRedRed";
            this.panelRedRed.Size = new System.Drawing.Size(263, 51);
            this.panelRedRed.TabIndex = 6;
            // 
            // pictureRedRed
            // 
            this.pictureRedRed.Image = global::WinFormsTimerOverlay.Properties.Resources._120px_LizardSquare;
            this.pictureRedRed.Location = new System.Drawing.Point(2, 1);
            this.pictureRedRed.Name = "pictureRedRed";
            this.pictureRedRed.Size = new System.Drawing.Size(48, 48);
            this.pictureRedRed.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureRedRed.TabIndex = 3;
            this.pictureRedRed.TabStop = false;
            // 
            // labelRedRed
            // 
            this.labelRedRed.AutoSize = true;
            this.labelRedRed.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F);
            this.labelRedRed.ForeColor = System.Drawing.Color.Yellow;
            this.labelRedRed.Location = new System.Drawing.Point(56, 14);
            this.labelRedRed.Name = "labelRedRed";
            this.labelRedRed.Size = new System.Drawing.Size(102, 20);
            this.labelRedRed.TabIndex = 2;
            this.labelRedRed.Text = "labelRedRed";
            // 
            // OverlayForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(986, 597);
            this.Controls.Add(this.panelRedRed);
            this.Controls.Add(this.panelRedBlue);
            this.Controls.Add(this.panelBlueRed);
            this.Controls.Add(this.panelBlueBlue);
            this.Controls.Add(this.panelBaron);
            this.Controls.Add(this.panelDragon);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "OverlayForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Form1";
            this.TopMost = true;
            this.TransparencyKey = System.Drawing.Color.Black;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panelDragon.ResumeLayout(false);
            this.panelDragon.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureDragon)).EndInit();
            this.panelBaron.ResumeLayout(false);
            this.panelBaron.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBaron)).EndInit();
            this.panelBlueBlue.ResumeLayout(false);
            this.panelBlueBlue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBlueBlue)).EndInit();
            this.panelBlueRed.ResumeLayout(false);
            this.panelBlueRed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBlueRed)).EndInit();
            this.panelRedBlue.ResumeLayout(false);
            this.panelRedBlue.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureRedBlue)).EndInit();
            this.panelRedRed.ResumeLayout(false);
            this.panelRedRed.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureRedRed)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panelDragon;
        private System.Windows.Forms.PictureBox pictureDragon;
        private System.Windows.Forms.Label labelDragon;
        private System.Windows.Forms.Panel panelBaron;
        private System.Windows.Forms.PictureBox pictureBaron;
        private System.Windows.Forms.Label labelBaron;
        private System.Windows.Forms.Panel panelBlueBlue;
        private System.Windows.Forms.PictureBox pictureBlueBlue;
        private System.Windows.Forms.Label labelBlueBlue;
        private System.Windows.Forms.Panel panelBlueRed;
        private System.Windows.Forms.PictureBox pictureBlueRed;
        private System.Windows.Forms.Label labelBlueRed;
        private System.Windows.Forms.Panel panelRedBlue;
        private System.Windows.Forms.PictureBox pictureRedBlue;
        private System.Windows.Forms.Label labelRedBlue;
        private System.Windows.Forms.Panel panelRedRed;
        private System.Windows.Forms.PictureBox pictureRedRed;
        private System.Windows.Forms.Label labelRedRed;

    }
}

