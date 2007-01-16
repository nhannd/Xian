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
            this._closeButton = new System.Windows.Forms.Button();
            this._aeTitle = new ClearCanvas.Controls.WinForms.TextField();
            this._port = new ClearCanvas.Controls.WinForms.TextField();
            this.SuspendLayout();
            // 
            // _toggleServerButton
            // 
            this._toggleServerButton.Location = new System.Drawing.Point(4, 97);
            this._toggleServerButton.Name = "_toggleServerButton";
            this._toggleServerButton.Size = new System.Drawing.Size(75, 23);
            this._toggleServerButton.TabIndex = 0;
            this._toggleServerButton.Text = "Start";
            this._toggleServerButton.UseVisualStyleBackColor = true;
            this._toggleServerButton.Click += new System.EventHandler(this._toggleServerButton_Click);
            // 
            // _closeButton
            // 
            this._closeButton.Location = new System.Drawing.Point(92, 97);
            this._closeButton.Name = "_closeButton";
            this._closeButton.Size = new System.Drawing.Size(75, 23);
            this._closeButton.TabIndex = 1;
            this._closeButton.Text = "Close";
            this._closeButton.UseVisualStyleBackColor = true;
            this._closeButton.Click += new System.EventHandler(this._closeButton_Click);
            // 
            // _aeTitle
            // 
            this._aeTitle.LabelText = "AE Title";
            this._aeTitle.Location = new System.Drawing.Point(4, 4);
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
            this._port.Location = new System.Drawing.Point(4, 51);
            this._port.Margin = new System.Windows.Forms.Padding(2);
            this._port.Mask = "";
            this._port.Name = "_port";
            this._port.Size = new System.Drawing.Size(150, 41);
            this._port.TabIndex = 3;
            this._port.Value = null;
            // 
            // DicomServerTestComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this._closeButton;
            this.Controls.Add(this._port);
            this.Controls.Add(this._aeTitle);
            this.Controls.Add(this._closeButton);
            this.Controls.Add(this._toggleServerButton);
            this.Name = "DicomServerTestComponentControl";
            this.Size = new System.Drawing.Size(170, 128);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button _toggleServerButton;
        private System.Windows.Forms.Button _closeButton;
        private ClearCanvas.Controls.WinForms.TextField _aeTitle;
        private ClearCanvas.Controls.WinForms.TextField _port;
    }
}
