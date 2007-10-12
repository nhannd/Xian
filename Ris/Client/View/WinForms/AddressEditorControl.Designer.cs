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
    partial class AddressEditorControl
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
            this._acceptButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this._postalCode = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._validFrom = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._validUntil = new ClearCanvas.Desktop.View.WinForms.DateTimeField();
            this._type = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._city = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._street = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._province = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this._country = new ClearCanvas.Desktop.View.WinForms.ComboBoxField();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this._unit = new ClearCanvas.Desktop.View.WinForms.TextField();
            this.tableLayoutPanel1.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(392, 3);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 0;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(473, 3);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _postalCode
            // 
            this._postalCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this._postalCode.LabelText = "Postal Code";
            this._postalCode.Location = new System.Drawing.Point(280, 92);
            this._postalCode.Margin = new System.Windows.Forms.Padding(2);
            this._postalCode.Name = "_postalCode";
            this._postalCode.Size = new System.Drawing.Size(135, 41);
            this._postalCode.TabIndex = 7;
            this._postalCode.Value = null;
            // 
            // _validFrom
            // 
            this._validFrom.Dock = System.Windows.Forms.DockStyle.Fill;
            this._validFrom.LabelText = "Valid From";
            this._validFrom.Location = new System.Drawing.Point(280, 2);
            this._validFrom.Margin = new System.Windows.Forms.Padding(2);
            this._validFrom.Name = "_validFrom";
            this._validFrom.Nullable = true;
            this._validFrom.ShowTime = false;
            this._validFrom.Size = new System.Drawing.Size(135, 41);
            this._validFrom.TabIndex = 1;
            this._validFrom.Value = new System.DateTime(2006, 7, 26, 16, 37, 8, 953);
            // 
            // _validUntil
            // 
            this._validUntil.Dock = System.Windows.Forms.DockStyle.Fill;
            this._validUntil.LabelText = "Valid Until";
            this._validUntil.Location = new System.Drawing.Point(419, 2);
            this._validUntil.Margin = new System.Windows.Forms.Padding(2);
            this._validUntil.Name = "_validUntil";
            this._validUntil.Nullable = true;
            this._validUntil.ShowTime = false;
            this._validUntil.Size = new System.Drawing.Size(136, 41);
            this._validUntil.TabIndex = 2;
            this._validUntil.Value = new System.DateTime(2006, 7, 26, 16, 37, 6, 765);
            // 
            // _type
            // 
            this._type.DataSource = null;
            this._type.Dock = System.Windows.Forms.DockStyle.Fill;
            this._type.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._type.LabelText = "Type";
            this._type.Location = new System.Drawing.Point(2, 2);
            this._type.Margin = new System.Windows.Forms.Padding(2);
            this._type.Name = "_type";
            this._type.Size = new System.Drawing.Size(135, 41);
            this._type.TabIndex = 0;
            this._type.Value = null;
            // 
            // _city
            // 
            this._city.Dock = System.Windows.Forms.DockStyle.Fill;
            this._city.LabelText = "City";
            this._city.Location = new System.Drawing.Point(2, 92);
            this._city.Margin = new System.Windows.Forms.Padding(2);
            this._city.Name = "_city";
            this._city.Size = new System.Drawing.Size(135, 41);
            this._city.TabIndex = 5;
            this._city.Value = null;
            // 
            // _street
            // 
            this.tableLayoutPanel1.SetColumnSpan(this._street, 3);
            this._street.Dock = System.Windows.Forms.DockStyle.Fill;
            this._street.LabelText = "Street";
            this._street.Location = new System.Drawing.Point(2, 47);
            this._street.Margin = new System.Windows.Forms.Padding(2);
            this._street.Name = "_street";
            this._street.Size = new System.Drawing.Size(413, 41);
            this._street.TabIndex = 3;
            this._street.Value = null;
            // 
            // _province
            // 
            this._province.DataSource = null;
            this._province.Dock = System.Windows.Forms.DockStyle.Fill;
            this._province.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._province.LabelText = "Province";
            this._province.Location = new System.Drawing.Point(141, 92);
            this._province.Margin = new System.Windows.Forms.Padding(2);
            this._province.Name = "_province";
            this._province.Size = new System.Drawing.Size(135, 41);
            this._province.TabIndex = 6;
            this._province.Value = null;
            // 
            // _country
            // 
            this._country.DataSource = null;
            this._country.Dock = System.Windows.Forms.DockStyle.Fill;
            this._country.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this._country.LabelText = "Country";
            this._country.Location = new System.Drawing.Point(419, 92);
            this._country.Margin = new System.Windows.Forms.Padding(2);
            this._country.Name = "_country";
            this._country.Size = new System.Drawing.Size(136, 41);
            this._country.TabIndex = 8;
            this._country.Value = null;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.AutoSize = true;
            this.tableLayoutPanel1.ColumnCount = 4;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this._type, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this._country, 3, 2);
            this.tableLayoutPanel1.Controls.Add(this._street, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this._validUntil, 3, 0);
            this.tableLayoutPanel1.Controls.Add(this._city, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this._validFrom, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this._postalCode, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this._province, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this._unit, 3, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(557, 171);
            this.tableLayoutPanel1.TabIndex = 10;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.AutoSize = true;
            this.flowLayoutPanel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel1.SetColumnSpan(this.flowLayoutPanel1, 4);
            this.flowLayoutPanel1.Controls.Add(this._cancelButton);
            this.flowLayoutPanel1.Controls.Add(this._acceptButton);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 138);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(551, 30);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // _unit
            // 
            this._unit.LabelText = "Apartment/Unit";
            this._unit.Location = new System.Drawing.Point(419, 47);
            this._unit.Margin = new System.Windows.Forms.Padding(2);
            this._unit.Name = "_unit";
            this._unit.Size = new System.Drawing.Size(136, 41);
            this._unit.TabIndex = 4;
            this._unit.Value = null;
            // 
            // AddressEditorControl
            // 
            this.AcceptButton = this._acceptButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "AddressEditorControl";
            this.Size = new System.Drawing.Size(557, 171);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _street;
        private ClearCanvas.Desktop.View.WinForms.TextField _city;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _type;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validUntil;
        private ClearCanvas.Desktop.View.WinForms.DateTimeField _validFrom;
        private ClearCanvas.Desktop.View.WinForms.TextField _postalCode;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _province;
        private ClearCanvas.Desktop.View.WinForms.ComboBoxField _country;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private ClearCanvas.Desktop.View.WinForms.TextField _unit;
    }
}
