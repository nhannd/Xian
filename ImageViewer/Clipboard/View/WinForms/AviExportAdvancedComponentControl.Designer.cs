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

namespace ClearCanvas.ImageViewer.Clipboard.View.WinForms
{
    partial class AviExportAdvancedComponentControl
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AviExportAdvancedComponentControl));
			this._trackBarQuality = new System.Windows.Forms.TrackBar();
			this._quality = new System.Windows.Forms.TextBox();
			this._comboCodec = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
			this._groupQuality = new System.Windows.Forms.GroupBox();
			this._checkUseDefaultQuality = new System.Windows.Forms.CheckBox();
			this._buttonCancel = new System.Windows.Forms.Button();
			this._buttonOk = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this._trackBarQuality)).BeginInit();
			this._groupQuality.SuspendLayout();
			this.SuspendLayout();
			// 
			// _trackBarQuality
			// 
			this._trackBarQuality.LargeChange = 10;
			resources.ApplyResources(this._trackBarQuality, "_trackBarQuality");
			this._trackBarQuality.Maximum = 100;
			this._trackBarQuality.Minimum = 25;
			this._trackBarQuality.Name = "_trackBarQuality";
			this._trackBarQuality.SmallChange = 5;
			this._trackBarQuality.TickFrequency = 5;
			this._trackBarQuality.Value = 25;
			// 
			// _quality
			// 
			this._quality.BackColor = System.Drawing.SystemColors.Control;
			this._quality.BorderStyle = System.Windows.Forms.BorderStyle.None;
			resources.ApplyResources(this._quality, "_quality");
			this._quality.Name = "_quality";
			this._quality.ReadOnly = true;
			this._quality.TabStop = false;
			// 
			// _comboCodec
			// 
			this._comboCodec.DataSource = null;
			this._comboCodec.DisplayMember = "";
			this._comboCodec.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboCodec.LabelText = "Codec";
			resources.ApplyResources(this._comboCodec, "_comboCodec");
			this._comboCodec.Name = "_comboCodec";
			this._comboCodec.Value = null;
			// 
			// _groupQuality
			// 
			this._groupQuality.Controls.Add(this._checkUseDefaultQuality);
			this._groupQuality.Controls.Add(this._trackBarQuality);
			this._groupQuality.Controls.Add(this._quality);
			resources.ApplyResources(this._groupQuality, "_groupQuality");
			this._groupQuality.Name = "_groupQuality";
			this._groupQuality.TabStop = false;
			// 
			// _checkUseDefaultQuality
			// 
			resources.ApplyResources(this._checkUseDefaultQuality, "_checkUseDefaultQuality");
			this._checkUseDefaultQuality.Name = "_checkUseDefaultQuality";
			this._checkUseDefaultQuality.UseVisualStyleBackColor = true;
			// 
			// _buttonCancel
			// 
			resources.ApplyResources(this._buttonCancel, "_buttonCancel");
			this._buttonCancel.Name = "_buttonCancel";
			this._buttonCancel.UseVisualStyleBackColor = true;
			this._buttonCancel.Click += new System.EventHandler(this.OnCancel);
			// 
			// _buttonOk
			// 
			resources.ApplyResources(this._buttonOk, "_buttonOk");
			this._buttonOk.Name = "_buttonOk";
			this._buttonOk.UseVisualStyleBackColor = true;
			this._buttonOk.Click += new System.EventHandler(this.OnOk);
			// 
			// AviExportAdvancedComponentControl
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._buttonCancel);
			this.Controls.Add(this._buttonOk);
			this.Controls.Add(this._groupQuality);
			this.Controls.Add(this._comboCodec);
			this.Name = "AviExportAdvancedComponentControl";
			((System.ComponentModel.ISupportInitialize)(this._trackBarQuality)).EndInit();
			this._groupQuality.ResumeLayout(false);
			this._groupQuality.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.TrackBar _trackBarQuality;
		private System.Windows.Forms.TextBox _quality;
		private ClearCanvas.Desktop.View.WinForms.ComboBoxField _comboCodec;
		private System.Windows.Forms.GroupBox _groupQuality;
		private System.Windows.Forms.CheckBox _checkUseDefaultQuality;
		private System.Windows.Forms.Button _buttonCancel;
		private System.Windows.Forms.Button _buttonOk;
    }
}
