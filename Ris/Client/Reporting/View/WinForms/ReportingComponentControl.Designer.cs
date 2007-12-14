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

namespace ClearCanvas.Ris.Client.Reporting.View.WinForms
{
    partial class ReportingComponentControl
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
            this._rootSplitContainer = new System.Windows.Forms.SplitContainer();
            this._reportEditorSplitContainer = new System.Windows.Forms.SplitContainer();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this._priorReportsTab = new System.Windows.Forms.TabPage();
            this._orderDetailsTab = new System.Windows.Forms.TabPage();
            this._rootSplitContainer.Panel2.SuspendLayout();
            this._rootSplitContainer.SuspendLayout();
            this._reportEditorSplitContainer.Panel2.SuspendLayout();
            this._reportEditorSplitContainer.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _rootSplitContainer
            // 
            this._rootSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._rootSplitContainer.Location = new System.Drawing.Point(0, 0);
            this._rootSplitContainer.Name = "_rootSplitContainer";
            this._rootSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // _rootSplitContainer.Panel2
            // 
            this._rootSplitContainer.Panel2.Controls.Add(this._reportEditorSplitContainer);
            this._rootSplitContainer.Size = new System.Drawing.Size(599, 535);
            this._rootSplitContainer.SplitterDistance = 95;
            this._rootSplitContainer.TabIndex = 0;
            this._rootSplitContainer.TabStop = false;
            // 
            // _reportEditorSplitContainer
            // 
            this._reportEditorSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this._reportEditorSplitContainer.Location = new System.Drawing.Point(0, 0);
            this._reportEditorSplitContainer.Name = "_reportEditorSplitContainer";
            // 
            // _reportEditorSplitContainer.Panel2
            // 
            this._reportEditorSplitContainer.Panel2.Controls.Add(this.tabControl1);
            this._reportEditorSplitContainer.Size = new System.Drawing.Size(599, 436);
            this._reportEditorSplitContainer.SplitterDistance = 297;
            this._reportEditorSplitContainer.TabIndex = 0;
            this._reportEditorSplitContainer.TabStop = false;
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this._priorReportsTab);
            this.tabControl1.Controls.Add(this._orderDetailsTab);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(298, 436);
            this.tabControl1.TabIndex = 0;
            // 
            // _priorReportsTab
            // 
            this._priorReportsTab.Location = new System.Drawing.Point(4, 22);
            this._priorReportsTab.Name = "_priorReportsTab";
            this._priorReportsTab.Padding = new System.Windows.Forms.Padding(3);
            this._priorReportsTab.Size = new System.Drawing.Size(290, 410);
            this._priorReportsTab.TabIndex = 0;
            this._priorReportsTab.Text = "Prior Reports";
            this._priorReportsTab.UseVisualStyleBackColor = true;
            // 
            // _orderDetailsTab
            // 
            this._orderDetailsTab.Location = new System.Drawing.Point(4, 22);
            this._orderDetailsTab.Name = "_orderDetailsTab";
            this._orderDetailsTab.Padding = new System.Windows.Forms.Padding(3);
            this._orderDetailsTab.Size = new System.Drawing.Size(290, 410);
            this._orderDetailsTab.TabIndex = 1;
            this._orderDetailsTab.Text = "Order Details";
            this._orderDetailsTab.UseVisualStyleBackColor = true;
            // 
            // ReportingComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._rootSplitContainer);
            this.Name = "ReportingComponentControl";
            this.Size = new System.Drawing.Size(599, 535);
            this._rootSplitContainer.Panel2.ResumeLayout(false);
            this._rootSplitContainer.ResumeLayout(false);
            this._reportEditorSplitContainer.Panel2.ResumeLayout(false);
            this._reportEditorSplitContainer.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer _rootSplitContainer;
        private System.Windows.Forms.SplitContainer _reportEditorSplitContainer;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage _priorReportsTab;
        private System.Windows.Forms.TabPage _orderDetailsTab;
    }
}
