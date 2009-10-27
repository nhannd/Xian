namespace ClearCanvas.ImageViewer.Externals.View.WinForms {
	partial class ExternalPropertiesComponentControl {
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
			System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
			System.Windows.Forms.FlowLayoutPanel flowLayoutPanel2;
			this._btnCancel = new System.Windows.Forms.Button();
			this._btnOk = new System.Windows.Forms.Button();
			this._pnlExternalType = new System.Windows.Forms.Panel();
			this._cboExternalType = new System.Windows.Forms.ComboBox();
			this._lblExternalType = new System.Windows.Forms.Label();
			this._pnlClientArea = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this._pnlName = new System.Windows.Forms.Panel();
			this._txtName = new System.Windows.Forms.TextBox();
			this._lblName = new System.Windows.Forms.Label();
			flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
			flowLayoutPanel2 = new System.Windows.Forms.FlowLayoutPanel();
			flowLayoutPanel2.SuspendLayout();
			this._pnlExternalType.SuspendLayout();
			this.panel1.SuspendLayout();
			this._pnlName.SuspendLayout();
			this.SuspendLayout();
			// 
			// flowLayoutPanel1
			// 
			flowLayoutPanel1.AutoScroll = true;
			flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
			flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			flowLayoutPanel1.Name = "flowLayoutPanel1";
			flowLayoutPanel1.Size = new System.Drawing.Size(453, 334);
			flowLayoutPanel1.TabIndex = 2;
			// 
			// flowLayoutPanel2
			// 
			flowLayoutPanel2.Controls.Add(this._btnCancel);
			flowLayoutPanel2.Controls.Add(this._btnOk);
			flowLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
			flowLayoutPanel2.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			flowLayoutPanel2.Location = new System.Drawing.Point(0, 334);
			flowLayoutPanel2.Name = "flowLayoutPanel2";
			flowLayoutPanel2.Padding = new System.Windows.Forms.Padding(5);
			flowLayoutPanel2.Size = new System.Drawing.Size(453, 43);
			flowLayoutPanel2.TabIndex = 4;
			// 
			// _btnCancel
			// 
			this._btnCancel.Location = new System.Drawing.Point(365, 8);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 0;
			this._btnCancel.Text = "&Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _btnOk
			// 
			this._btnOk.Location = new System.Drawing.Point(284, 8);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
			this._btnOk.TabIndex = 1;
			this._btnOk.Text = "&Ok";
			this._btnOk.UseVisualStyleBackColor = true;
			this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
			// 
			// _pnlExternalType
			// 
			this._pnlExternalType.Controls.Add(this._cboExternalType);
			this._pnlExternalType.Controls.Add(this._lblExternalType);
			this._pnlExternalType.Dock = System.Windows.Forms.DockStyle.Top;
			this._pnlExternalType.Location = new System.Drawing.Point(0, 0);
			this._pnlExternalType.Margin = new System.Windows.Forms.Padding(5);
			this._pnlExternalType.Name = "_pnlExternalType";
			this._pnlExternalType.Padding = new System.Windows.Forms.Padding(10);
			this._pnlExternalType.Size = new System.Drawing.Size(453, 37);
			this._pnlExternalType.TabIndex = 0;
			this._pnlExternalType.Visible = false;
			// 
			// _cboExternalType
			// 
			this._cboExternalType.Dock = System.Windows.Forms.DockStyle.Fill;
			this._cboExternalType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this._cboExternalType.FormattingEnabled = true;
			this._cboExternalType.Location = new System.Drawing.Point(90, 10);
			this._cboExternalType.Name = "_cboExternalType";
			this._cboExternalType.Size = new System.Drawing.Size(353, 21);
			this._cboExternalType.TabIndex = 1;
			this._cboExternalType.SelectedIndexChanged += new System.EventHandler(this._cboExternalType_SelectedIndexChanged);
			// 
			// _lblExternalType
			// 
			this._lblExternalType.Dock = System.Windows.Forms.DockStyle.Left;
			this._lblExternalType.Location = new System.Drawing.Point(10, 10);
			this._lblExternalType.Name = "_lblExternalType";
			this._lblExternalType.Size = new System.Drawing.Size(80, 17);
			this._lblExternalType.TabIndex = 0;
			this._lblExternalType.Text = "External Type:";
			this._lblExternalType.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// _pnlClientArea
			// 
			this._pnlClientArea.Dock = System.Windows.Forms.DockStyle.Top;
			this._pnlClientArea.Location = new System.Drawing.Point(0, 74);
			this._pnlClientArea.Margin = new System.Windows.Forms.Padding(5);
			this._pnlClientArea.Name = "_pnlClientArea";
			this._pnlClientArea.Size = new System.Drawing.Size(453, 232);
			this._pnlClientArea.TabIndex = 1;
			// 
			// panel1
			// 
			this.panel1.AutoScroll = true;
			this.panel1.Controls.Add(this._pnlClientArea);
			this.panel1.Controls.Add(this._pnlName);
			this.panel1.Controls.Add(this._pnlExternalType);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(453, 334);
			this.panel1.TabIndex = 5;
			// 
			// _pnlName
			// 
			this._pnlName.Controls.Add(this._txtName);
			this._pnlName.Controls.Add(this._lblName);
			this._pnlName.Dock = System.Windows.Forms.DockStyle.Top;
			this._pnlName.Location = new System.Drawing.Point(0, 37);
			this._pnlName.Name = "_pnlName";
			this._pnlName.Padding = new System.Windows.Forms.Padding(10);
			this._pnlName.Size = new System.Drawing.Size(453, 37);
			this._pnlName.TabIndex = 2;
			// 
			// _txtName
			// 
			this._txtName.Dock = System.Windows.Forms.DockStyle.Fill;
			this._txtName.Location = new System.Drawing.Point(90, 10);
			this._txtName.Name = "_txtName";
			this._txtName.Size = new System.Drawing.Size(353, 20);
			this._txtName.TabIndex = 2;
			// 
			// _lblName
			// 
			this._lblName.Dock = System.Windows.Forms.DockStyle.Left;
			this._lblName.Location = new System.Drawing.Point(10, 10);
			this._lblName.Name = "_lblName";
			this._lblName.Size = new System.Drawing.Size(80, 17);
			this._lblName.TabIndex = 0;
			this._lblName.Text = "Name:";
			this._lblName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// ExternalPropertiesComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panel1);
			this.Controls.Add(flowLayoutPanel1);
			this.Controls.Add(flowLayoutPanel2);
			this.Name = "ExternalPropertiesComponentControl";
			this.Size = new System.Drawing.Size(453, 377);
			flowLayoutPanel2.ResumeLayout(false);
			this._pnlExternalType.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			this._pnlName.ResumeLayout(false);
			this._pnlName.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Panel _pnlExternalType;
		private System.Windows.Forms.Label _lblExternalType;
		private System.Windows.Forms.Panel _pnlClientArea;
		private System.Windows.Forms.ComboBox _cboExternalType;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnOk;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Panel _pnlName;
		private System.Windows.Forms.TextBox _txtName;
		private System.Windows.Forms.Label _lblName;
	}
}
