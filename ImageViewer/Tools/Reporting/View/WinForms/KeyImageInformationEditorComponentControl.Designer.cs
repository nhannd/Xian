namespace ClearCanvas.ImageViewer.Tools.Reporting.View.WinForms
{
	partial class KeyImageInformationEditorComponentControl {
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.pnlTitle = new System.Windows.Forms.Panel();
			this.cboTitle = new System.Windows.Forms.ComboBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.pnlDesc = new System.Windows.Forms.Panel();
			this.txtDesc = new System.Windows.Forms.TextBox();
			this.lblDesc = new System.Windows.Forms.Label();
			this.pnlSeriesDesc = new System.Windows.Forms.Panel();
			this.txtSeriesDesc = new System.Windows.Forms.TextBox();
			this.lblSeriesDesc = new System.Windows.Forms.Label();
			this._cancelButton = new System.Windows.Forms.Button();
			this._okButton = new System.Windows.Forms.Button();
			this.flowLayoutPanel1.SuspendLayout();
			this.pnlTitle.SuspendLayout();
			this.pnlDesc.SuspendLayout();
			this.pnlSeriesDesc.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.flowLayoutPanel1.Controls.Add(this.pnlTitle);
			this.flowLayoutPanel1.Controls.Add(this.pnlDesc);
			this.flowLayoutPanel1.Controls.Add(this.pnlSeriesDesc);
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
			this.flowLayoutPanel1.Size = new System.Drawing.Size(312, 216);
			this.flowLayoutPanel1.TabIndex = 0;
			// 
			// pnlTitle
			// 
			this.pnlTitle.Controls.Add(this.cboTitle);
			this.pnlTitle.Controls.Add(this.lblTitle);
			this.pnlTitle.Location = new System.Drawing.Point(13, 13);
			this.pnlTitle.Name = "pnlTitle";
			this.pnlTitle.Size = new System.Drawing.Size(285, 44);
			this.pnlTitle.TabIndex = 3;
			// 
			// cboTitle
			// 
			this.cboTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.cboTitle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cboTitle.FormattingEnabled = true;
			this.cboTitle.Location = new System.Drawing.Point(0, 18);
			this.cboTitle.Name = "cboTitle";
			this.cboTitle.Size = new System.Drawing.Size(285, 21);
			this.cboTitle.TabIndex = 1;
			// 
			// lblTitle
			// 
			this.lblTitle.AutoSize = true;
			this.lblTitle.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblTitle.Location = new System.Drawing.Point(0, 0);
			this.lblTitle.Name = "lblTitle";
			this.lblTitle.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.lblTitle.Size = new System.Drawing.Size(82, 18);
			this.lblTitle.TabIndex = 0;
			this.lblTitle.Text = "Document Title:";
			// 
			// pnlDesc
			// 
			this.pnlDesc.Controls.Add(this.txtDesc);
			this.pnlDesc.Controls.Add(this.lblDesc);
			this.pnlDesc.Location = new System.Drawing.Point(13, 63);
			this.pnlDesc.Name = "pnlDesc";
			this.pnlDesc.Size = new System.Drawing.Size(285, 88);
			this.pnlDesc.TabIndex = 4;
			// 
			// txtDesc
			// 
			this.txtDesc.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtDesc.Location = new System.Drawing.Point(0, 18);
			this.txtDesc.Multiline = true;
			this.txtDesc.Name = "txtDesc";
			this.txtDesc.Size = new System.Drawing.Size(285, 70);
			this.txtDesc.TabIndex = 3;
			// 
			// lblDesc
			// 
			this.lblDesc.AutoSize = true;
			this.lblDesc.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDesc.Location = new System.Drawing.Point(0, 0);
			this.lblDesc.Name = "lblDesc";
			this.lblDesc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.lblDesc.Size = new System.Drawing.Size(63, 18);
			this.lblDesc.TabIndex = 2;
			this.lblDesc.Text = "Description:";
			// 
			// pnlSeriesDesc
			// 
			this.pnlSeriesDesc.Controls.Add(this.txtSeriesDesc);
			this.pnlSeriesDesc.Controls.Add(this.lblSeriesDesc);
			this.pnlSeriesDesc.Location = new System.Drawing.Point(13, 157);
			this.pnlSeriesDesc.Name = "pnlSeriesDesc";
			this.pnlSeriesDesc.Size = new System.Drawing.Size(285, 44);
			this.pnlSeriesDesc.TabIndex = 5;
			// 
			// txtSeriesDesc
			// 
			this.txtSeriesDesc.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtSeriesDesc.Location = new System.Drawing.Point(0, 18);
			this.txtSeriesDesc.Name = "txtSeriesDesc";
			this.txtSeriesDesc.Size = new System.Drawing.Size(285, 20);
			this.txtSeriesDesc.TabIndex = 5;
			// 
			// lblSeriesDesc
			// 
			this.lblSeriesDesc.AutoSize = true;
			this.lblSeriesDesc.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblSeriesDesc.Location = new System.Drawing.Point(0, 0);
			this.lblSeriesDesc.Name = "lblSeriesDesc";
			this.lblSeriesDesc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.lblSeriesDesc.Size = new System.Drawing.Size(95, 18);
			this.lblSeriesDesc.TabIndex = 4;
			this.lblSeriesDesc.Text = "Series Description:";
			// 
			// _cancelButton
			// 
			this._cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._cancelButton.Location = new System.Drawing.Point(224, 223);
			this._cancelButton.Name = "_cancelButton";
			this._cancelButton.Size = new System.Drawing.Size(75, 23);
			this._cancelButton.TabIndex = 7;
			this._cancelButton.Text = "Cancel";
			this._cancelButton.UseVisualStyleBackColor = true;
			this._cancelButton.Click += new System.EventHandler(this.OnCancel);
			// 
			// _okButton
			// 
			this._okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this._okButton.Location = new System.Drawing.Point(143, 223);
			this._okButton.Name = "_okButton";
			this._okButton.Size = new System.Drawing.Size(75, 23);
			this._okButton.TabIndex = 6;
			this._okButton.Text = "OK";
			this._okButton.UseVisualStyleBackColor = true;
			this._okButton.Click += new System.EventHandler(this.OnOk);
			// 
			// KeyImageInformationEditorComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._cancelButton);
			this.Controls.Add(this._okButton);
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "KeyImageInformationEditorComponentControl";
			this.Size = new System.Drawing.Size(312, 260);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.pnlTitle.ResumeLayout(false);
			this.pnlTitle.PerformLayout();
			this.pnlDesc.ResumeLayout(false);
			this.pnlDesc.PerformLayout();
			this.pnlSeriesDesc.ResumeLayout(false);
			this.pnlSeriesDesc.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
		private System.Windows.Forms.Panel pnlTitle;
		private System.Windows.Forms.ComboBox cboTitle;
		private System.Windows.Forms.Label lblTitle;
		private System.Windows.Forms.Panel pnlDesc;
		private System.Windows.Forms.TextBox txtDesc;
		private System.Windows.Forms.Label lblDesc;
		private System.Windows.Forms.Panel pnlSeriesDesc;
		private System.Windows.Forms.TextBox txtSeriesDesc;
		private System.Windows.Forms.Label lblSeriesDesc;
		private System.Windows.Forms.Button _cancelButton;
		private System.Windows.Forms.Button _okButton;
	}
}