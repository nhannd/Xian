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

using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	partial class DateFormatApplicationComponentControl
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
			this._dateSample = new System.Windows.Forms.TextBox();
			this._radioCustom = new System.Windows.Forms.RadioButton();
			this._comboCustomDateFormat = new System.Windows.Forms.ComboBox();
			this._radioSystemShortDate = new System.Windows.Forms.RadioButton();
			this._radioSystemLongDate = new System.Windows.Forms.RadioButton();
			this.SuspendLayout();
			// 
			// _dateSample
			// 
			this._dateSample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._dateSample.Enabled = false;
			this._dateSample.Location = new System.Drawing.Point(92, 91);
			this._dateSample.Margin = new System.Windows.Forms.Padding(2);
			this._dateSample.Name = "_dateSample";
			this._dateSample.ReadOnly = true;
			this._dateSample.Size = new System.Drawing.Size(232, 20);
			this._dateSample.TabIndex = 4;
			this._dateSample.TabStop = false;
			// 
			// _radioCustom
			// 
			this._radioCustom.AutoSize = true;
			this._radioCustom.Location = new System.Drawing.Point(26, 60);
			this._radioCustom.Margin = new System.Windows.Forms.Padding(2);
			this._radioCustom.Name = "_radioCustom";
			this._radioCustom.Size = new System.Drawing.Size(60, 17);
			this._radioCustom.TabIndex = 2;
			this._radioCustom.Text = "Custom";
			this._radioCustom.UseVisualStyleBackColor = true;
			// 
			// _comboCustomDateFormat
			// 
			this._comboCustomDateFormat.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
			this._comboCustomDateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboCustomDateFormat.Enabled = false;
			this._comboCustomDateFormat.Location = new System.Drawing.Point(92, 60);
			this._comboCustomDateFormat.Margin = new System.Windows.Forms.Padding(2);
			this._comboCustomDateFormat.Name = "_comboCustomDateFormat";
			this._comboCustomDateFormat.Size = new System.Drawing.Size(232, 21);
			this._comboCustomDateFormat.TabIndex = 3;
			// 
			// _radioSystemShortDate
			// 
			this._radioSystemShortDate.AutoSize = true;
			this._radioSystemShortDate.Location = new System.Drawing.Point(26, 14);
			this._radioSystemShortDate.Margin = new System.Windows.Forms.Padding(2);
			this._radioSystemShortDate.Name = "_radioSystemShortDate";
			this._radioSystemShortDate.Size = new System.Drawing.Size(119, 17);
			this._radioSystemShortDate.TabIndex = 0;
			this._radioSystemShortDate.Text = "Short Date (System)";
			this._radioSystemShortDate.UseVisualStyleBackColor = true;
			// 
			// _radioSystemLongDate
			// 
			this._radioSystemLongDate.AutoSize = true;
			this._radioSystemLongDate.Location = new System.Drawing.Point(26, 37);
			this._radioSystemLongDate.Margin = new System.Windows.Forms.Padding(2);
			this._radioSystemLongDate.Name = "_radioSystemLongDate";
			this._radioSystemLongDate.Size = new System.Drawing.Size(118, 17);
			this._radioSystemLongDate.TabIndex = 1;
			this._radioSystemLongDate.Text = "Long Date (System)";
			this._radioSystemLongDate.UseVisualStyleBackColor = true;
			// 
			// DateFormatApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._comboCustomDateFormat);
			this.Controls.Add(this._radioCustom);
			this.Controls.Add(this._radioSystemLongDate);
			this.Controls.Add(this._radioSystemShortDate);
			this.Controls.Add(this._dateSample);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "DateFormatApplicationComponentControl";
			this.Size = new System.Drawing.Size(333, 122);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox _dateSample;
		private System.Windows.Forms.RadioButton _radioCustom;
		private ComboBox _comboCustomDateFormat;
		private RadioButton _radioSystemShortDate;
		private RadioButton _radioSystemLongDate;
	}
}