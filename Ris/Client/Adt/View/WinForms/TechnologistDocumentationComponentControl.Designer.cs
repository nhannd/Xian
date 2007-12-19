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

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    partial class TechnologistDocumentationComponentControl
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
            this._tableLayoutPanelRoot = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._btnSave = new System.Windows.Forms.Button();
            this._btnComplete = new System.Windows.Forms.Button();
            this._bannerPanel = new System.Windows.Forms.Panel();
            this._orderDocumentationPanel = new System.Windows.Forms.Panel();
            this._tableLayoutPanelRoot.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _tableLayoutPanelRoot
            // 
            this._tableLayoutPanelRoot.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._tableLayoutPanelRoot.ColumnCount = 1;
            this._tableLayoutPanelRoot.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanelRoot.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this._tableLayoutPanelRoot.Controls.Add(this._bannerPanel, 0, 0);
            this._tableLayoutPanelRoot.Controls.Add(this._orderDocumentationPanel, 0, 1);
            this._tableLayoutPanelRoot.Dock = System.Windows.Forms.DockStyle.Fill;
            this._tableLayoutPanelRoot.Location = new System.Drawing.Point(0, 0);
            this._tableLayoutPanelRoot.Name = "_tableLayoutPanelRoot";
            this._tableLayoutPanelRoot.RowCount = 3;
            this._tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this._tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._tableLayoutPanelRoot.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this._tableLayoutPanelRoot.Size = new System.Drawing.Size(1033, 650);
            this._tableLayoutPanelRoot.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.flowLayoutPanel1.Controls.Add(this._btnSave);
            this.flowLayoutPanel1.Controls.Add(this._btnComplete);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 618);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.flowLayoutPanel1.Size = new System.Drawing.Size(1027, 29);
            this.flowLayoutPanel1.TabIndex = 0;
            // 
            // _btnSave
            // 
            this._btnSave.Location = new System.Drawing.Point(949, 3);
            this._btnSave.Name = "_btnSave";
            this._btnSave.Size = new System.Drawing.Size(75, 23);
            this._btnSave.TabIndex = 1;
            this._btnSave.Text = "Save";
            this._btnSave.UseVisualStyleBackColor = true;
            this._btnSave.Click += new System.EventHandler(this._btnSave_Click);
            // 
            // _btnComplete
            // 
            this._btnComplete.Location = new System.Drawing.Point(838, 3);
            this._btnComplete.Name = "_btnComplete";
            this._btnComplete.Size = new System.Drawing.Size(105, 23);
            this._btnComplete.TabIndex = 0;
            this._btnComplete.Text = "Complete";
            this._btnComplete.UseVisualStyleBackColor = true;
            this._btnComplete.Click += new System.EventHandler(this._btnComplete_Click);
            // 
            // _bannerPanel
            // 
            this._bannerPanel.AutoSize = true;
            this._bannerPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._bannerPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bannerPanel.Location = new System.Drawing.Point(3, 3);
            this._bannerPanel.Name = "_bannerPanel";
            this._bannerPanel.Size = new System.Drawing.Size(1027, 94);
            this._bannerPanel.TabIndex = 1;
            // 
            // _orderDocumentationPanel
            // 
            this._orderDocumentationPanel.AutoSize = true;
            this._orderDocumentationPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._orderDocumentationPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this._orderDocumentationPanel.Location = new System.Drawing.Point(3, 103);
            this._orderDocumentationPanel.Name = "_orderDocumentationPanel";
            this._orderDocumentationPanel.Size = new System.Drawing.Size(1027, 509);
            this._orderDocumentationPanel.TabIndex = 2;
            // 
            // TechnologistDocumentationComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this._tableLayoutPanelRoot);
            this.Name = "TechnologistDocumentationComponentControl";
            this.Size = new System.Drawing.Size(1033, 650);
            this._tableLayoutPanelRoot.ResumeLayout(false);
            this._tableLayoutPanelRoot.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel _tableLayoutPanelRoot;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button _btnComplete;
        private System.Windows.Forms.Button _btnSave;
        private System.Windows.Forms.Panel _bannerPanel;
        private System.Windows.Forms.Panel _orderDocumentationPanel;
    }
}
