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
    partial class NavigatorComponentContainerControl
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._treeView = new System.Windows.Forms.TreeView();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this._contentPanel = new System.Windows.Forms.Panel();
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this._nextButton = new System.Windows.Forms.Button();
			this._backButton = new System.Windows.Forms.Button();
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._applyButton = new System.Windows.Forms.Button();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.flowLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Margin = new System.Windows.Forms.Padding(2);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._treeView);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.tableLayoutPanel1);
			this.splitContainer1.Size = new System.Drawing.Size(638, 377);
			this.splitContainer1.SplitterDistance = 162;
			this.splitContainer1.SplitterWidth = 3;
			this.splitContainer1.TabIndex = 0;
			// 
			// _treeView
			// 
			this._treeView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._treeView.HideSelection = false;
			this._treeView.Location = new System.Drawing.Point(0, 0);
			this._treeView.Margin = new System.Windows.Forms.Padding(2);
			this._treeView.Name = "_treeView";
			this._treeView.Size = new System.Drawing.Size(162, 377);
			this._treeView.TabIndex = 0;
			this._treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this._treeView_AfterSelect);
			this._treeView.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this._treeView_BeforeSelect);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this._contentPanel, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this._titleBar, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.Size = new System.Drawing.Size(473, 377);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// _contentPanel
			// 
			this._contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this._contentPanel.Location = new System.Drawing.Point(2, 33);
			this._contentPanel.Margin = new System.Windows.Forms.Padding(2);
			this._contentPanel.Name = "_contentPanel";
			this._contentPanel.Size = new System.Drawing.Size(469, 303);
			this._contentPanel.TabIndex = 0;
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this._cancelButton);
			this.flowLayoutPanel1.Controls.Add(this._okButton);
			this.flowLayoutPanel1.Controls.Add(this._applyButton);
			this.flowLayoutPanel1.Controls.Add(this._nextButton);
			this.flowLayoutPanel1.Controls.Add(this._backButton);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(41, 340);
			this.flowLayoutPanel1.Margin = new System.Windows.Forms.Padding(2);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Size = new System.Drawing.Size(430, 35);
			this.flowLayoutPanel1.TabIndex = 1;
			// 
			// _cancelButton
			// 
			this._cancelButton.Location = new System.Drawing.Point(353, 2);
			this._cancelButton.Margin = new System.Windows.Forms.Padding(2);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
			// 
			// _okButton
			// 
			this._okButton.Location = new System.Drawing.Point(274, 2);
			this._okButton.Margin = new System.Windows.Forms.Padding(2);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 2;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this._okButton_Click);
			// 
			// _nextButton
			// 
			this._nextButton.Location = new System.Drawing.Point(116, 2);
			this._nextButton.Margin = new System.Windows.Forms.Padding(2);
			this._nextButton.Name = "_nextButton";
			this._nextButton.Size = new System.Drawing.Size(75, 23);
			this._nextButton.TabIndex = 1;
			this._nextButton.Text = "Next >";
			this._nextButton.UseVisualStyleBackColor = true;
			this._nextButton.Click += new System.EventHandler(this._nextButton_Click);
			// 
			// _backButton
			// 
			this._backButton.Location = new System.Drawing.Point(37, 2);
			this._backButton.Margin = new System.Windows.Forms.Padding(2);
			this._backButton.Name = "_backButton";
			this._backButton.Size = new System.Drawing.Size(75, 23);
			this._backButton.TabIndex = 0;
			this._backButton.Text = "< Previous";
			this._backButton.UseVisualStyleBackColor = true;
			this._backButton.Click += new System.EventHandler(this._backButton_Click);
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.GradientColoring = Crownwood.DotNetMagic.Controls.GradientColoring.LightBackToDarkBack;
			this._titleBar.Location = new System.Drawing.Point(3, 3);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(467, 25);
			this._titleBar.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this._titleBar.TabIndex = 3;
			this._titleBar.Text = "titleBar1";
			// 
			// _applyButton
			// 
			this._applyButton.Location = new System.Drawing.Point(195, 2);
			this._applyButton.Margin = new System.Windows.Forms.Padding(2);
			this._applyButton.Name = "_applyButton";
			this._applyButton.Size = new System.Drawing.Size(75, 23);
			this._applyButton.TabIndex = 4;
			this._applyButton.Text = "Apply";
			this._applyButton.UseVisualStyleBackColor = true;
			// 
			// NavigatorComponentContainerControl
			// 
			this.AcceptButton = this._okButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelButton;
			this.Controls.Add(this.splitContainer1);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "NavigatorComponentContainerControl";
			this.Size = new System.Drawing.Size(638, 377);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.TreeView _treeView;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel _contentPanel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _backButton;
        private System.Windows.Forms.Button _nextButton;
        private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;
		private System.Windows.Forms.Button _applyButton;
    }
}
