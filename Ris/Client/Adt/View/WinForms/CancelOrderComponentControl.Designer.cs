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
    partial class CancelOrderComponentControl
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
            ClearCanvas.Desktop.Selection selection2 = new ClearCanvas.Desktop.Selection();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this._cancelOrderTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
            this._cancelReason = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this.SuspendLayout();
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(522, 281);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 6;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(441, 281);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 5;
            this._okButton.Text = "Confirm";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // _cancelOrderTableView
            // 
            this._cancelOrderTableView.Location = new System.Drawing.Point(14, 56);
            this._cancelOrderTableView.MenuModel = null;
            this._cancelOrderTableView.Name = "_cancelOrderTableView";
            this._cancelOrderTableView.ReadOnly = false;
            this._cancelOrderTableView.Selection = selection2;
            this._cancelOrderTableView.ShowToolbar = false;
            this._cancelOrderTableView.Size = new System.Drawing.Size(583, 219);
            this._cancelOrderTableView.TabIndex = 4;
            this._cancelOrderTableView.Table = null;
            this._cancelOrderTableView.ToolbarModel = null;
            this._cancelOrderTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this._cancelOrderTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // _cancelReason
            // 
            this._cancelReason.DataSource = null;
            this._cancelReason.DisplayMember = "";
            this._cancelReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._cancelReason.LabelText = "Cancel Reason";
            this._cancelReason.Location = new System.Drawing.Point(14, 10);
            this._cancelReason.Margin = new System.Windows.Forms.Padding(2);
            this._cancelReason.Name = "_cancelReason";
            this._cancelReason.Size = new System.Drawing.Size(266, 41);
            this._cancelReason.TabIndex = 7;
            this._cancelReason.Value = null;
            // 
            // CancelOrderComponentControl
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._cancelReason);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelOrderTableView);
            this.Name = "CancelOrderComponentControl";
            this.Size = new System.Drawing.Size(600, 307);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
        private ClearCanvas.Desktop.View.WinForms.TableView _cancelOrderTableView;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _cancelReason;
    }
}
