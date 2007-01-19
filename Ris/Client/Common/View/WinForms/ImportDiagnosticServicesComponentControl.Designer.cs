namespace ClearCanvas.Ris.Client.Common.View.WinForms
{
    partial class ImportDiagnosticServicesComponentControl
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
            this._filename = new ClearCanvas.Controls.WinForms.TextField();
            this._browseButton = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this._batchSize = new ClearCanvas.Controls.WinForms.TextField();
            this._startButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // _filename
            // 
            this._filename.LabelText = "File to import:";
            this._filename.Location = new System.Drawing.Point(16, 15);
            this._filename.Margin = new System.Windows.Forms.Padding(2);
            this._filename.Mask = "";
            this._filename.Name = "_filename";
            this._filename.Size = new System.Drawing.Size(195, 41);
            this._filename.TabIndex = 0;
            this._filename.Value = null;
            // 
            // _browseButton
            // 
            this._browseButton.Location = new System.Drawing.Point(216, 33);
            this._browseButton.Name = "_browseButton";
            this._browseButton.Size = new System.Drawing.Size(75, 23);
            this._browseButton.TabIndex = 1;
            this._browseButton.Text = "Browse";
            this._browseButton.UseVisualStyleBackColor = true;
            this._browseButton.Click += new System.EventHandler(this._browseButton_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // _batchSize
            // 
            this._batchSize.LabelText = "Rows per batch import:";
            this._batchSize.Location = new System.Drawing.Point(16, 62);
            this._batchSize.Margin = new System.Windows.Forms.Padding(2);
            this._batchSize.Mask = "";
            this._batchSize.Name = "_batchSize";
            this._batchSize.Size = new System.Drawing.Size(195, 41);
            this._batchSize.TabIndex = 2;
            this._batchSize.Value = "0";
            // 
            // _startButton
            // 
            this._startButton.Location = new System.Drawing.Point(216, 80);
            this._startButton.Name = "_startButton";
            this._startButton.Size = new System.Drawing.Size(75, 23);
            this._startButton.TabIndex = 3;
            this._startButton.Text = "Start";
            this._startButton.UseVisualStyleBackColor = true;
            this._startButton.Click += new System.EventHandler(this._startButton_Click);
            // 
            // ImportDiagnosticServicesComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._startButton);
            this.Controls.Add(this._batchSize);
            this.Controls.Add(this._browseButton);
            this.Controls.Add(this._filename);
            this.Name = "ImportDiagnosticServicesComponentControl";
            this.Size = new System.Drawing.Size(308, 138);
            this.ResumeLayout(false);

        }

        #endregion

        private ClearCanvas.Controls.WinForms.TextField _filename;
        private System.Windows.Forms.Button _browseButton;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private ClearCanvas.Controls.WinForms.TextField _batchSize;
        private System.Windows.Forms.Button _startButton;
    }
}
