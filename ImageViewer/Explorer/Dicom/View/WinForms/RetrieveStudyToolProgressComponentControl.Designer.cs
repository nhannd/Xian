namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    partial class RetrieveStudyToolProgressComponentControl
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
            this._retrieveSourceLabel = new System.Windows.Forms.Label();
            this._studyDescriptionLabel = new System.Windows.Forms.Label();
            this._progressGroupBox = new System.Windows.Forms.GroupBox();
            this._progressTextLabel = new System.Windows.Forms.Label();
            this._progressGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // _retrieveSourceLabel
            // 
            this._retrieveSourceLabel.AutoSize = true;
            this._retrieveSourceLabel.Location = new System.Drawing.Point(13, 22);
            this._retrieveSourceLabel.Name = "_retrieveSourceLabel";
            this._retrieveSourceLabel.Size = new System.Drawing.Size(149, 17);
            this._retrieveSourceLabel.TabIndex = 0;
            this._retrieveSourceLabel.Text = "Retrieve Source Label";
            // 
            // _studyDescriptionLabel
            // 
            this._studyDescriptionLabel.AutoSize = true;
            this._studyDescriptionLabel.Location = new System.Drawing.Point(13, 48);
            this._studyDescriptionLabel.Name = "_studyDescriptionLabel";
            this._studyDescriptionLabel.Size = new System.Drawing.Size(158, 17);
            this._studyDescriptionLabel.TabIndex = 1;
            this._studyDescriptionLabel.Text = "Study Description Label";
            // 
            // _progressGroupBox
            // 
            this._progressGroupBox.Controls.Add(this._progressTextLabel);
            this._progressGroupBox.Location = new System.Drawing.Point(16, 81);
            this._progressGroupBox.Name = "_progressGroupBox";
            this._progressGroupBox.Size = new System.Drawing.Size(451, 69);
            this._progressGroupBox.TabIndex = 2;
            this._progressGroupBox.TabStop = false;
            this._progressGroupBox.Text = "Progress";
            // 
            // _progressTextLabel
            // 
            this._progressTextLabel.AutoSize = true;
            this._progressTextLabel.Location = new System.Drawing.Point(26, 32);
            this._progressTextLabel.Name = "_progressTextLabel";
            this._progressTextLabel.Size = new System.Drawing.Size(135, 17);
            this._progressTextLabel.TabIndex = 0;
            this._progressTextLabel.Text = "Progress Text Label";
            // 
            // RetrieveStudyToolProgressComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._progressGroupBox);
            this.Controls.Add(this._studyDescriptionLabel);
            this.Controls.Add(this._retrieveSourceLabel);
            this.Name = "RetrieveStudyToolProgressComponentControl";
            this.Size = new System.Drawing.Size(483, 166);
            this._progressGroupBox.ResumeLayout(false);
            this._progressGroupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label _retrieveSourceLabel;
        private System.Windows.Forms.Label _studyDescriptionLabel;
        private System.Windows.Forms.GroupBox _progressGroupBox;
        private System.Windows.Forms.Label _progressTextLabel;
    }
}
