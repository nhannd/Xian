namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class AddressesEditorControl
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
            this._postalCode = new ClearCanvas.Controls.WinForms.TextField();
            this._validFrom = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._validUntil = new ClearCanvas.Controls.WinForms.DateTimeField();
            this._type = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._city = new ClearCanvas.Controls.WinForms.TextField();
            this._street = new ClearCanvas.Controls.WinForms.TextField();
            this._province = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._country = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this.SuspendLayout();
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(466, 219);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 8;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(563, 219);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 9;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _postalCode
            // 
            this._postalCode.LabelText = "Postal Code";
            this._postalCode.Location = new System.Drawing.Point(535, 148);
            this._postalCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._postalCode.Name = "_postalCode";
            this._postalCode.Size = new System.Drawing.Size(102, 41);
            this._postalCode.TabIndex = 5;
            this._postalCode.Value = null;
            // 
            // _validFrom
            // 
            this._validFrom.LabelText = "Valid From";
            this._validFrom.Location = new System.Drawing.Point(314, 24);
            this._validFrom.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._validFrom.Name = "_validFrom";
            this._validFrom.Nullable = true;
            this._validFrom.Size = new System.Drawing.Size(150, 41);
            this._validFrom.TabIndex = 6;
            this._validFrom.Value = new System.DateTime(2006, 7, 26, 16, 37, 8, 953);
            this._validFrom.Visible = false;
            // 
            // _validUntil
            // 
            this._validUntil.LabelText = "Valid Until";
            this._validUntil.Location = new System.Drawing.Point(488, 24);
            this._validUntil.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._validUntil.Name = "_validUntil";
            this._validUntil.Nullable = true;
            this._validUntil.Size = new System.Drawing.Size(150, 41);
            this._validUntil.TabIndex = 7;
            this._validUntil.Value = new System.DateTime(2006, 7, 26, 16, 37, 6, 765);
            this._validUntil.Visible = false;
            // 
            // _type
            // 
            this._type.DataSource = null;
            this._type.LabelText = "Type";
            this._type.Location = new System.Drawing.Point(25, 24);
            this._type.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._type.Name = "_type";
            this._type.Size = new System.Drawing.Size(150, 41);
            this._type.TabIndex = 0;
            this._type.Value = null;
            // 
            // _city
            // 
            this._city.LabelText = "City";
            this._city.Location = new System.Drawing.Point(25, 148);
            this._city.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._city.Name = "_city";
            this._city.Size = new System.Drawing.Size(102, 41);
            this._city.TabIndex = 2;
            this._city.Value = null;
            // 
            // _street
            // 
            this._street.LabelText = "Street";
            this._street.Location = new System.Drawing.Point(25, 90);
            this._street.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._street.Name = "_street";
            this._street.Size = new System.Drawing.Size(613, 41);
            this._street.TabIndex = 1;
            this._street.Value = null;
            // 
            // _province
            // 
            this._province.DataSource = null;
            this._province.LabelText = "Province";
            this._province.Location = new System.Drawing.Point(169, 144);
            this._province.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._province.Name = "_province";
            this._province.Size = new System.Drawing.Size(150, 41);
            this._province.TabIndex = 3;
            this._province.Value = null;
            // 
            // _country
            // 
            this._country.DataSource = null;
            this._country.LabelText = "Country";
            this._country.Location = new System.Drawing.Point(351, 144);
            this._country.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._country.Name = "_country";
            this._country.Size = new System.Drawing.Size(150, 41);
            this._country.TabIndex = 4;
            this._country.Value = null;
            // 
            // AddressesEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._country);
            this.Controls.Add(this._province);
            this.Controls.Add(this._postalCode);
            this.Controls.Add(this._validFrom);
            this.Controls.Add(this._validUntil);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._acceptButton);
            this.Controls.Add(this._type);
            this.Controls.Add(this._city);
            this.Controls.Add(this._street);
            this.Name = "AddressesEditorControl";
            this.Size = new System.Drawing.Size(678, 273);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _street;
        private ClearCanvas.Controls.WinForms.TextField _city;
        private ClearCanvas.Controls.WinForms.ComboBoxField _type;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Button _cancelButton;
        private ClearCanvas.Controls.WinForms.DateTimeField _validUntil;
        private ClearCanvas.Controls.WinForms.DateTimeField _validFrom;
        private ClearCanvas.Controls.WinForms.TextField _postalCode;
        private ClearCanvas.Controls.WinForms.ComboBoxField _province;
        private ClearCanvas.Controls.WinForms.ComboBoxField _country;
    }
}
