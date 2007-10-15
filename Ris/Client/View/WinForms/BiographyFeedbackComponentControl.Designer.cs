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
    partial class BiographyFeedbackComponentControl
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
            ClearCanvas.Desktop.Selection selection1 = new ClearCanvas.Desktop.Selection();
            this._feedbackTable = new ClearCanvas.Desktop.View.WinForms.DecoratedTableView();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this._comments = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this._subject = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _feedbackTable
            // 
            this._feedbackTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._feedbackTable.Location = new System.Drawing.Point(0, 0);
            this._feedbackTable.MenuModel = null;
            this._feedbackTable.Name = "_feedbackTable";
            this._feedbackTable.ReadOnly = false;
            this._feedbackTable.Selection = selection1;
            this._feedbackTable.Size = new System.Drawing.Size(527, 239);
            this._feedbackTable.TabIndex = 0;
            this._feedbackTable.Table = null;
            this._feedbackTable.ToolbarModel = null;
            this._feedbackTable.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._feedbackTable.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this._feedbackTable);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this._comments);
            this.splitContainer1.Panel2.Controls.Add(this._subject);
            this.splitContainer1.Size = new System.Drawing.Size(527, 484);
            this.splitContainer1.SplitterDistance = 239;
            this.splitContainer1.TabIndex = 1;
            // 
            // _comments
            // 
            this._comments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this._comments.LabelText = "Comments";
            this._comments.Location = new System.Drawing.Point(4, 51);
            this._comments.Margin = new System.Windows.Forms.Padding(2);
            this._comments.Name = "_comments";
            this._comments.ReadOnly = true;
            this._comments.Size = new System.Drawing.Size(521, 188);
            this._comments.TabIndex = 1;
            this._comments.Value = null;
            // 
            // _subject
            // 
            this._subject.LabelText = "Subject";
            this._subject.Location = new System.Drawing.Point(4, 4);
            this._subject.Margin = new System.Windows.Forms.Padding(2);
            this._subject.Mask = "";
            this._subject.Name = "_subject";
            this._subject.ReadOnly = true;
            this._subject.Size = new System.Drawing.Size(411, 41);
            this._subject.TabIndex = 0;
            this._subject.Value = null;
            // 
            // BiographyFeedbackComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Name = "BiographyFeedbackComponentControl";
            this.Size = new System.Drawing.Size(527, 484);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.DecoratedTableView _feedbackTable;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _comments;
        private ClearCanvas.Desktop.View.WinForms.TextField _subject;
    }
}
