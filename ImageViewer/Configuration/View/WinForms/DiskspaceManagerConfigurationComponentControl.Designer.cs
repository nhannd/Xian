namespace ClearCanvas.ImageViewer.Configuration.View.WinForms
{
    partial class DiskspaceManagerConfigurationComponentControl
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
            this._tbLowWatermark = new System.Windows.Forms.TrackBar();
            this._tbHighWatermark = new System.Windows.Forms.TrackBar();
            this._pbUsedSpace = new System.Windows.Forms.ProgressBar();
            this._txtHighWatermark = new System.Windows.Forms.TextBox();
            this._txtUsedSpace = new System.Windows.Forms.TextBox();
            this._txtLowWatermark = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this._txtStatus = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this._lbDriveName = new System.Windows.Forms.ListBox();
            this.label9 = new System.Windows.Forms.Label();
            this._tbFrequency = new System.Windows.Forms.TrackBar();
            this._txtFrequency = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this._btRefresh = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this._tbLowWatermark)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._tbHighWatermark)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._tbFrequency)).BeginInit();
            this.SuspendLayout();
            // 
            // _tbLowWatermark
            // 
            this._tbLowWatermark.Location = new System.Drawing.Point(95, 177);
            this._tbLowWatermark.Maximum = 10000;
            this._tbLowWatermark.Name = "_tbLowWatermark";
            this._tbLowWatermark.Size = new System.Drawing.Size(267, 53);
            this._tbLowWatermark.TabIndex = 0;
            this._tbLowWatermark.TickFrequency = 1000;
            this._tbLowWatermark.TickStyle = System.Windows.Forms.TickStyle.TopLeft;
            // 
            // _tbHighWatermark
            // 
            this._tbHighWatermark.Location = new System.Drawing.Point(95, 76);
            this._tbHighWatermark.Maximum = 10000;
            this._tbHighWatermark.Name = "_tbHighWatermark";
            this._tbHighWatermark.Size = new System.Drawing.Size(267, 53);
            this._tbHighWatermark.TabIndex = 1;
            this._tbHighWatermark.TickFrequency = 1000;
            // 
            // _pbUsedSpace
            // 
            this._pbUsedSpace.Location = new System.Drawing.Point(104, 135);
            this._pbUsedSpace.Name = "_pbUsedSpace";
            this._pbUsedSpace.Size = new System.Drawing.Size(246, 23);
            this._pbUsedSpace.TabIndex = 2;
            // 
            // _txtHighWatermark
            // 
            this._txtHighWatermark.Location = new System.Drawing.Point(368, 95);
            this._txtHighWatermark.Name = "_txtHighWatermark";
            this._txtHighWatermark.Size = new System.Drawing.Size(57, 22);
            this._txtHighWatermark.TabIndex = 3;
            // 
            // _txtUsedSpace
            // 
            this._txtUsedSpace.Location = new System.Drawing.Point(368, 136);
            this._txtUsedSpace.Name = "_txtUsedSpace";
            this._txtUsedSpace.ReadOnly = true;
            this._txtUsedSpace.Size = new System.Drawing.Size(57, 22);
            this._txtUsedSpace.TabIndex = 4;
            // 
            // _txtLowWatermark
            // 
            this._txtLowWatermark.Location = new System.Drawing.Point(368, 177);
            this._txtLowWatermark.Name = "_txtLowWatermark";
            this._txtLowWatermark.Size = new System.Drawing.Size(57, 22);
            this._txtLowWatermark.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 81);
            this.label1.MaximumSize = new System.Drawing.Size(80, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 34);
            this.label1.TabIndex = 6;
            this.label1.Text = "High Watermark";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(32, 130);
            this.label2.MaximumSize = new System.Drawing.Size(60, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(48, 34);
            this.label2.TabIndex = 7;
            this.label2.Text = "Used Space";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(15, 180);
            this.label3.MaximumSize = new System.Drawing.Size(80, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 34);
            this.label3.TabIndex = 8;
            this.label3.Text = "Low Watermark";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(431, 98);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 17);
            this.label4.TabIndex = 9;
            this.label4.Text = "%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(431, 139);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(20, 17);
            this.label5.TabIndex = 10;
            this.label5.Text = "%";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(431, 180);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(20, 17);
            this.label6.TabIndex = 11;
            this.label6.Text = "%";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(32, 289);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(48, 17);
            this.label7.TabIndex = 12;
            this.label7.Text = "Status";
            // 
            // _txtStatus
            // 
            this._txtStatus.Location = new System.Drawing.Point(104, 286);
            this._txtStatus.Name = "_txtStatus";
            this._txtStatus.ReadOnly = true;
            this._txtStatus.Size = new System.Drawing.Size(321, 22);
            this._txtStatus.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(51, 44);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(41, 17);
            this.label8.TabIndex = 14;
            this.label8.Text = "Drive";
            // 
            // _lbDriveName
            // 
            this._lbDriveName.FormattingEnabled = true;
            this._lbDriveName.ItemHeight = 16;
            this._lbDriveName.Location = new System.Drawing.Point(104, 44);
            this._lbDriveName.Name = "_lbDriveName";
            this._lbDriveName.Size = new System.Drawing.Size(65, 20);
            this._lbDriveName.TabIndex = 15;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(17, 230);
            this.label9.MaximumSize = new System.Drawing.Size(80, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(75, 34);
            this.label9.TabIndex = 16;
            this.label9.Text = "Check Frequency";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // _tbFrequency
            // 
            this._tbFrequency.Location = new System.Drawing.Point(95, 227);
            this._tbFrequency.Maximum = 30;
            this._tbFrequency.Minimum = 1;
            this._tbFrequency.Name = "_tbFrequency";
            this._tbFrequency.Size = new System.Drawing.Size(267, 53);
            this._tbFrequency.TabIndex = 17;
            this._tbFrequency.TickFrequency = 3;
            this._tbFrequency.Value = 1;
            // 
            // _txtFrequency
            // 
            this._txtFrequency.Location = new System.Drawing.Point(369, 227);
            this._txtFrequency.Name = "_txtFrequency";
            this._txtFrequency.ReadOnly = true;
            this._txtFrequency.Size = new System.Drawing.Size(56, 22);
            this._txtFrequency.TabIndex = 18;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(431, 230);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(30, 17);
            this.label10.TabIndex = 19;
            this.label10.Text = "Min";
            // 
            // _btRefresh
            // 
            this._btRefresh.Location = new System.Drawing.Point(35, 334);
            this._btRefresh.Name = "_btRefresh";
            this._btRefresh.Size = new System.Drawing.Size(75, 28);
            this._btRefresh.TabIndex = 20;
            this._btRefresh.Text = "Refresh";
            this._btRefresh.UseVisualStyleBackColor = true;
            // 
            // DiskspaceManagerConfigurationComponentControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this._btRefresh);
            this.Controls.Add(this.label10);
            this.Controls.Add(this._txtFrequency);
            this.Controls.Add(this._tbFrequency);
            this.Controls.Add(this.label9);
            this.Controls.Add(this._lbDriveName);
            this.Controls.Add(this.label8);
            this.Controls.Add(this._txtStatus);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this._txtLowWatermark);
            this.Controls.Add(this._txtUsedSpace);
            this.Controls.Add(this._txtHighWatermark);
            this.Controls.Add(this._pbUsedSpace);
            this.Controls.Add(this._tbHighWatermark);
            this.Controls.Add(this._tbLowWatermark);
            this.Name = "DiskspaceManagerConfigurationComponentControl";
            this.Size = new System.Drawing.Size(492, 383);
            ((System.ComponentModel.ISupportInitialize)(this._tbLowWatermark)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._tbHighWatermark)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._tbFrequency)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar _tbLowWatermark;
        private System.Windows.Forms.TrackBar _tbHighWatermark;
        private System.Windows.Forms.ProgressBar _pbUsedSpace;
        private System.Windows.Forms.TextBox _txtHighWatermark;
        private System.Windows.Forms.TextBox _txtUsedSpace;
        private System.Windows.Forms.TextBox _txtLowWatermark;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox _txtStatus;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListBox _lbDriveName;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TrackBar _tbFrequency;
        private System.Windows.Forms.TextBox _txtFrequency;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button _btRefresh;
    }
}
