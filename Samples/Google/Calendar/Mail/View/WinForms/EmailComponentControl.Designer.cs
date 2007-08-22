namespace ClearCanvas.Samples.Google.Calendar.Mail.View.WinForms
{
    partial class EmailComponentControl
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
            this._emailAddress = new ClearCanvas.Controls.WinForms.TextField();
            this._cancelButton = new System.Windows.Forms.Button();
            this._okButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _emailAddress
            // 
            this._emailAddress.LabelText = "E-mail Address:";
            this._emailAddress.Location = new System.Drawing.Point(17, 28);
            this._emailAddress.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this._emailAddress.Mask = "";
            this._emailAddress.Name = "_emailAddress";
            this._emailAddress.Size = new System.Drawing.Size(375, 50);
            this._emailAddress.TabIndex = 0;
            this._emailAddress.Value = null;
            // 
            // _cancelButton
            // 
            this._cancelButton.Location = new System.Drawing.Point(318, 109);
            this._cancelButton.Name = "_cancelButton";
            this._cancelButton.Size = new System.Drawing.Size(75, 23);
            this._cancelButton.TabIndex = 1;
            this._cancelButton.Text = "Cancel";
            this._cancelButton.UseVisualStyleBackColor = true;
            this._cancelButton.Click += new System.EventHandler(this._cancelButton_Click);
            // 
            // _okButton
            // 
            this._okButton.Location = new System.Drawing.Point(237, 109);
            this._okButton.Name = "_okButton";
            this._okButton.Size = new System.Drawing.Size(75, 23);
            this._okButton.TabIndex = 2;
            this._okButton.Text = "OK";
            this._okButton.UseVisualStyleBackColor = true;
            this._okButton.Click += new System.EventHandler(this._okButton_Click);
            // 
            // EmailComponentControl
            // 
            this.AcceptButton = this._okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._okButton);
            this.Controls.Add(this._cancelButton);
            this.Controls.Add(this._emailAddress);
            this.Name = "EmailComponentControl";
            this.Size = new System.Drawing.Size(419, 150);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _emailAddress;
        private System.Windows.Forms.Button _cancelButton;
        private System.Windows.Forms.Button _okButton;
    }
}
