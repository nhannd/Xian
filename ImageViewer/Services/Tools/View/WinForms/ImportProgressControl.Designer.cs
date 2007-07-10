namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
	partial class ImportProgressControl
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
			this._statusLabel = new System.Windows.Forms.Label();
			this._processedCount = new System.Windows.Forms.Label();
			this._processedProgress = new System.Windows.Forms.ProgressBar();
			this._failedLabel = new System.Windows.Forms.Label();
			this._processedLabel = new System.Windows.Forms.Label();
			this._availableLabel = new System.Windows.Forms.Label();
			this._failedCount = new System.Windows.Forms.Label();
			this._statusMessage = new System.Windows.Forms.Label();
			this._availableCount = new System.Windows.Forms.Label();
			this._cancelButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// _statusLabel
			// 
			this._statusLabel.AutoSize = true;
			this._statusLabel.Location = new System.Drawing.Point(23, 15);
			this._statusLabel.Name = "_statusLabel";
			this._statusLabel.Size = new System.Drawing.Size(40, 13);
			this._statusLabel.TabIndex = 0;
			this._statusLabel.Text = "Status:";
			this._statusLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _processedCount
			// 
			this._processedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._processedCount.AutoEllipsis = true;
			this._processedCount.Location = new System.Drawing.Point(338, 47);
			this._processedCount.Name = "_processedCount";
			this._processedCount.Size = new System.Drawing.Size(40, 23);
			this._processedCount.TabIndex = 4;
			this._processedCount.Text = "0";
			this._processedCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _processedProgress
			// 
			this._processedProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._processedProgress.Location = new System.Drawing.Point(69, 48);
			this._processedProgress.Name = "_processedProgress";
			this._processedProgress.Size = new System.Drawing.Size(263, 20);
			this._processedProgress.TabIndex = 3;
			// 
			// _failedLabel
			// 
			this._failedLabel.AutoSize = true;
			this._failedLabel.Location = new System.Drawing.Point(25, 126);
			this._failedLabel.Name = "_failedLabel";
			this._failedLabel.Size = new System.Drawing.Size(38, 13);
			this._failedLabel.TabIndex = 7;
			this._failedLabel.Text = "Failed:";
			this._failedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _processedLabel
			// 
			this._processedLabel.AutoSize = true;
			this._processedLabel.Location = new System.Drawing.Point(3, 52);
			this._processedLabel.Name = "_processedLabel";
			this._processedLabel.Size = new System.Drawing.Size(60, 13);
			this._processedLabel.TabIndex = 2;
			this._processedLabel.Text = "Processed:";
			this._processedLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _availableLabel
			// 
			this._availableLabel.AutoSize = true;
			this._availableLabel.Location = new System.Drawing.Point(10, 92);
			this._availableLabel.Name = "_availableLabel";
			this._availableLabel.Size = new System.Drawing.Size(53, 13);
			this._availableLabel.TabIndex = 5;
			this._availableLabel.Text = "Available:";
			this._availableLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// _failedCount
			// 
			this._failedCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._failedCount.AutoEllipsis = true;
			this._failedCount.Location = new System.Drawing.Point(69, 126);
			this._failedCount.Name = "_failedCount";
			this._failedCount.Size = new System.Drawing.Size(85, 23);
			this._failedCount.TabIndex = 8;
			this._failedCount.Text = "0";
			// 
			// _statusMessage
			// 
			this._statusMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._statusMessage.AutoEllipsis = true;
			this._statusMessage.Location = new System.Drawing.Point(69, 15);
			this._statusMessage.Name = "_statusMessage";
			this._statusMessage.Size = new System.Drawing.Size(263, 23);
			this._statusMessage.TabIndex = 1;
			this._statusMessage.Text = "Pending ...";
			// 
			// _availableCount
			// 
			this._availableCount.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this._availableCount.AutoEllipsis = true;
			this._availableCount.Location = new System.Drawing.Point(69, 87);
			this._availableCount.Name = "_availableCount";
			this._availableCount.Size = new System.Drawing.Size(85, 23);
			this._availableCount.TabIndex = 6;
			this._availableCount.Text = "0";
			this._availableCount.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(257, 121);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 9;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this.OnCancelButtonClicked);
			// 
			// ImportProgressControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._availableCount);
			this.Controls.Add(this._statusMessage);
			this.Controls.Add(this._failedCount);
			this.Controls.Add(this._availableLabel);
			this.Controls.Add(this._processedLabel);
			this.Controls.Add(this._failedLabel);
			this.Controls.Add(this._processedProgress);
			this.Controls.Add(this._processedCount);
			this.Controls.Add(this._statusLabel);
			this.Name = "ImportProgressControl";
			this.Size = new System.Drawing.Size(385, 170);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label _statusLabel;
		private System.Windows.Forms.Label _processedCount;
		private System.Windows.Forms.ProgressBar _processedProgress;
		private System.Windows.Forms.Label _failedLabel;
		private System.Windows.Forms.Label _processedLabel;
		private System.Windows.Forms.Label _availableLabel;
		private System.Windows.Forms.Label _failedCount;
		private System.Windows.Forms.Label _statusMessage;
		private System.Windows.Forms.Label _availableCount;
		private System.Windows.Forms.Button _cancelButton;
	}
}
