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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class JscriptComponentControl
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
			this._script = new System.Windows.Forms.TextBox();
			this._result = new System.Windows.Forms.TextBox();
			this._runButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// _script
			// 
			this._script.AcceptsReturn = true;
			this._script.Location = new System.Drawing.Point(15, 21);
			this._script.Margin = new System.Windows.Forms.Padding(2);
			this._script.Multiline = true;
			this._script.Name = "_script";
			this._script.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._script.Size = new System.Drawing.Size(248, 118);
			this._script.TabIndex = 1;
			// 
			// _result
			// 
			this._result.Location = new System.Drawing.Point(15, 167);
			this._result.Margin = new System.Windows.Forms.Padding(2);
			this._result.Multiline = true;
			this._result.Name = "_result";
			this._result.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this._result.Size = new System.Drawing.Size(248, 118);
			this._result.TabIndex = 4;
			// 
			// _runButton
			// 
			this._runButton.Location = new System.Drawing.Point(188, 295);
			this._runButton.Margin = new System.Windows.Forms.Padding(2);
			this._runButton.Name = "_runButton";
			this._runButton.Size = new System.Drawing.Size(75, 23);
			this._runButton.TabIndex = 2;
			this._runButton.Text = "Run";
			this._runButton.UseVisualStyleBackColor = true;
			this._runButton.Click += new System.EventHandler(this._runButton_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(13, 5);
			this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(34, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Script";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(13, 149);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(37, 13);
			this.label2.TabIndex = 3;
			this.label2.Text = "Result";
			// 
			// JscriptComponentControl
			// 
			this.AcceptButton = this._runButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this._runButton);
			this.Controls.Add(this._result);
			this.Controls.Add(this._script);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "JscriptComponentControl";
			this.Size = new System.Drawing.Size(279, 327);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox _script;
        private System.Windows.Forms.TextBox _result;
        private System.Windows.Forms.Button _runButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
