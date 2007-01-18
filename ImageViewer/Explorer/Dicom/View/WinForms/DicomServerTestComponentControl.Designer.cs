namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class DicomServerTestComponentControl
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
            this._toggleServerButton = new System.Windows.Forms.Button();
            this._aeTitle = new ClearCanvas.Controls.WinForms.TextField();
            this._port = new ClearCanvas.Controls.WinForms.TextField();
            this._saveDirectory = new ClearCanvas.Controls.WinForms.TextField();
            this._sopInstanceUID = new System.Windows.Forms.Label();
            this._progress = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // _toggleServerButton
            // 
            this._toggleServerButton.Location = new System.Drawing.Point(0, 201);
            this._toggleServerButton.Name = "_toggleServerButton";
            this._toggleServerButton.Size = new System.Drawing.Size(75, 23);
            this._toggleServerButton.TabIndex = 0;
            this._toggleServerButton.Text = "Start";
            this._toggleServerButton.UseVisualStyleBackColor = true;
            this._toggleServerButton.Click += new System.EventHandler(this._toggleServerButton_Click);
            // 
            // _aeTitle
            // 
            this._aeTitle.LabelText = "AE Title";
            this._aeTitle.Location = new System.Drawing.Point(0, 6);
            this._aeTitle.Margin = new System.Windows.Forms.Padding(2);
            this._aeTitle.Mask = "";
            this._aeTitle.Name = "_aeTitle";
            this._aeTitle.Size = new System.Drawing.Size(150, 41);
            this._aeTitle.TabIndex = 2;
            this._aeTitle.Value = null;
            // 
            // _port
            // 
            this._port.LabelText = "Listening Port";
            this._port.Location = new System.Drawing.Point(0, 51);
            this._port.Margin = new System.Windows.Forms.Padding(2);
            this._port.Mask = "";
            this._port.Name = "_port";
            this._port.Size = new System.Drawing.Size(150, 41);
            this._port.TabIndex = 3;
            this._port.Value = null;
            // 
            // _saveDirectory
            // 
            this._saveDirectory.LabelText = "Save Directory";
            this._saveDirectory.Location = new System.Drawing.Point(0, 96);
            this._saveDirectory.Margin = new System.Windows.Forms.Padding(2);
            this._saveDirectory.Mask = "";
            this._saveDirectory.Name = "_saveDirectory";
            this._saveDirectory.Size = new System.Drawing.Size(150, 41);
            this._saveDirectory.TabIndex = 4;
            this._saveDirectory.Value = null;
            // 
            // _sopInstanceUID
            // 
            this._sopInstanceUID.AutoSize = true;
            this._sopInstanceUID.Location = new System.Drawing.Point(0, 156);
            this._sopInstanceUID.Name = "_sopInstanceUID";
            this._sopInstanceUID.Size = new System.Drawing.Size(95, 13);
            this._sopInstanceUID.TabIndex = 5;
            this._sopInstanceUID.Text = "SOP Instance UID";
            // 
            // _progress
            // 
            this._progress.Location = new System.Drawing.Point(0, 172);
            this._progress.Name = "_progress";
            this._progress.Size = new System.Drawing.Size(147, 23);
            this._progress.TabIndex = 6;
            // 
            // DicomServerTestComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._progress);
            this.Controls.Add(this._sopInstanceUID);
            this.Controls.Add(this._saveDirectory);
            this.Controls.Add(this._port);
            this.Controls.Add(this._aeTitle);
            this.Controls.Add(this._toggleServerButton);
            this.Name = "DicomServerTestComponentControl";
            this.Size = new System.Drawing.Size(162, 232);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button _toggleServerButton;
        private ClearCanvas.Controls.WinForms.TextField _aeTitle;
        private ClearCanvas.Controls.WinForms.TextField _port;
        private ClearCanvas.Controls.WinForms.TextField _saveDirectory;
        private System.Windows.Forms.Label _sopInstanceUID;
        private System.Windows.Forms.ProgressBar _progress;
    }
}
