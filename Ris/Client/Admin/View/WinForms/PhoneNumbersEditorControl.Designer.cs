namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class PhoneNumbersEditorControl
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
            this._use = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._equipment = new ClearCanvas.Controls.WinForms.ComboBoxField();
            this._countryCode = new ClearCanvas.Controls.WinForms.TextField();
            this._extension = new ClearCanvas.Controls.WinForms.TextField();
            this._areaCode = new ClearCanvas.Controls.WinForms.TextField();
            this._number = new ClearCanvas.Controls.WinForms.TextField();
            this._acceptButton = new System.Windows.Forms.Button();
            this._cancelButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _use
            // 
            this._use.DataSource = null;
            this._use.LabelText = "Use";
            this._use.Location = new System.Drawing.Point(44, 29);
            this._use.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._use.Name = "_use";
            this._use.Size = new System.Drawing.Size(150, 41);
            this._use.TabIndex = 0;
            this._use.Value = null;
            // 
            // _equipment
            // 
            this._equipment.DataSource = null;
            this._equipment.LabelText = "Equipment";
            this._equipment.Location = new System.Drawing.Point(257, 29);
            this._equipment.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._equipment.Name = "_equipment";
            this._equipment.Size = new System.Drawing.Size(150, 41);
            this._equipment.TabIndex = 1;
            this._equipment.Value = null;
            // 
            // _countryCode
            // 
            this._countryCode.LabelText = "Country Code";
            this._countryCode.Location = new System.Drawing.Point(44, 98);
            this._countryCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._countryCode.Name = "_countryCode";
            this._countryCode.Size = new System.Drawing.Size(150, 41);
            this._countryCode.TabIndex = 2;
            this._countryCode.Value = null;
            // 
            // _extension
            // 
            this._extension.LabelText = "Extension";
            this._extension.Location = new System.Drawing.Point(44, 228);
            this._extension.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._extension.Name = "_extension";
            this._extension.Size = new System.Drawing.Size(150, 41);
            this._extension.TabIndex = 5;
            this._extension.Value = null;
            // 
            // _areaCode
            // 
            this._areaCode.LabelText = "Area Code";
            this._areaCode.Location = new System.Drawing.Point(44, 157);
            this._areaCode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._areaCode.Name = "_areaCode";
            this._areaCode.Size = new System.Drawing.Size(150, 41);
            this._areaCode.TabIndex = 3;
            this._areaCode.Value = null;
            // 
            // _number
            // 
            this._number.LabelText = "Number";
            this._number.Location = new System.Drawing.Point(257, 157);
            this._number.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this._number.Name = "_number";
            this._number.Size = new System.Drawing.Size(150, 41);
            this._number.TabIndex = 4;
            this._number.Value = null;
            // 
            // _acceptButton
            // 
            this._acceptButton.Location = new System.Drawing.Point(237, 316);
            this._acceptButton.Name = "_acceptButton";
            this._acceptButton.Size = new System.Drawing.Size(75, 23);
            this._acceptButton.TabIndex = 6;
            this._acceptButton.Text = "Accept";
            this._acceptButton.UseVisualStyleBackColor = true;
            this._acceptButton.Click += new System.EventHandler(this._acceptButton_Click);
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(332, 316);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 7;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // PhoneNumbersEditorControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._acceptButton);
            this.Controls.Add(this._number);
            this.Controls.Add(this._areaCode);
            this.Controls.Add(this._extension);
            this.Controls.Add(this._countryCode);
            this.Controls.Add(this._equipment);
            this.Controls.Add(this._use);
            this.Name = "PhoneNumbersEditorControl";
            this.Size = new System.Drawing.Size(667, 462);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.ComboBoxField _use;
        private ClearCanvas.Controls.WinForms.ComboBoxField _equipment;
        private ClearCanvas.Controls.WinForms.TextField _countryCode;
        private ClearCanvas.Controls.WinForms.TextField _extension;
        private ClearCanvas.Controls.WinForms.TextField _areaCode;
        private ClearCanvas.Controls.WinForms.TextField _number;
        private System.Windows.Forms.Button _acceptButton;
        private System.Windows.Forms.Button _cancelButton;
    }
}
