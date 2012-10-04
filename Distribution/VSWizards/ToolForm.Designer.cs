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

namespace ClearCanvas.VSWizards
{
    partial class ToolForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this._templateChoices = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._toolExtPointChoices = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this._classList = new System.Windows.Forms.ListBox();
            this._contextInterface = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // _templateChoices
            // 
            this._templateChoices.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._templateChoices.FormattingEnabled = true;
            this._templateChoices.Location = new System.Drawing.Point(26, 38);
            this._templateChoices.Name = "_templateChoices";
            this._templateChoices.Size = new System.Drawing.Size(500, 24);
            this._templateChoices.TabIndex = 1;
            this._templateChoices.SelectedIndexChanged += new System.EventHandler(this._templateChoices_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "Template";
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(370, 321);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 8;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(451, 321);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 9;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _toolExtPointChoices
            // 
            this._toolExtPointChoices.FormattingEnabled = true;
            this._toolExtPointChoices.Location = new System.Drawing.Point(26, 101);
            this._toolExtPointChoices.Name = "_toolExtPointChoices";
            this._toolExtPointChoices.Size = new System.Drawing.Size(500, 24);
            this._toolExtPointChoices.TabIndex = 3;
            this._toolExtPointChoices.SelectedIndexChanged += new System.EventHandler(this._toolExtPointChoices_SelectedIndexChanged);
            this._toolExtPointChoices.TextChanged += new System.EventHandler(this._toolExtPointChoices_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(137, 17);
            this.label2.TabIndex = 2;
            this.label2.Text = "Tool Extension Point";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 145);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(146, 17);
            this.label3.TabIndex = 4;
            this.label3.Text = "Tool Context Interface";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 213);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(257, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "The following classes will be generated:";
            // 
            // _classList
            // 
            this._classList.FormattingEnabled = true;
            this._classList.ItemHeight = 16;
            this._classList.Location = new System.Drawing.Point(25, 233);
            this._classList.Name = "_classList";
            this._classList.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this._classList.Size = new System.Drawing.Size(501, 68);
            this._classList.TabIndex = 7;
            // 
            // _contextInterface
            // 
            this._contextInterface.Location = new System.Drawing.Point(25, 165);
            this._contextInterface.Name = "_contextInterface";
            this._contextInterface.Size = new System.Drawing.Size(501, 22);
            this._contextInterface.TabIndex = 5;
            // 
            // ToolForm
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(568, 356);
            this.Controls.Add(this._contextInterface);
            this.Controls.Add(this.label4);
            this.Controls.Add(this._classList);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this._toolExtPointChoices);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._templateChoices);
            this.Name = "ToolForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "ClearCanvas Tool Wizard";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox _templateChoices;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _okButton;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.ComboBox _toolExtPointChoices;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ListBox _classList;
        private System.Windows.Forms.TextBox _contextInterface;
    }
}