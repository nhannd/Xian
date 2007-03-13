namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class DicomExplorerConfigurationApplicationComponentControl
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
			this._showPhoneticIdeographicNames = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// _showPhoneticIdeographicNames
			// 
			this._showPhoneticIdeographicNames.AutoSize = true;
			this._showPhoneticIdeographicNames.Location = new System.Drawing.Point(11, 11);
			this._showPhoneticIdeographicNames.Name = "_showPhoneticIdeographicNames";
			this._showPhoneticIdeographicNames.Size = new System.Drawing.Size(210, 17);
			this._showPhoneticIdeographicNames.TabIndex = 0;
			this._showPhoneticIdeographicNames.Text = "Show phonetic and ideographic names";
			this._showPhoneticIdeographicNames.UseVisualStyleBackColor = true;
			// 
			// DicomExplorerConfigurationApplicationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._showPhoneticIdeographicNames);
			this.Name = "DicomExplorerConfigurationApplicationComponentControl";
			this.Size = new System.Drawing.Size(329, 210);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.CheckBox _showPhoneticIdeographicNames;
    }
}
