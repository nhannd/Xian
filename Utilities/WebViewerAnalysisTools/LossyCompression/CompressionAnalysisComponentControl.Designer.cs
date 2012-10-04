namespace ClearCanvas.Utilities.WebViewerAnalysisTools.LossyCompression
{
    partial class CompressionAnalysisComponentControl
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
            this.LosslessBMPVSLossyBMP = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.DICOMCompressionQuality = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.JPEGCompressionQuality = new System.Windows.Forms.NumericUpDown();
            this.losslessBMPVsLossyJPEG = new System.Windows.Forms.Button();
            this.LosslessBMPvsLosslessJPEG = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.DICOMCompressionQuality)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.JPEGCompressionQuality)).BeginInit();
            this.SuspendLayout();
            // 
            // LosslessBMPVSLossyBMP
            // 
            this.LosslessBMPVSLossyBMP.Location = new System.Drawing.Point(20, 64);
            this.LosslessBMPVSLossyBMP.Name = "LosslessBMPVSLossyBMP";
            this.LosslessBMPVSLossyBMP.Size = new System.Drawing.Size(190, 72);
            this.LosslessBMPVSLossyBMP.TabIndex = 0;
            this.LosslessBMPVSLossyBMP.Text = "Lossless (BMP) vs Lossy (BMP)\r\n\r\n\"Dicom Compression Loss\"";
            this.LosslessBMPVSLossyBMP.UseVisualStyleBackColor = true;
            this.LosslessBMPVSLossyBMP.Click += new System.EventHandler(this.LosslessBMPVSLossyBMP_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(116, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "J2K Compression Ratio";
            // 
            // DICOMCompressionQuality
            // 
            this.DICOMCompressionQuality.Location = new System.Drawing.Point(146, 15);
            this.DICOMCompressionQuality.Name = "DICOMCompressionQuality";
            this.DICOMCompressionQuality.Size = new System.Drawing.Size(53, 20);
            this.DICOMCompressionQuality.TabIndex = 2;
            this.DICOMCompressionQuality.Value = new decimal(new int[] {
            8,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(285, 17);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(69, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "JPEG Quality";
            // 
            // JPEGCompressionQuality
            // 
            this.JPEGCompressionQuality.Location = new System.Drawing.Point(360, 15);
            this.JPEGCompressionQuality.Name = "JPEGCompressionQuality";
            this.JPEGCompressionQuality.Size = new System.Drawing.Size(53, 20);
            this.JPEGCompressionQuality.TabIndex = 2;
            this.JPEGCompressionQuality.Value = new decimal(new int[] {
            80,
            0,
            0,
            0});
            // 
            // losslessBMPVsLossyJPEG
            // 
            this.losslessBMPVsLossyJPEG.Location = new System.Drawing.Point(451, 64);
            this.losslessBMPVsLossyJPEG.Name = "losslessBMPVsLossyJPEG";
            this.losslessBMPVsLossyJPEG.Size = new System.Drawing.Size(185, 72);
            this.losslessBMPVsLossyJPEG.TabIndex = 0;
            this.losslessBMPVsLossyJPEG.Text = "Lossless (BMP) vs Lossy (JPEG)\r\n\r\n\"DICOM + JPEG Compression Losses\"";
            this.losslessBMPVsLossyJPEG.UseVisualStyleBackColor = true;
            this.losslessBMPVsLossyJPEG.Click += new System.EventHandler(this.losslessBMPVsLossyJPEG_Click);
            // 
            // LosslessBMPvsLosslessJPEG
            // 
            this.LosslessBMPvsLosslessJPEG.Location = new System.Drawing.Point(233, 64);
            this.LosslessBMPvsLosslessJPEG.Name = "LosslessBMPvsLosslessJPEG";
            this.LosslessBMPvsLosslessJPEG.Size = new System.Drawing.Size(182, 72);
            this.LosslessBMPvsLosslessJPEG.TabIndex = 0;
            this.LosslessBMPvsLosslessJPEG.Text = "Lossless (BMP) vs Lossless (JPEG)\r\n\r\n\"JPEG Compression Loss\"";
            this.LosslessBMPvsLosslessJPEG.UseVisualStyleBackColor = true;
            this.LosslessBMPvsLosslessJPEG.Click += new System.EventHandler(this.LosslessBMPvsLosslessJPEG_Click);
            // 
            // CompressionAnalysisComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.JPEGCompressionQuality);
            this.Controls.Add(this.DICOMCompressionQuality);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.LosslessBMPvsLosslessJPEG);
            this.Controls.Add(this.losslessBMPVsLossyJPEG);
            this.Controls.Add(this.LosslessBMPVSLossyBMP);
            this.Name = "CompressionAnalysisComponentControl";
            this.Size = new System.Drawing.Size(673, 168);
            ((System.ComponentModel.ISupportInitialize)(this.DICOMCompressionQuality)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.JPEGCompressionQuality)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button LosslessBMPVSLossyBMP;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown DICOMCompressionQuality;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown JPEGCompressionQuality;
        private System.Windows.Forms.Button losslessBMPVsLossyJPEG;
        private System.Windows.Forms.Button LosslessBMPvsLosslessJPEG;
    }
}
