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
    partial class ExceptionDialogControl
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
			this._flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this._detailButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._quitButton = new System.Windows.Forms.Button();
			this._description = new System.Windows.Forms.TextBox();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._warningIcon = new System.Windows.Forms.PictureBox();
			this._detailTree = new System.Windows.Forms.TreeView();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._flowLayoutPanel.SuspendLayout();
			this._tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this._warningIcon)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _flowLayoutPanel
			// 
			this._flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._flowLayoutPanel.AutoSize = true;
			this._flowLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayoutPanel.SetColumnSpan(this._flowLayoutPanel, 2);
			this._flowLayoutPanel.Controls.Add(this._detailButton);
			this._flowLayoutPanel.Controls.Add(this._okButton);
			this._flowLayoutPanel.Controls.Add(this._quitButton);
			this._flowLayoutPanel.Location = new System.Drawing.Point(3, 114);
			this._flowLayoutPanel.MinimumSize = new System.Drawing.Size(432, 32);
			this._flowLayoutPanel.Name = "_flowLayoutPanel";
			this._flowLayoutPanel.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this._flowLayoutPanel.Size = new System.Drawing.Size(434, 32);
			this._flowLayoutPanel.TabIndex = 0;
			// 
			// _detailButton
			// 
			this._detailButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._detailButton.AutoSize = true;
			this._detailButton.Location = new System.Drawing.Point(356, 3);
			this._detailButton.Name = "_detailButton";
			this._detailButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._detailButton.Size = new System.Drawing.Size(75, 23);
			this._detailButton.TabIndex = 1;
			this._detailButton.Text = "&Details >>";
			this._detailButton.UseVisualStyleBackColor = true;
			this._detailButton.Click += new System.EventHandler(this._detailButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.AutoSize = true;
			this._okButton.Location = new System.Drawing.Point(275, 3);
			this._okButton.Name = "_okButton";
			this._okButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 2;
			this._okButton.Text = "&OK";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// _quitButton
			// 
			this._quitButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._quitButton.AutoSize = true;
			this._quitButton.Location = new System.Drawing.Point(194, 3);
			this._quitButton.Name = "_quitButton";
			this._quitButton.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this._quitButton.Size = new System.Drawing.Size(75, 23);
			this._quitButton.TabIndex = 0;
			this._quitButton.Text = "&Quit";
			this._quitButton.UseVisualStyleBackColor = true;
			// 
			// _description
			// 
			this._description.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._description.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this._description.Location = new System.Drawing.Point(43, 3);
			this._description.MinimumSize = new System.Drawing.Size(394, 105);
			this._description.Multiline = true;
			this._description.Name = "_description";
			this._description.ReadOnly = true;
			this._description.Size = new System.Drawing.Size(394, 105);
			this._description.TabIndex = 2;
			this._description.Text = "Add description here";
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.AutoSize = true;
			this._tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this._tableLayoutPanel.ColumnCount = 2;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 40F));
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._flowLayoutPanel, 0, 1);
			this._tableLayoutPanel.Controls.Add(this._warningIcon, 0, 0);
			this._tableLayoutPanel.Controls.Add(this._description, 1, 0);
			this._tableLayoutPanel.Controls.Add(this._detailTree, 0, 2);
			this._tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 3;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(440, 334);
			this._tableLayoutPanel.TabIndex = 0;
			// 
			// _warningIcon
			// 
			this._warningIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._warningIcon.Image = global::ClearCanvas.Desktop.View.WinForms.SR.Stop;
			this._warningIcon.Location = new System.Drawing.Point(3, 3);
			this._warningIcon.Name = "_warningIcon";
			this._warningIcon.Size = new System.Drawing.Size(34, 36);
			this._warningIcon.TabIndex = 4;
			this._warningIcon.TabStop = false;
			// 
			// _detailTree
			// 
			this._detailTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutPanel.SetColumnSpan(this._detailTree, 2);
			this._detailTree.Location = new System.Drawing.Point(3, 152);
			this._detailTree.Name = "_detailTree";
			this._detailTree.Size = new System.Drawing.Size(434, 179);
			this._detailTree.TabIndex = 1;
			this._detailTree.Visible = false;
			this._detailTree.MouseDown += new System.Windows.Forms.MouseEventHandler(this._detailTree_MouseDown);
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(111, 26);
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(110, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			this.copyToolStripMenuItem.Click += new System.EventHandler(this.copyToolStripMenuItem_Click);
			// 
			// ExceptionHandlerComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this._tableLayoutPanel);
			this.MinimumSize = new System.Drawing.Size(440, 152);
			this.Name = "ExceptionHandlerComponentControl";
			this.Size = new System.Drawing.Size(440, 334);
			this._flowLayoutPanel.ResumeLayout(false);
			this._flowLayoutPanel.PerformLayout();
			this._tableLayoutPanel.ResumeLayout(false);
			this._tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this._warningIcon)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
        private System.Windows.Forms.TextBox _description;
        private System.Windows.Forms.FlowLayoutPanel _flowLayoutPanel;
        private System.Windows.Forms.Button _detailButton;
        private System.Windows.Forms.Button _quitButton;
        private System.Windows.Forms.PictureBox _warningIcon;
        private System.Windows.Forms.TreeView _detailTree;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.Button _okButton;


    }
}
