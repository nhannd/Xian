#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    partial class BiographyOrderHistoryComponentControl
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
			this._orderList = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this._orderPage = new System.Windows.Forms.TabPage();
			this._visitPage = new System.Windows.Forms.TabPage();
			this._documentPage = new System.Windows.Forms.TabPage();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.tabControl1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _orderList
			// 
			this._orderList.Dock = System.Windows.Forms.DockStyle.Fill;
			this._orderList.Location = new System.Drawing.Point(0, 0);
			this._orderList.Name = "_orderList";
			this._orderList.ReadOnly = false;
			this._orderList.Size = new System.Drawing.Size(465, 586);
			this._orderList.TabIndex = 0;
			this._orderList.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this._orderList);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.AutoScroll = true;
			this.splitContainer1.Panel2.Controls.Add(this.tabControl1);
			this.splitContainer1.Size = new System.Drawing.Size(982, 586);
			this.splitContainer1.SplitterDistance = 465;
			this.splitContainer1.TabIndex = 1;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this._orderPage);
			this.tabControl1.Controls.Add(this._visitPage);
			this.tabControl1.Controls.Add(this._documentPage);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(0, 0);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(513, 586);
			this.tabControl1.TabIndex = 24;
			// 
			// _orderPage
			// 
			this._orderPage.Location = new System.Drawing.Point(4, 22);
			this._orderPage.Name = "_orderPage";
			this._orderPage.Padding = new System.Windows.Forms.Padding(3);
			this._orderPage.Size = new System.Drawing.Size(505, 560);
			this._orderPage.TabIndex = 0;
			this._orderPage.Text = "Order Details";
			this._orderPage.UseVisualStyleBackColor = true;
			// 
			// _visitPage
			// 
			this._visitPage.Location = new System.Drawing.Point(4, 22);
			this._visitPage.Name = "_visitPage";
			this._visitPage.Padding = new System.Windows.Forms.Padding(3);
			this._visitPage.Size = new System.Drawing.Size(505, 560);
			this._visitPage.TabIndex = 1;
			this._visitPage.Text = "Order Visit";
			this._visitPage.UseVisualStyleBackColor = true;
			// 
			// _documentPage
			// 
			this._documentPage.Location = new System.Drawing.Point(4, 22);
			this._documentPage.Name = "_documentPage";
			this._documentPage.Padding = new System.Windows.Forms.Padding(3);
			this._documentPage.Size = new System.Drawing.Size(505, 560);
			this._documentPage.TabIndex = 3;
			this._documentPage.Text = "Order Document";
			this._documentPage.UseVisualStyleBackColor = true;
			// 
			// BiographyOrderHistoryComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer1);
			this.Name = "BiographyOrderHistoryComponentControl";
			this.Size = new System.Drawing.Size(982, 586);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			this.tabControl1.ResumeLayout(false);
			this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TableView _orderList;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage _orderPage;
		private System.Windows.Forms.TabPage _visitPage;
		private System.Windows.Forms.TabPage _documentPage;
    }
}
