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
            this._helpIcon = new System.Windows.Forms.PictureBox();
            this._localServiceControlLink = new System.Windows.Forms.LinkLabel();
            this.label6 = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this._deleteStudiesCheck = new System.Windows.Forms.CheckBox();
            this._deleteTimeValue = new System.Windows.Forms.NumericUpDown();
            this._deleteTimeUnits = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this._infoMessage = new System.Windows.Forms.Label();
            this._studyDeletion = new System.Windows.Forms.GroupBox();
            this._studyDeletionValidationPlaceholder = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this._maxDiskSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._upDownMaxDiskSpace)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._fileStoreWarningIcon)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._helpIcon)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._deleteTimeValue)).BeginInit();
            this._studyDeletion.SuspendLayout();
            this.SuspendLayout();
            // 
            // _maxDiskSpace
            // 
            this._maxDiskSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._maxDiskSpace, 2);
            this._maxDiskSpace.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._maxDiskSpace.LargeChange = 10000;
            this._maxDiskSpace.Location = new System.Drawing.Point(67, 107);
            this._maxDiskSpace.Margin = new System.Windows.Forms.Padding(1, 6, 1, 0);
            this._maxDiskSpace.Maximum = 100000;
            this._maxDiskSpace.Name = "_maxDiskSpace";
            this._maxDiskSpace.Size = new System.Drawing.Size(228, 30);
            this._maxDiskSpace.SmallChange = 10;
            this._maxDiskSpace.TabIndex = 3;
            this._maxDiskSpace.TickFrequency = 10000;
            this._maxDiskSpace.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            // 
            // _usedDiskSpace
            // 
            this._usedDiskSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._usedDiskSpace.Location = new System.Drawing.Point(299, 77);
            this._usedDiskSpace.Name = "_usedDiskSpace";
            this._usedDiskSpace.ReadOnly = true;
            this._usedDiskSpace.Size = new System.Drawing.Size(64, 20);
            this._usedDiskSpace.TabIndex = 9;
            // 
            // label4
            // 
            this.label4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label4.AutoSize = true;
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(369, 112);
            this.label4.Margin = new System.Windows.Forms.Padding(3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(14, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "%";
            // 
            // label5
            // 
            this.label5.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.label5.AutoSize = true;
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(369, 80);
            this.label5.Margin = new System.Windows.Forms.Padding(3);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(14, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "%";
            // 
            // label8
            // 
            this.label8.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.label8.AutoSize = true;
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(13, 8);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(51, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "File Store";
            this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _fileStoreDirectory
            // 
            this._fileStoreDirectory.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._fileStoreDirectory, 3);
            this._fileStoreDirectory.Location = new System.Drawing.Point(69, 5);
            this._fileStoreDirectory.Name = "_fileStoreDirectory";
            this._fileStoreDirectory.Size = new System.Drawing.Size(294, 20);
            this._fileStoreDirectory.TabIndex = 13;
            // 
            // _maxDiskSpaceDisplay
            // 
            this._maxDiskSpaceDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._maxDiskSpaceDisplay.Location = new System.Drawing.Point(389, 109);
            this._maxDiskSpaceDisplay.Name = "_maxDiskSpaceDisplay";
            this._maxDiskSpaceDisplay.ReadOnly = true;
            this._maxDiskSpaceDisplay.Size = new System.Drawing.Size(64, 20);
            this._maxDiskSpaceDisplay.TabIndex = 6;
            // 
            // _usedDiskSpaceDisplay
            // 
            this._usedDiskSpaceDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._usedDiskSpaceDisplay.Location = new System.Drawing.Point(389, 77);
            this._usedDiskSpaceDisplay.Name = "_usedDiskSpaceDisplay";
            this._usedDiskSpaceDisplay.ReadOnly = true;
            this._usedDiskSpaceDisplay.Size = new System.Drawing.Size(64, 20);
            this._usedDiskSpaceDisplay.TabIndex = 11;
            // 
            // _upDownMaxDiskSpace
            // 
            this._upDownMaxDiskSpace.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._upDownMaxDiskSpace.DecimalPlaces = 3;
            this._upDownMaxDiskSpace.Location = new System.Drawing.Point(299, 109);
            this._upDownMaxDiskSpace.Name = "_upDownMaxDiskSpace";
            this._upDownMaxDiskSpace.Size = new System.Drawing.Size(64, 20);
            this._upDownMaxDiskSpace.TabIndex = 4;
            // 
            // _changeFileStore
            // 
            this._changeFileStore.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this._changeFileStore.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._changeFileStore.Location = new System.Drawing.Point(389, 3);
            this._changeFileStore.Name = "_changeFileStore";
            this._changeFileStore.Size = new System.Drawing.Size(64, 23);
            this._changeFileStore.TabIndex = 12;
            this._changeFileStore.Text = "Browse";
            this._changeFileStore.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(3, 73);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 28);
            this.label1.TabIndex = 13;
            this.label1.Text = "Disk Usage";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label2
            // 
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(3, 101);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 36);
            this.label2.TabIndex = 14;
            this.label2.Text = "Max. Disk Usage";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // _usedSpaceMeter
            // 
            this._usedSpaceMeter.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.SetColumnSpan(this._usedSpaceMeter, 2);
            this._usedSpaceMeter.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold);
            this._usedSpaceMeter.ForeColor = System.Drawing.Color.WhiteSmoke;
            this._usedSpaceMeter.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._usedSpaceMeter.Location = new System.Drawing.Point(75, 77);
            this._usedSpaceMeter.Margin = new System.Windows.Forms.Padding(9, 3, 9, 3);
            this._usedSpaceMeter.Name = "_usedSpaceMeter";
            this._usedSpaceMeter.Size = new System.Drawing.Size(212, 20);
            this._usedSpaceMeter.TabIndex = 17;
            this._usedSpaceMeter.Value = 50;
            // 
            // _diskSpaceWarningMessage
            // 
            this._diskSpaceWarningMessage.AutoEllipsis = true;
            this._diskSpaceWarningMessage.AutoSize = true;
            this._diskSpaceWarningMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._diskSpaceWarningMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this._diskSpaceWarningMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._diskSpaceWarningMessage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._diskSpaceWarningMessage.Location = new System.Drawing.Point(69, 142);
            this._diskSpaceWarningMessage.Margin = new System.Windows.Forms.Padding(3, 5, 3, 0);
            this._diskSpaceWarningMessage.Name = "_diskSpaceWarningMessage";
            this._diskSpaceWarningMessage.Size = new System.Drawing.Size(192, 24);
            this._diskSpaceWarningMessage.TabIndex = 15;
            this._diskSpaceWarningMessage.Text = "don\'t translate me";
            // 
            // _diskSpaceWarningIcon
            // 
            this._diskSpaceWarningIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._diskSpaceWarningIcon.Image = ((System.Drawing.Image)(resources.GetObject("_diskSpaceWarningIcon.Image")));
            this._diskSpaceWarningIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._diskSpaceWarningIcon.Location = new System.Drawing.Point(47, 140);
            this._diskSpaceWarningIcon.Name = "_diskSpaceWarningIcon";
            this._diskSpaceWarningIcon.Size = new System.Drawing.Size(16, 16);
            this._diskSpaceWarningIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._diskSpaceWarningIcon.TabIndex = 16;
            this._diskSpaceWarningIcon.TabStop = false;
            // 
            // _totalDiskSpaceDisplay
            // 
            this._totalDiskSpaceDisplay.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._totalDiskSpaceDisplay.AutoSize = true;
            this.tableLayoutPanel1.SetColumnSpan(this._totalDiskSpaceDisplay, 2);
            this._totalDiskSpaceDisplay.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._totalDiskSpaceDisplay.Location = new System.Drawing.Point(198, 60);
            this._totalDiskSpaceDisplay.Margin = new System.Windows.Forms.Padding(0, 0, 9, 0);
            this._totalDiskSpaceDisplay.Name = "_totalDiskSpaceDisplay";
            this._totalDiskSpaceDisplay.Size = new System.Drawing.Size(89, 13);
            this._totalDiskSpaceDisplay.TabIndex = 18;
            this._totalDiskSpaceDisplay.Text = "Total Disk Space";
            this._totalDiskSpaceDisplay.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 7;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 66F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 32F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 70F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 5F));
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
            this.tableLayoutPanel1.Controls.Add(this._helpIcon, 5, 5);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 12);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 7;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 27F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 16F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 28F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 36F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(461, 173);
            this.tableLayoutPanel1.TabIndex = 19;
            // 
            // _fileStoreWarningIcon
            // 
            this._fileStoreWarningIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._fileStoreWarningIcon.Image = ((System.Drawing.Image)(resources.GetObject("_fileStoreWarningIcon.Image")));
            this._fileStoreWarningIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._fileStoreWarningIcon.Location = new System.Drawing.Point(47, 30);
            this._fileStoreWarningIcon.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this._fileStoreWarningIcon.Name = "_fileStoreWarningIcon";
            this._fileStoreWarningIcon.Size = new System.Drawing.Size(16, 16);
            this._fileStoreWarningIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this._fileStoreWarningIcon.TabIndex = 20;
            this._fileStoreWarningIcon.TabStop = false;
            // 
            // _fileStoreWarningMessage
            // 
            this._fileStoreWarningMessage.AutoEllipsis = true;
            this._fileStoreWarningMessage.Dock = System.Windows.Forms.DockStyle.Fill;
            this._fileStoreWarningMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this._fileStoreWarningMessage.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this._fileStoreWarningMessage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._fileStoreWarningMessage.Location = new System.Drawing.Point(69, 32);
            this._fileStoreWarningMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this._fileStoreWarningMessage.Name = "_fileStoreWarningMessage";
            this._fileStoreWarningMessage.Size = new System.Drawing.Size(192, 25);
            this._fileStoreWarningMessage.TabIndex = 21;
            this._fileStoreWarningMessage.Text = "don\'t translate me";
            // 
            // _helpIcon
            // 
            this._helpIcon.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._helpIcon.Image = ((System.Drawing.Image)(resources.GetObject("_helpIcon.Image")));
            this._helpIcon.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._helpIcon.Location = new System.Drawing.Point(429, 137);
            this._helpIcon.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this._helpIcon.Name = "_helpIcon";
            this._helpIcon.Size = new System.Drawing.Size(24, 24);
            this._helpIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this._helpIcon.TabIndex = 22;
            this._helpIcon.TabStop = false;
            // 
            // _localServiceControlLink
            // 
            this._localServiceControlLink.AutoSize = true;
            this._localServiceControlLink.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._localServiceControlLink.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this._localServiceControlLink.Location = new System.Drawing.Point(367, 273);
            this._localServiceControlLink.Name = "_localServiceControlLink";
            this._localServiceControlLink.Padding = new System.Windows.Forms.Padding(0, 1, 0, 0);
            this._localServiceControlLink.Size = new System.Drawing.Size(97, 14);
            this._localServiceControlLink.TabIndex = 19;
            this._localServiceControlLink.TabStop = true;
            this._localServiceControlLink.Text = "Stop Local Service";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label6, 4);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(3, 27);
            this.label6.Name = "label6";
            this.label6.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.label6.Size = new System.Drawing.Size(264, 18);
            this.label6.TabIndex = 24;
            this.label6.Text = "Note: changes made will be applied to existing studies.";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 7;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 26F));
            this.tableLayoutPanel2.Controls.Add(this._deleteStudiesCheck, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this._deleteTimeValue, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this._deleteTimeUnits, 2, 0);
            this.tableLayoutPanel2.Controls.Add(this.label3, 3, 0);
            this.tableLayoutPanel2.Controls.Add(this.label6, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this._studyDeletionValidationPlaceholder, 5, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(6, 19);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 3;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(449, 49);
            this.tableLayoutPanel2.TabIndex = 25;
            // 
            // _deleteStudiesCheck
            // 
            this._deleteStudiesCheck.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._deleteStudiesCheck.AutoSize = true;
            this._deleteStudiesCheck.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._deleteStudiesCheck.Location = new System.Drawing.Point(3, 5);
            this._deleteStudiesCheck.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this._deleteStudiesCheck.Name = "_deleteStudiesCheck";
            this._deleteStudiesCheck.Size = new System.Drawing.Size(93, 17);
            this._deleteStudiesCheck.TabIndex = 24;
            this._deleteStudiesCheck.Text = "Delete studies";
            this._deleteStudiesCheck.UseVisualStyleBackColor = true;
            // 
            // _deleteTimeValue
            // 
            this._deleteTimeValue.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._deleteTimeValue.Location = new System.Drawing.Point(99, 4);
            this._deleteTimeValue.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this._deleteTimeValue.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this._deleteTimeValue.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this._deleteTimeValue.Name = "_deleteTimeValue";
            this._deleteTimeValue.Size = new System.Drawing.Size(57, 20);
            this._deleteTimeValue.TabIndex = 25;
            this._deleteTimeValue.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this._deleteTimeValue.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // _deleteTimeUnits
            // 
            this._deleteTimeUnits.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this._deleteTimeUnits.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._deleteTimeUnits.FormattingEnabled = true;
            this._deleteTimeUnits.Location = new System.Drawing.Point(162, 3);
            this._deleteTimeUnits.Name = "_deleteTimeUnits";
            this._deleteTimeUnits.Size = new System.Drawing.Size(77, 21);
            this._deleteTimeUnits.TabIndex = 26;
            // 
            // label3
            // 
            this.label3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.label3.AutoSize = true;
            this.tableLayoutPanel2.SetColumnSpan(this.label3, 2);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(245, 7);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 27;
            this.label3.Text = "after receipt/import";
            // 
            // _infoMessage
            // 
            this._infoMessage.AutoEllipsis = true;
            this._infoMessage.AutoSize = true;
            this._infoMessage.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this._infoMessage.ForeColor = System.Drawing.Color.CornflowerBlue;
            this._infoMessage.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this._infoMessage.Location = new System.Drawing.Point(9, 274);
            this._infoMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 0);
            this._infoMessage.Name = "_infoMessage";
            this._infoMessage.Size = new System.Drawing.Size(90, 13);
            this._infoMessage.TabIndex = 22;
            this._infoMessage.Text = "don\'t translate me";
            // 
            // _studyDeletion
            // 
            this._studyDeletion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this._studyDeletion.Controls.Add(this.tableLayoutPanel2);
            this._studyDeletion.Location = new System.Drawing.Point(3, 191);
            this._studyDeletion.Name = "_studyDeletion";
            this._studyDeletion.Size = new System.Drawing.Size(461, 78);
            this._studyDeletion.TabIndex = 0;
            this._studyDeletion.TabStop = false;
            this._studyDeletion.Text = "Automatic Study Deletion";
            // 
            // _studyDeletionValidationPlaceholder
            // 
            this._studyDeletionValidationPlaceholder.Dock = System.Windows.Forms.DockStyle.Fill;
            this._studyDeletionValidationPlaceholder.Location = new System.Drawing.Point(347, 0);
            this._studyDeletionValidationPlaceholder.Name = "_studyDeletionValidationPlaceholder";
            this.tableLayoutPanel2.SetRowSpan(this._studyDeletionValidationPlaceholder, 2);
            this._studyDeletionValidationPlaceholder.Size = new System.Drawing.Size(73, 45);
            this._studyDeletionValidationPlaceholder.TabIndex = 28;
            this._studyDeletionValidationPlaceholder.Text = " don\'t translate";
            this._studyDeletionValidationPlaceholder.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // StorageConfigurationComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this._infoMessage);
            this.Controls.Add(this._studyDeletion);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this._localServiceControlLink);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "StorageConfigurationComponentControl";
            this.Size = new System.Drawing.Size(467, 298);
            ((System.ComponentModel.ISupportInitialize)(this._maxDiskSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._upDownMaxDiskSpace)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._diskSpaceWarningIcon)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._fileStoreWarningIcon)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._helpIcon)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this._deleteTimeValue)).EndInit();
            this._studyDeletion.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.LinkLabel _localServiceControlLink;
        private System.Windows.Forms.PictureBox _fileStoreWarningIcon;
        private System.Windows.Forms.Label _fileStoreWarningMessage;
        private System.Windows.Forms.PictureBox _helpIcon;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.CheckBox _deleteStudiesCheck;
        private System.Windows.Forms.NumericUpDown _deleteTimeValue;
        private System.Windows.Forms.ComboBox _deleteTimeUnits;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label _infoMessage;
        private System.Windows.Forms.GroupBox _studyDeletion;
        private System.Windows.Forms.Label _studyDeletionValidationPlaceholder;
    }
}
