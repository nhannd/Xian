namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    partial class DicomServerConfigurationComponentControl
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
			this._aeTitle = new ClearCanvas.Desktop.View.WinForms.TextField();
			this._port = new ClearCanvas.Desktop.View.WinForms.TextField();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this._refreshButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _aeTitle
			// 
			this._aeTitle.LabelText = "AE Title";
			this._aeTitle.Location = new System.Drawing.Point(2, 2);
			this._aeTitle.Margin = new System.Windows.Forms.Padding(2);
			this._aeTitle.Mask = "";
			this._aeTitle.Name = "_aeTitle";
			this._aeTitle.Size = new System.Drawing.Size(150, 41);
			this._aeTitle.TabIndex = 1;
			this._aeTitle.Value = null;
			// 
			// _port
			// 
			this._port.LabelText = "Port";
			this._port.Location = new System.Drawing.Point(2, 49);
			this._port.Margin = new System.Windows.Forms.Padding(2);
			this._port.Mask = "";
			this._port.Name = "_port";
			this._port.Size = new System.Drawing.Size(150, 41);
			this._port.TabIndex = 2;
			this._port.Value = null;
			// 
			// _refreshButton
			// 
			this._refreshButton.Location = new System.Drawing.Point(4, 95);
			this._refreshButton.Name = "_refreshButton";
			this._refreshButton.Size = new System.Drawing.Size(75, 23);
			this._refreshButton.TabIndex = 5;
			this._refreshButton.Text = "Refresh";
			this._refreshButton.UseVisualStyleBackColor = true;
			this._refreshButton.Click += new System.EventHandler(this._refreshButton_Click);
			// 
			// DicomServerConfigurationComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._refreshButton);
			this.Controls.Add(this._port);
			this.Controls.Add(this._aeTitle);
			this.Name = "DicomServerConfigurationComponentControl";
			this.Size = new System.Drawing.Size(158, 137);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Desktop.View.WinForms.TextField _aeTitle;
		private ClearCanvas.Desktop.View.WinForms.TextField _port;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button _refreshButton;
    }
}
