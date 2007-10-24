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

using System.Windows.Forms;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    partial class PresetVoiLutOperationComponentContainerControl
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
			this._okButton = new System.Windows.Forms.Button();
			this._cancelButton = new System.Windows.Forms.Button();
			this._tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this._keyStrokeComboBox = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._tableLayoutPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(27, 64);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 2;
			this._okButton.Text = "Ok";
			this._okButton.UseVisualStyleBackColor = true;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(108, 64);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 3;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			// 
			// _tableLayoutPanel
			// 
			this._tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._tableLayoutPanel.AutoSize = true;
			this._tableLayoutPanel.ColumnCount = 1;
			this._tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this._tableLayoutPanel.Controls.Add(this._keyStrokeComboBox, 0, 0);
			this._tableLayoutPanel.Location = new System.Drawing.Point(4, 1);
			this._tableLayoutPanel.Name = "_tableLayoutPanel";
			this._tableLayoutPanel.RowCount = 1;
			this._tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
			this._tableLayoutPanel.Size = new System.Drawing.Size(179, 60);
			this._tableLayoutPanel.TabIndex = 4;
			// 
			// _keyStrokeComboBox
			// 
			this._keyStrokeComboBox.DataSource = null;
			this._keyStrokeComboBox.DisplayMember = "";
			this._keyStrokeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._keyStrokeComboBox.LabelText = "Keystroke";
			this._keyStrokeComboBox.Location = new System.Drawing.Point(2, 2);
			this._keyStrokeComboBox.Margin = new System.Windows.Forms.Padding(2);
			this._keyStrokeComboBox.Name = "_keyStrokeComboBox";
			this._keyStrokeComboBox.Size = new System.Drawing.Size(175, 45);
			this._keyStrokeComboBox.TabIndex = 0;
			this._keyStrokeComboBox.Value = null;
			// 
			// EditPresetVoiLutComponentContainerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this._tableLayoutPanel);
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Name = "EditPresetVoiLutComponentContainerControl";
			this.Size = new System.Drawing.Size(187, 90);
			this._tableLayoutPanel.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _okButton;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.TableLayoutPanel _tableLayoutPanel;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _keyStrokeComboBox;

	}
}
