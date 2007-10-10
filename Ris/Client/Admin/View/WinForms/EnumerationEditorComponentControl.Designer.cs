namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    partial class EnumerationEditorComponentControl
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
            this._code = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._displayValue = new ClearCanvas.Desktop.View.WinForms.TextField();
            this._description = new ClearCanvas.Desktop.View.WinForms.TextAreaField();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _code
            // 
            this._code.LabelText = "Code";
            this._code.Location = new System.Drawing.Point(30, 21);
            this._code.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._code.Mask = "";
            this._code.Name = "_code";
            this._code.Size = new System.Drawing.Size(350, 50);
            this._code.TabIndex = 0;
            this._code.Value = null;
            // 
            // _displayValue
            // 
            this._displayValue.LabelText = "Value";
            this._displayValue.Location = new System.Drawing.Point(30, 99);
            this._displayValue.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._displayValue.Mask = "";
            this._displayValue.Name = "_displayValue";
            this._displayValue.Size = new System.Drawing.Size(350, 50);
            this._displayValue.TabIndex = 1;
            this._displayValue.Value = null;
            // 
            // _description
            // 
            this._description.LabelText = "Description";
            this._description.Location = new System.Drawing.Point(30, 179);
            this._description.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._description.Name = "_description";
            this._description.Size = new System.Drawing.Size(350, 97);
            this._description.TabIndex = 2;
            this._description.Value = null;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(305, 309);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 3;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(224, 309);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 4;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // EnumerationEditorComponentControl
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._cancelButton;
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._description);
            this.Controls.Add(this._displayValue);
            this.Controls.Add(this._code);
            this.Name = "EnumerationEditorComponentControl";
            this.Size = new System.Drawing.Size(418, 355);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Desktop.View.WinForms.TextField _code;
        private ClearCanvas.Desktop.View.WinForms.TextField _displayValue;
        private ClearCanvas.Desktop.View.WinForms.TextAreaField _description;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
    }
}
