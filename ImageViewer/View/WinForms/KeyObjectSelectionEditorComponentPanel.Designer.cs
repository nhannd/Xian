namespace ClearCanvas.ImageViewer.View.WinForms
{
	partial class KeyObjectSelectionEditorComponentPanel {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(KeyObjectSelectionEditorComponentPanel));
			this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			this.pnlTitle = new System.Windows.Forms.Panel();
			this.cboTitle = new System.Windows.Forms.ComboBox();
			this.lblTitle = new System.Windows.Forms.Label();
			this.pnlDesc = new System.Windows.Forms.Panel();
			this.txtDesc = new System.Windows.Forms.TextBox();
			this.lblDesc = new System.Windows.Forms.Label();
			this.pnlDateTime = new System.Windows.Forms.Panel();
			this.txtDateTime = new System.Windows.Forms.TextBox();
			this.lblDateTime = new System.Windows.Forms.Label();
			this.pnlSeriesDesc = new System.Windows.Forms.Panel();
			this.txtSeriesDesc = new System.Windows.Forms.TextBox();
			this.lblSeriesDesc = new System.Windows.Forms.Label();
			this.pnlSeriesNum = new System.Windows.Forms.Panel();
			this.txtSeriesNum = new System.Windows.Forms.TextBox();
			this.lblSeriesNum = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.flowLayoutPanel1.SuspendLayout();
			this.pnlTitle.SuspendLayout();
			this.pnlDesc.SuspendLayout();
			this.pnlDateTime.SuspendLayout();
			this.pnlSeriesDesc.SuspendLayout();
			this.pnlSeriesNum.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			this.flowLayoutPanel1.Controls.Add(this.pnlTitle);
			this.flowLayoutPanel1.Controls.Add(this.pnlDesc);
			this.flowLayoutPanel1.Controls.Add(this.pnlDateTime);
			this.flowLayoutPanel1.Controls.Add(this.pnlSeriesDesc);
			this.flowLayoutPanel1.Controls.Add(this.pnlSeriesNum);
			this.flowLayoutPanel1.Controls.Add(this.label1);
			this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel1.Name = "flowLayoutPanel1";
			this.flowLayoutPanel1.Padding = new System.Windows.Forms.Padding(10);
			this.flowLayoutPanel1.Size = new System.Drawing.Size(312, 447);
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
			// pnlDateTime
			// 
			this.pnlDateTime.Controls.Add(this.txtDateTime);
			this.pnlDateTime.Controls.Add(this.lblDateTime);
			this.pnlDateTime.Location = new System.Drawing.Point(13, 157);
			this.pnlDateTime.Name = "pnlDateTime";
			this.pnlDateTime.Size = new System.Drawing.Size(285, 45);
			this.pnlDateTime.TabIndex = 7;
			// 
			// txtDateTime
			// 
			this.txtDateTime.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtDateTime.Location = new System.Drawing.Point(0, 18);
			this.txtDateTime.Name = "txtDateTime";
			this.txtDateTime.ReadOnly = true;
			this.txtDateTime.Size = new System.Drawing.Size(285, 20);
			this.txtDateTime.TabIndex = 4;
			this.txtDateTime.Text = "(Use Current Time)";
			// 
			// lblDateTime
			// 
			this.lblDateTime.AutoSize = true;
			this.lblDateTime.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblDateTime.Location = new System.Drawing.Point(0, 0);
			this.lblDateTime.Name = "lblDateTime";
			this.lblDateTime.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.lblDateTime.Size = new System.Drawing.Size(113, 18);
			this.lblDateTime.TabIndex = 0;
			this.lblDateTime.Text = "Document Date/Time:";
			// 
			// pnlSeriesDesc
			// 
			this.pnlSeriesDesc.Controls.Add(this.txtSeriesDesc);
			this.pnlSeriesDesc.Controls.Add(this.lblSeriesDesc);
			this.pnlSeriesDesc.Location = new System.Drawing.Point(13, 208);
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
			this.txtSeriesDesc.TabIndex = 3;
			// 
			// lblSeriesDesc
			// 
			this.lblSeriesDesc.AutoSize = true;
			this.lblSeriesDesc.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblSeriesDesc.Location = new System.Drawing.Point(0, 0);
			this.lblSeriesDesc.Name = "lblSeriesDesc";
			this.lblSeriesDesc.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.lblSeriesDesc.Size = new System.Drawing.Size(95, 18);
			this.lblSeriesDesc.TabIndex = 2;
			this.lblSeriesDesc.Text = "Series Description:";
			// 
			// pnlSeriesNum
			// 
			this.pnlSeriesNum.Controls.Add(this.txtSeriesNum);
			this.pnlSeriesNum.Controls.Add(this.lblSeriesNum);
			this.pnlSeriesNum.Location = new System.Drawing.Point(13, 258);
			this.pnlSeriesNum.Name = "pnlSeriesNum";
			this.pnlSeriesNum.Size = new System.Drawing.Size(285, 44);
			this.pnlSeriesNum.TabIndex = 6;
			// 
			// txtSeriesNum
			// 
			this.txtSeriesNum.Dock = System.Windows.Forms.DockStyle.Top;
			this.txtSeriesNum.Location = new System.Drawing.Point(0, 18);
			this.txtSeriesNum.Name = "txtSeriesNum";
			this.txtSeriesNum.Size = new System.Drawing.Size(285, 20);
			this.txtSeriesNum.TabIndex = 3;
			this.txtSeriesNum.Validating += new System.ComponentModel.CancelEventHandler(this.txtSeriesNum_Validating);
			// 
			// lblSeriesNum
			// 
			this.lblSeriesNum.AutoSize = true;
			this.lblSeriesNum.Dock = System.Windows.Forms.DockStyle.Top;
			this.lblSeriesNum.Location = new System.Drawing.Point(0, 0);
			this.lblSeriesNum.Name = "lblSeriesNum";
			this.lblSeriesNum.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
			this.lblSeriesNum.Size = new System.Drawing.Size(79, 18);
			this.lblSeriesNum.TabIndex = 2;
			this.lblSeriesNum.Text = "Series Number:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(13, 305);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(281, 78);
			this.label1.TabIndex = 8;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// KeyObjectSelectionEditorComponentPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.flowLayoutPanel1);
			this.Name = "KeyObjectSelectionEditorComponentPanel";
			this.Size = new System.Drawing.Size(312, 447);
			this.flowLayoutPanel1.ResumeLayout(false);
			this.flowLayoutPanel1.PerformLayout();
			this.pnlTitle.ResumeLayout(false);
			this.pnlTitle.PerformLayout();
			this.pnlDesc.ResumeLayout(false);
			this.pnlDesc.PerformLayout();
			this.pnlDateTime.ResumeLayout(false);
			this.pnlDateTime.PerformLayout();
			this.pnlSeriesDesc.ResumeLayout(false);
			this.pnlSeriesDesc.PerformLayout();
			this.pnlSeriesNum.ResumeLayout(false);
			this.pnlSeriesNum.PerformLayout();
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
		private System.Windows.Forms.Panel pnlSeriesNum;
		private System.Windows.Forms.TextBox txtSeriesNum;
		private System.Windows.Forms.Label lblSeriesNum;
		private System.Windows.Forms.Panel pnlDateTime;
		private System.Windows.Forms.Label lblDateTime;
		private System.Windows.Forms.TextBox txtDateTime;
		private System.Windows.Forms.Label label1;
	}
}