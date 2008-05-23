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
    partial class StackTabComponentContainerControl
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
			this.components = new System.ComponentModel.Container();
			this._stackTabControl = new Crownwood.DotNetMagic.Controls.TabbedGroups();
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this._toolStrip = new System.Windows.Forms.ToolStrip();
			this.panel1 = new System.Windows.Forms.Panel();
			((System.ComponentModel.ISupportInitialize)(this._stackTabControl)).BeginInit();
			this._statusStrip.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _stackTabControl
			// 
			this._stackTabControl.AllowDrop = true;
			this._stackTabControl.AtLeastOneLeaf = false;
			this._stackTabControl.ContextMenuStrip = this._contextMenu;
			this._stackTabControl.DisplayTabMode = Crownwood.DotNetMagic.Controls.DisplayTabModes.HideAll;
			this._stackTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._stackTabControl.Location = new System.Drawing.Point(0, 0);
			this._stackTabControl.Name = "_stackTabControl";
			this._stackTabControl.ProminentLeaf = null;
			this._stackTabControl.ResizeBarColor = System.Drawing.SystemColors.Control;
			this._stackTabControl.Size = new System.Drawing.Size(222, 233);
			this._stackTabControl.TabIndex = 0;
			// 
			// _contextMenu
			// 
			this._contextMenu.Name = "_contextMenu";
			this._contextMenu.Size = new System.Drawing.Size(61, 4);
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel,
            this.toolStripStatusLabel1});
			this._statusStrip.Location = new System.Drawing.Point(0, 236);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.Size = new System.Drawing.Size(222, 22);
			this._statusStrip.TabIndex = 2;
			this._statusStrip.Text = "statusStrip1";
			this._statusStrip.Visible = false;
			// 
			// _statusLabel
			// 
			this._statusLabel.Name = "_statusLabel";
			this._statusLabel.Size = new System.Drawing.Size(0, 17);
			this._statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(207, 17);
			this.toolStripStatusLabel1.Spring = true;
			// 
			// _toolStrip
			// 
			this._toolStrip.GripMargin = new System.Windows.Forms.Padding(0);
			this._toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
			this._toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolStrip.Location = new System.Drawing.Point(0, 0);
			this._toolStrip.Name = "_toolStrip";
			this._toolStrip.ShowItemToolTips = false;
			this._toolStrip.Size = new System.Drawing.Size(222, 25);
			this._toolStrip.TabIndex = 4;
			this._toolStrip.Text = "toolStrip1";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this._stackTabControl);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 25);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(222, 233);
			this.panel1.TabIndex = 5;
			// 
			// StackTabComponentContainerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(this._toolStrip);
			this.Controls.Add(this._statusStrip);
			this.Name = "StackTabComponentContainerControl";
			this.Size = new System.Drawing.Size(222, 258);
			this.Load += new System.EventHandler(this.StackTabComponentContainerControl_Load);
			((System.ComponentModel.ISupportInitialize)(this._stackTabControl)).EndInit();
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private Crownwood.DotNetMagic.Controls.TabbedGroups _stackTabControl;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.ToolStrip _toolStrip;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
		private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
		private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
		private System.Windows.Forms.Panel panel1;
    }
}
