namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    partial class DowntimePrintFormsComponentControl
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
			this._printButton = new System.Windows.Forms.Button();
			this._numberOfForms = new System.Windows.Forms.NumericUpDown();
			this._formPreviewPanel = new System.Windows.Forms.Panel();
			this._cancelPrintingButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this._statusStrip = new System.Windows.Forms.StatusStrip();
			this._statusText = new System.Windows.Forms.ToolStripStatusLabel();
			this._progressBar = new System.Windows.Forms.ToolStripProgressBar();
			((System.ComponentModel.ISupportInitialize)(this._numberOfForms)).BeginInit();
			this.groupBox1.SuspendLayout();
			this._statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// _printButton
			// 
			this._printButton.Location = new System.Drawing.Point(183, 20);
			this._printButton.Name = "_printButton";
			this._printButton.Size = new System.Drawing.Size(75, 23);
			this._printButton.TabIndex = 2;
			this._printButton.Text = "Print";
			this._printButton.UseVisualStyleBackColor = true;
			this._printButton.Click += new System.EventHandler(this._printButton_Click);
			// 
			// _numberOfForms
			// 
			this._numberOfForms.Location = new System.Drawing.Point(100, 20);
			this._numberOfForms.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
			this._numberOfForms.Name = "_numberOfForms";
			this._numberOfForms.Size = new System.Drawing.Size(67, 20);
			this._numberOfForms.TabIndex = 1;
			this._numberOfForms.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			this._numberOfForms.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
			// 
			// _formPreviewPanel
			// 
			this._formPreviewPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._formPreviewPanel.Location = new System.Drawing.Point(3, 65);
			this._formPreviewPanel.Name = "_formPreviewPanel";
			this._formPreviewPanel.Size = new System.Drawing.Size(774, 663);
			this._formPreviewPanel.TabIndex = 1;
			// 
			// _cancelPrintingButton
			// 
			this._cancelPrintingButton.Location = new System.Drawing.Point(264, 20);
			this._cancelPrintingButton.Name = "_cancelPrintingButton";
			this._cancelPrintingButton.Size = new System.Drawing.Size(97, 23);
			this._cancelPrintingButton.TabIndex = 3;
			this._cancelPrintingButton.Text = "Cancel Printing";
			this._cancelPrintingButton.UseVisualStyleBackColor = true;
			this._cancelPrintingButton.Click += new System.EventHandler(this._cancelPrintingButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this._numberOfForms);
			this.groupBox1.Controls.Add(this._cancelPrintingButton);
			this.groupBox1.Controls.Add(this._printButton);
			this.groupBox1.Location = new System.Drawing.Point(0, 4);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(777, 55);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Print downtime forms";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(7, 23);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(87, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Number of Forms";
			// 
			// _statusStrip
			// 
			this._statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusText,
            this._progressBar});
			this._statusStrip.Location = new System.Drawing.Point(0, 731);
			this._statusStrip.Name = "_statusStrip";
			this._statusStrip.Size = new System.Drawing.Size(780, 22);
			this._statusStrip.TabIndex = 2;
			this._statusStrip.Text = "statusStrip1";
			// 
			// _statusText
			// 
			this._statusText.Name = "_statusText";
			this._statusText.Size = new System.Drawing.Size(513, 17);
			this._statusText.Spring = true;
			this._statusText.Text = "Select number of forms to print";
			this._statusText.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _progressBar
			// 
			this._progressBar.Name = "_progressBar";
			this._progressBar.Size = new System.Drawing.Size(250, 16);
			// 
			// DowntimePrintFormsComponentControl
			// 
			this.AcceptButton = this._printButton;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this._cancelPrintingButton;
			this.Controls.Add(this._statusStrip);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this._formPreviewPanel);
			this.Name = "DowntimePrintFormsComponentControl";
			this.Size = new System.Drawing.Size(780, 753);
			((System.ComponentModel.ISupportInitialize)(this._numberOfForms)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this._statusStrip.ResumeLayout(false);
			this._statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.Button _printButton;
		private System.Windows.Forms.NumericUpDown _numberOfForms;
		private System.Windows.Forms.Panel _formPreviewPanel;
		private System.Windows.Forms.Button _cancelPrintingButton;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.StatusStrip _statusStrip;
		private System.Windows.Forms.ToolStripStatusLabel _statusText;
		private System.Windows.Forms.ToolStripProgressBar _progressBar;

	}
}
