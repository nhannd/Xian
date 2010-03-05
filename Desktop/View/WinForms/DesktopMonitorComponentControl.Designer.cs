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

namespace ClearCanvas.Desktop.View.WinForms
{
    partial class DesktopMonitorComponentControl
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

#region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
			this._windows = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._openWindow = new System.Windows.Forms.Button();
			this._closeWindow = new System.Windows.Forms.Button();
			this._activateWindow = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this._closeShelf = new System.Windows.Forms.Button();
			this._hideShelf = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this._activateShelf = new System.Windows.Forms.Button();
			this._showShelf = new System.Windows.Forms.Button();
			this._openShelf = new System.Windows.Forms.Button();
			this._shelves = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label2 = new System.Windows.Forms.Label();
			this._activateWorkspace = new System.Windows.Forms.Button();
			this._closeWorkspace = new System.Windows.Forms.Button();
			this._openWorkspace = new System.Windows.Forms.Button();
			this._workspaces = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this._events = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _windows
			// 
			this._windows.Location = new System.Drawing.Point(48, 40);
			this._windows.Margin = new System.Windows.Forms.Padding(4);
			this._windows.MultiSelect = false;
			this._windows.Name = "_windows";
			this._windows.ReadOnly = false;
			this._windows.ShowToolbar = false;
			this._windows.Size = new System.Drawing.Size(662, 106);
			this._windows.TabIndex = 0;
			// 
			// _openWindow
			// 
			this._openWindow.Location = new System.Drawing.Point(728, 50);
			this._openWindow.Name = "_openWindow";
			this._openWindow.Size = new System.Drawing.Size(75, 23);
			this._openWindow.TabIndex = 1;
			this._openWindow.Text = "Open";
			this._openWindow.UseVisualStyleBackColor = true;
			this._openWindow.Click += new System.EventHandler(this._openWindow_Click);
			// 
			// _closeWindow
			// 
			this._closeWindow.Location = new System.Drawing.Point(728, 108);
			this._closeWindow.Name = "_closeWindow";
			this._closeWindow.Size = new System.Drawing.Size(75, 23);
			this._closeWindow.TabIndex = 3;
			this._closeWindow.Text = "Close";
			this._closeWindow.UseVisualStyleBackColor = true;
			this._closeWindow.Click += new System.EventHandler(this._closeWindow_Click);
			// 
			// _activateWindow
			// 
			this._activateWindow.Location = new System.Drawing.Point(728, 79);
			this._activateWindow.Name = "_activateWindow";
			this._activateWindow.Size = new System.Drawing.Size(75, 23);
			this._activateWindow.TabIndex = 2;
			this._activateWindow.Text = "Activate";
			this._activateWindow.UseVisualStyleBackColor = true;
			this._activateWindow.Click += new System.EventHandler(this._activateWindow_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this._closeShelf);
			this.groupBox2.Controls.Add(this._hideShelf);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this._activateShelf);
			this.groupBox2.Controls.Add(this._showShelf);
			this.groupBox2.Controls.Add(this._openShelf);
			this.groupBox2.Controls.Add(this._shelves);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this._activateWorkspace);
			this.groupBox2.Controls.Add(this._closeWorkspace);
			this.groupBox2.Controls.Add(this._openWorkspace);
			this.groupBox2.Controls.Add(this._workspaces);
			this.groupBox2.Location = new System.Drawing.Point(48, 163);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(781, 335);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Window Contents";
			// 
			// _closeShelf
			// 
			this._closeShelf.Location = new System.Drawing.Point(687, 300);
			this._closeShelf.Name = "_closeShelf";
			this._closeShelf.Size = new System.Drawing.Size(75, 23);
			this._closeShelf.TabIndex = 9;
			this._closeShelf.Text = "Close";
			this._closeShelf.UseVisualStyleBackColor = true;
			this._closeShelf.Click += new System.EventHandler(this._closeShelf_Click);
			// 
			// _hideShelf
			// 
			this._hideShelf.Location = new System.Drawing.Point(687, 271);
			this._hideShelf.Name = "_hideShelf";
			this._hideShelf.Size = new System.Drawing.Size(75, 23);
			this._hideShelf.TabIndex = 8;
			this._hideShelf.Text = "Hide";
			this._hideShelf.UseVisualStyleBackColor = true;
			this._hideShelf.Click += new System.EventHandler(this._hideShelf_Click);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 165);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(45, 13);
			this.label3.TabIndex = 10;
			this.label3.Text = "Shelves";
			// 
			// _activateShelf
			// 
			this._activateShelf.Location = new System.Drawing.Point(687, 213);
			this._activateShelf.Name = "_activateShelf";
			this._activateShelf.Size = new System.Drawing.Size(75, 23);
			this._activateShelf.TabIndex = 6;
			this._activateShelf.Text = "Activate";
			this._activateShelf.UseVisualStyleBackColor = true;
			this._activateShelf.Click += new System.EventHandler(this._activateShelf_Click);
			// 
			// _showShelf
			// 
			this._showShelf.Location = new System.Drawing.Point(687, 242);
			this._showShelf.Name = "_showShelf";
			this._showShelf.Size = new System.Drawing.Size(75, 23);
			this._showShelf.TabIndex = 7;
			this._showShelf.Text = "Show";
			this._showShelf.UseVisualStyleBackColor = true;
			this._showShelf.Click += new System.EventHandler(this._showShelf_Click);
			// 
			// _openShelf
			// 
			this._openShelf.Location = new System.Drawing.Point(687, 184);
			this._openShelf.Name = "_openShelf";
			this._openShelf.Size = new System.Drawing.Size(75, 23);
			this._openShelf.TabIndex = 5;
			this._openShelf.Text = "Open";
			this._openShelf.UseVisualStyleBackColor = true;
			this._openShelf.Click += new System.EventHandler(this._openShelf_Click);
			// 
			// _shelves
			// 
			this._shelves.Location = new System.Drawing.Point(7, 186);
			this._shelves.Margin = new System.Windows.Forms.Padding(4);
			this._shelves.MultiSelect = false;
			this._shelves.Name = "_shelves";
			this._shelves.ReadOnly = false;
			this._shelves.ShowToolbar = false;
			this._shelves.Size = new System.Drawing.Size(662, 137);
			this._shelves.TabIndex = 4;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 23);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Workspaces";
			// 
			// _activateWorkspace
			// 
			this._activateWorkspace.Location = new System.Drawing.Point(687, 73);
			this._activateWorkspace.Name = "_activateWorkspace";
			this._activateWorkspace.Size = new System.Drawing.Size(75, 23);
			this._activateWorkspace.TabIndex = 2;
			this._activateWorkspace.Text = "Activate";
			this._activateWorkspace.UseVisualStyleBackColor = true;
			this._activateWorkspace.Click += new System.EventHandler(this._activateWorkspace_Click);
			// 
			// _closeWorkspace
			// 
			this._closeWorkspace.Location = new System.Drawing.Point(687, 102);
			this._closeWorkspace.Name = "_closeWorkspace";
			this._closeWorkspace.Size = new System.Drawing.Size(75, 23);
			this._closeWorkspace.TabIndex = 3;
			this._closeWorkspace.Text = "Close";
			this._closeWorkspace.UseVisualStyleBackColor = true;
			this._closeWorkspace.Click += new System.EventHandler(this._closeWorkspace_Click);
			// 
			// _openWorkspace
			// 
			this._openWorkspace.Location = new System.Drawing.Point(687, 44);
			this._openWorkspace.Name = "_openWorkspace";
			this._openWorkspace.Size = new System.Drawing.Size(75, 23);
			this._openWorkspace.TabIndex = 1;
			this._openWorkspace.Text = "Open";
			this._openWorkspace.UseVisualStyleBackColor = true;
			this._openWorkspace.Click += new System.EventHandler(this._openWorkspace_Click);
			// 
			// _workspaces
			// 
			this._workspaces.Location = new System.Drawing.Point(7, 44);
			this._workspaces.Margin = new System.Windows.Forms.Padding(4);
			this._workspaces.MultiSelect = false;
			this._workspaces.Name = "_workspaces";
			this._workspaces.ReadOnly = false;
			this._workspaces.ShowToolbar = false;
			this._workspaces.Size = new System.Drawing.Size(662, 105);
			this._workspaces.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(45, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(51, 13);
			this.label1.TabIndex = 4;
			this.label1.Text = "Windows";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this._events);
			this.groupBox1.Location = new System.Drawing.Point(48, 524);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(781, 211);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Events";
			// 
			// _events
			// 
			this._events.Location = new System.Drawing.Point(9, 22);
			this._events.Margin = new System.Windows.Forms.Padding(4);
			this._events.MultiSelect = false;
			this._events.Name = "_events";
			this._events.ReadOnly = false;
			this._events.ShowToolbar = false;
			this._events.Size = new System.Drawing.Size(753, 182);
			this._events.TabIndex = 0;
			// 
			// DesktopMonitorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._activateWindow);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this._closeWindow);
			this.Controls.Add(this._openWindow);
			this.Controls.Add(this._windows);
			this.Name = "DesktopMonitorComponentControl";
			this.Size = new System.Drawing.Size(856, 758);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

		#endregion

        private TableView _windows;
        private System.Windows.Forms.Button _activateWindow;
        private System.Windows.Forms.Button _closeWindow;
        private System.Windows.Forms.Button _openWindow;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button _activateWorkspace;
        private System.Windows.Forms.Button _closeWorkspace;
        private System.Windows.Forms.Button _openWorkspace;
        private TableView _workspaces;
        private System.Windows.Forms.Button _closeShelf;
        private System.Windows.Forms.Button _hideShelf;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button _activateShelf;
        private System.Windows.Forms.Button _showShelf;
        private System.Windows.Forms.Button _openShelf;
        private TableView _shelves;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private TableView _events;
    }
}
