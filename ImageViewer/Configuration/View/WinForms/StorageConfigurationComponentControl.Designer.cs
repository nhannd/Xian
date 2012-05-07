#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    partial class StorageConfigurationComponentControl
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
            if (disposing)
            {
                if (components != null)
                    components.Dispose();
            }

            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StorageConfigurationComponentControl));
            this._maxDiskSpace = new System.Windows.Forms.TrackBar();
            this._usedDiskSpace = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this._fileStoreDirectory = new System.Windows.Forms.TextBox();
            this._maxDiskSpaceDisplay = new System.Windows.Forms.TextBox();
            this._usedDiskSpaceDisplay = new System.Windows.Forms.TextBox();
            this._upDownMaxDiskSpace = new System.Windows.Forms.NumericUpDown();
            this._changeFileStore = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this._usedSpaceMeter = new ClearCanvas.Desktop.View.WinForms.Meter();
            this._diskSpaceWarningMessage = new System.Windows.Forms.Label();
            this._tooltip = new System.Windows.Forms.ToolTip(this.components);
            this._diskSpaceWarningIcon = new System.Windows.Forms.PictureBox();
            this._totalDiskSpaceDisplay = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this._fileStoreWarningIcon = new System.Windows.Forms.PictureBox();
            this._fileStoreWarningMessage = new System.Windows.Forms.Label();
            this._stopLocalService = new System.Windows.Forms.LinkLabel();
            this._helpIcon = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this._maxDiskSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._upDownMaxDiskSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._fileStoreWarningIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._helpIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // _maxDiskSpace
            // 
            resources.ApplyResources(this._maxDiskSpace, "_maxDiskSpace");
            this.tableLayoutPanel1.SetColumnSpan(this._maxDiskSpace, 2);
            this._maxDiskSpace.LargeChange = 10000;
            this._maxDiskSpace.Maximum = 100000;
            this._maxDiskSpace.Name = "_maxDiskSpace";
            this._maxDiskSpace.SmallChange = 10;
            this._maxDiskSpace.TickFrequency = 10000;
            this._maxDiskSpace.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            // 
            // _usedDiskSpace
            // 
            resources.ApplyResources(this._usedDiskSpace, "_usedDiskSpace");
            this._usedDiskSpace.Name = "_usedDiskSpace";
            this._usedDiskSpace.ReadOnly = true;
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // _fileStoreDirectory
            // 
            resources.ApplyResources(this._fileStoreDirectory, "_fileStoreDirectory");
            this.tableLayoutPanel1.SetColumnSpan(this._fileStoreDirectory, 3);
            this._fileStoreDirectory.Name = "_fileStoreDirectory";
            // 
            // _maxDiskSpaceDisplay
            // 
            resources.ApplyResources(this._maxDiskSpaceDisplay, "_maxDiskSpaceDisplay");
            this._maxDiskSpaceDisplay.Name = "_maxDiskSpaceDisplay";
            this._maxDiskSpaceDisplay.ReadOnly = true;
            // 
            // _usedDiskSpaceDisplay
            // 
            resources.ApplyResources(this._usedDiskSpaceDisplay, "_usedDiskSpaceDisplay");
            this._usedDiskSpaceDisplay.Name = "_usedDiskSpaceDisplay";
            this._usedDiskSpaceDisplay.ReadOnly = true;
            // 
            // _upDownMaxDiskSpace
            // 
            resources.ApplyResources(this._upDownMaxDiskSpace, "_upDownMaxDiskSpace");
            this._upDownMaxDiskSpace.DecimalPlaces = 3;
            this._upDownMaxDiskSpace.Name = "_upDownMaxDiskSpace";
            // 
            // _changeFileStore
            // 
            resources.ApplyResources(this._changeFileStore, "_changeFileStore");
            this._changeFileStore.Name = "_changeFileStore";
            this._changeFileStore.UseVisualStyleBackColor = true;
            this._changeFileStore.Click += new System.EventHandler(this._changeFileStore_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // _usedSpaceMeter
            // 
            resources.ApplyResources(this._usedSpaceMeter, "_usedSpaceMeter");
            this.tableLayoutPanel1.SetColumnSpan(this._usedSpaceMeter, 2);
            this._usedSpaceMeter.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._usedSpaceMeter.Name = "_usedSpaceMeter";
            this._usedSpaceMeter.Value = 50;
            // 
            // _diskSpaceWarningMessage
            // 
            this._diskSpaceWarningMessage.AutoEllipsis = true;
            resources.ApplyResources(this._diskSpaceWarningMessage, "_diskSpaceWarningMessage");
            this._diskSpaceWarningMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._diskSpaceWarningMessage.Name = "_diskSpaceWarningMessage";
            // 
            // _diskSpaceWarningIcon
            // 
            resources.ApplyResources(this._diskSpaceWarningIcon, "_diskSpaceWarningIcon");
            this._diskSpaceWarningIcon.Name = "_diskSpaceWarningIcon";
            this._diskSpaceWarningIcon.TabStop = false;
            // 
            // _totalDiskSpaceDisplay
            // 
            resources.ApplyResources(this._totalDiskSpaceDisplay, "_totalDiskSpaceDisplay");
            this.tableLayoutPanel1.SetColumnSpan(this._totalDiskSpaceDisplay, 2);
            this._totalDiskSpaceDisplay.Name = "_totalDiskSpaceDisplay";
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this._changeFileStore, 5, 0);
            this.tableLayoutPanel1.Controls.Add(this._fileStoreDirectory, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this._usedDiskSpaceDisplay, 5, 3);
            this.tableLayoutPanel1.Controls.Add(this._upDownMaxDiskSpace, 3, 4);
            this.tableLayoutPanel1.Controls.Add(this._maxDiskSpaceDisplay, 5, 4);
            this.tableLayoutPanel1.Controls.Add(this._diskSpaceWarningIcon, 0, 5);
            this.tableLayoutPanel1.Controls.Add(this._totalDiskSpaceDisplay, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.label4, 4, 4);
            this.tableLayoutPanel1.Controls.Add(this.label5, 4, 3);
            this.tableLayoutPanel1.Controls.Add(this.label1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.label2, 0, 4);
            this.tableLayoutPanel1.Controls.Add(this._maxDiskSpace, 1, 4);
            this.tableLayoutPanel1.Controls.Add(this._usedDiskSpace, 3, 3);
            this.tableLayoutPanel1.Controls.Add(this.label8, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._usedSpaceMeter, 1, 3);
            this.tableLayoutPanel1.Controls.Add(this._diskSpaceWarningMessage, 1, 5);
            this.tableLayoutPanel1.Controls.Add(this._fileStoreWarningIcon, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._fileStoreWarningMessage, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this._stopLocalService, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this._helpIcon, 5, 6);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // _fileStoreWarningIcon
            // 
            resources.ApplyResources(this._fileStoreWarningIcon, "_fileStoreWarningIcon");
            this._fileStoreWarningIcon.Name = "_fileStoreWarningIcon";
            this._fileStoreWarningIcon.TabStop = false;
            // 
            // _fileStoreWarningMessage
            // 
            this._fileStoreWarningMessage.AutoEllipsis = true;
            resources.ApplyResources(this._fileStoreWarningMessage, "_fileStoreWarningMessage");
            this._fileStoreWarningMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._fileStoreWarningMessage.Name = "_fileStoreWarningMessage";
            // 
            // _stopLocalService
            // 
            resources.ApplyResources(this._stopLocalService, "_stopLocalService");
            this.tableLayoutPanel1.SetColumnSpan(this._stopLocalService, 2);
            this._stopLocalService.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this._stopLocalService.Name = "_stopLocalService";
            this._stopLocalService.TabStop = true;
            this._stopLocalService.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this._stopLocalServices_LinkClicked);
            // 
            // _helpIcon
            // 
            resources.ApplyResources(this._helpIcon, "_helpIcon");
            this._helpIcon.Name = "_helpIcon";
            this._helpIcon.TabStop = false;
            this._helpIcon.Click += new System.EventHandler(this._helpIcon_Click);
            // 
            // StorageConfigurationComponentControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "StorageConfigurationComponentControl";
            ((System.ComponentModel.ISupportInitialize)(this._maxDiskSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._upDownMaxDiskSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._fileStoreWarningIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._helpIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TrackBar _maxDiskSpace;
        private System.Windows.Forms.TextBox _usedDiskSpace;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox _fileStoreDirectory;
		private System.Windows.Forms.TextBox _usedDiskSpaceDisplay;
        private System.Windows.Forms.TextBox _maxDiskSpaceDisplay;
        private System.Windows.Forms.NumericUpDown _upDownMaxDiskSpace;
        private System.Windows.Forms.Button _changeFileStore;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private Desktop.View.WinForms.Meter _usedSpaceMeter;
        private System.Windows.Forms.Label _diskSpaceWarningMessage;
        private System.Windows.Forms.ToolTip _tooltip;
        private System.Windows.Forms.PictureBox _diskSpaceWarningIcon;
        private System.Windows.Forms.Label _totalDiskSpaceDisplay;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.LinkLabel _stopLocalService;
        private System.Windows.Forms.PictureBox _fileStoreWarningIcon;
        private System.Windows.Forms.Label _fileStoreWarningMessage;
        private System.Windows.Forms.PictureBox _helpIcon;
    }
}
