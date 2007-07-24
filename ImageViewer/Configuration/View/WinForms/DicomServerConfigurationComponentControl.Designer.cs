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
			this._aeTitle = new ClearCanvas.Controls.WinForms.TextField();
			this._port = new ClearCanvas.Controls.WinForms.TextField();
			this._storageDir = new ClearCanvas.Controls.WinForms.TextField();
			this._buttonBrowse = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this._refreshButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _aeTitle
			// 
			this._aeTitle.LabelText = "AETitle";
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
			// _storageDir
			// 
			this._storageDir.LabelText = "Interim Storage Directory";
			this._storageDir.Location = new System.Drawing.Point(2, 96);
			this._storageDir.Margin = new System.Windows.Forms.Padding(2);
			this._storageDir.Mask = "";
			this._storageDir.Name = "_storageDir";
			this._storageDir.Size = new System.Drawing.Size(150, 41);
			this._storageDir.TabIndex = 3;
			this._storageDir.Value = null;
			// 
			// _buttonBrowse
			// 
			this._buttonBrowse.Location = new System.Drawing.Point(158, 113);
			this._buttonBrowse.Name = "_buttonBrowse";
			this._buttonBrowse.Size = new System.Drawing.Size(75, 23);
			this._buttonBrowse.TabIndex = 4;
			this._buttonBrowse.Text = "Browse";
			this._buttonBrowse.UseVisualStyleBackColor = true;
			this._buttonBrowse.Click += new System.EventHandler(this._buttonBrowse_Click);
			// 
			// _refreshButton
			// 
			this._refreshButton.Location = new System.Drawing.Point(2, 142);
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
			this.Controls.Add(this._buttonBrowse);
			this.Controls.Add(this._storageDir);
			this.Controls.Add(this._port);
			this.Controls.Add(this._aeTitle);
			this.Name = "DicomServerConfigurationComponentControl";
			this.Size = new System.Drawing.Size(252, 226);
			this.ResumeLayout(false);

        }

        #endregion

		private ClearCanvas.Controls.WinForms.TextField _aeTitle;
        private ClearCanvas.Controls.WinForms.TextField _port;
        private ClearCanvas.Controls.WinForms.TextField _storageDir;
        private System.Windows.Forms.Button _buttonBrowse;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Button _refreshButton;
    }
}
