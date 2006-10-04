using System;
using System.Windows.Forms;

namespace ClearCanvas.Desktop.Configuration.View.WinForms
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
			this._radioUnchanged = new System.Windows.Forms.RadioButton();
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
			this._dateSample.Location = new System.Drawing.Point(123, 138);
			this._dateSample.Name = "_dateSample";
			this._dateSample.ReadOnly = true;
			this._dateSample.Size = new System.Drawing.Size(308, 22);
			this._dateSample.TabIndex = 5;
			// 
			// _radioUnchanged
			// 
			this._radioUnchanged.AutoSize = true;
			this._radioUnchanged.Location = new System.Drawing.Point(34, 71);
			this._radioUnchanged.Name = "_radioUnchanged";
			this._radioUnchanged.Size = new System.Drawing.Size(192, 21);
			this._radioUnchanged.TabIndex = 2;
			this._radioUnchanged.Text = "Unchanged (ANSI/DICOM)";
			this._radioUnchanged.UseVisualStyleBackColor = true;
			// 
			// _radioCustom
			// 
			this._radioCustom.AutoSize = true;
			this._radioCustom.Location = new System.Drawing.Point(34, 98);
			this._radioCustom.Name = "_radioCustom";
			this._radioCustom.Size = new System.Drawing.Size(73, 21);
			this._radioCustom.TabIndex = 3;
			this._radioCustom.Text = "Custom";
			this._radioCustom.UseVisualStyleBackColor = true;
			// 
			// _comboCustomDateFormat
			// 
			this._comboCustomDateFormat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._comboCustomDateFormat.Enabled = false;
			this._comboCustomDateFormat.Location = new System.Drawing.Point(123, 97);
			this._comboCustomDateFormat.Name = "_comboCustomDateFormat";
			this._comboCustomDateFormat.Size = new System.Drawing.Size(308, 24);
			this._comboCustomDateFormat.TabIndex = 4;
			// 
			// _radioSystemShortDate
			// 
			this._radioSystemShortDate.AutoSize = true;
			this._radioSystemShortDate.Location = new System.Drawing.Point(34, 17);
			this._radioSystemShortDate.Name = "_radioSystemShortDate";
			this._radioSystemShortDate.Size = new System.Drawing.Size(154, 21);
			this._radioSystemShortDate.TabIndex = 0;
			this._radioSystemShortDate.Text = "Short Date (System)";
			this._radioSystemShortDate.UseVisualStyleBackColor = true;
			// 
			// _radioSystemLongDate
			// 
			this._radioSystemLongDate.AutoSize = true;
			this._radioSystemLongDate.Location = new System.Drawing.Point(34, 44);
			this._radioSystemLongDate.Name = "_radioSystemLongDate";
			this._radioSystemLongDate.Size = new System.Drawing.Size(152, 21);
			this._radioSystemLongDate.TabIndex = 1;
			this._radioSystemLongDate.Text = "Long Date (System)";
			this._radioSystemLongDate.UseVisualStyleBackColor = true;
			// 
			// DateFormatApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._comboCustomDateFormat);
			this.Controls.Add(this._radioCustom);
			this.Controls.Add(this._radioUnchanged);
			this.Controls.Add(this._radioSystemLongDate);
			this.Controls.Add(this._radioSystemShortDate);
			this.Controls.Add(this._dateSample);
			this.Name = "DateFormatApplicationComponentControl";
			this.Size = new System.Drawing.Size(444, 174);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.TextBox _dateSample;
		private System.Windows.Forms.RadioButton _radioUnchanged;
		private System.Windows.Forms.RadioButton _radioCustom;
		private ComboBox _comboCustomDateFormat;
		private RadioButton _radioSystemShortDate;
		private RadioButton _radioSystemLongDate;
    }
}
