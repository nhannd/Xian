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

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    partial class SendQueueApplicationComponentControl
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
			DoDispose(disposing);
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
			this.components = new System.ComponentModel.Container();
			this._sendTable = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._toolTip = new System.Windows.Forms.ToolTip(this.components);
			this._statusBar = new System.Windows.Forms.StatusStrip();
			this._fillerStatusItem = new System.Windows.Forms.ToolStripStatusLabel();
			this._failuresStatusItem = new System.Windows.Forms.ToolStripStatusLabel();
			this._statusBar.SuspendLayout();
			this.SuspendLayout();
			// 
			// _sendTable
			// 
			this._sendTable.ColumnHeaderTooltip = null;
			this._sendTable.Dock = System.Windows.Forms.DockStyle.Fill;
			this._sendTable.Location = new System.Drawing.Point(0, 31);
			this._sendTable.Name = "_sendTable";
			this._sendTable.ReadOnly = false;
			this._sendTable.Size = new System.Drawing.Size(687, 389);
			this._sendTable.SortButtonTooltip = null;
			this._sendTable.TabIndex = 1;
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(687, 31);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this._titleBar.TabIndex = 0;
			this._titleBar.Text = "Send Operations";
			// 
			// _toolTip
			// 
			this._toolTip.IsBalloon = true;
			// 
			// _statusBar
			// 
			this._statusBar.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._fillerStatusItem,
            this._failuresStatusItem});
			this._statusBar.Location = new System.Drawing.Point(0, 398);
			this._statusBar.Name = "_statusBar";
			this._statusBar.Size = new System.Drawing.Size(687, 22);
			this._statusBar.SizingGrip = false;
			this._statusBar.TabIndex = 2;
			// 
			// _fillerStatusItem
			// 
			this._fillerStatusItem.AutoSize = false;
			this._fillerStatusItem.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this._fillerStatusItem.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this._fillerStatusItem.Name = "_fillerStatusItem";
			this._fillerStatusItem.Size = new System.Drawing.Size(637, 17);
			this._fillerStatusItem.Spring = true;
			// 
			// _failuresStatusItem
			// 
			this._failuresStatusItem.BorderSides = ((System.Windows.Forms.ToolStripStatusLabelBorderSides)((((System.Windows.Forms.ToolStripStatusLabelBorderSides.Left | System.Windows.Forms.ToolStripStatusLabelBorderSides.Top)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Right)
						| System.Windows.Forms.ToolStripStatusLabelBorderSides.Bottom)));
			this._failuresStatusItem.BorderStyle = System.Windows.Forms.Border3DStyle.SunkenOuter;
			this._failuresStatusItem.Name = "_failuresStatusItem";
			this._failuresStatusItem.Size = new System.Drawing.Size(4, 17);
			// 
			// SendQueueApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._statusBar);
			this.Controls.Add(this._sendTable);
			this.Controls.Add(this._titleBar);
			this.Name = "SendQueueApplicationComponentControl";
			this.Size = new System.Drawing.Size(687, 420);
			this._statusBar.ResumeLayout(false);
			this._statusBar.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _sendTable;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
		private System.Windows.Forms.ToolTip _toolTip;
		private System.Windows.Forms.StatusStrip _statusBar;
		private System.Windows.Forms.ToolStripStatusLabel _fillerStatusItem;
		private System.Windows.Forms.ToolStripStatusLabel _failuresStatusItem;

	}
}
