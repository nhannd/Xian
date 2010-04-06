namespace ClearCanvas.ImageViewer.Configuration.View.WinForms {
	partial class CustomizeViewerActionModelsComponentControl {
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
			this._pnlButtons = new System.Windows.Forms.FlowLayoutPanel();
			this._btnApply = new System.Windows.Forms.Button();
			this._btnCancel = new System.Windows.Forms.Button();
			this._btnOk = new System.Windows.Forms.Button();
			this._pnlMain = new System.Windows.Forms.Panel();
			this._pnlButtons.SuspendLayout();
			this.SuspendLayout();
			// 
			// _pnlButtons
			// 
			this._pnlButtons.Controls.Add(this._btnApply);
			this._pnlButtons.Controls.Add(this._btnCancel);
			this._pnlButtons.Controls.Add(this._btnOk);
			this._pnlButtons.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._pnlButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this._pnlButtons.Location = new System.Drawing.Point(0, 365);
			this._pnlButtons.Name = "_pnlButtons";
			this._pnlButtons.Padding = new System.Windows.Forms.Padding(3);
			this._pnlButtons.Size = new System.Drawing.Size(371, 36);
			this._pnlButtons.TabIndex = 1;
			// 
			// _btnApply
			// 
			this._btnApply.Location = new System.Drawing.Point(287, 6);
			this._btnApply.Name = "_btnApply";
			this._btnApply.Size = new System.Drawing.Size(75, 23);
			this._btnApply.TabIndex = 2;
			this._btnApply.Text = "&Apply";
			this._btnApply.UseVisualStyleBackColor = true;
			this._btnApply.Click += new System.EventHandler(this._btnApply_Click);
			// 
			// _btnCancel
			// 
			this._btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this._btnCancel.Location = new System.Drawing.Point(206, 6);
			this._btnCancel.Name = "_btnCancel";
			this._btnCancel.Size = new System.Drawing.Size(75, 23);
			this._btnCancel.TabIndex = 1;
			this._btnCancel.Text = "&Cancel";
			this._btnCancel.UseVisualStyleBackColor = true;
			this._btnCancel.Click += new System.EventHandler(this._btnCancel_Click);
			// 
			// _btnOk
			// 
			this._btnOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this._btnOk.Location = new System.Drawing.Point(125, 6);
			this._btnOk.Name = "_btnOk";
			this._btnOk.Size = new System.Drawing.Size(75, 23);
			this._btnOk.TabIndex = 0;
			this._btnOk.Text = "&Ok";
			this._btnOk.UseVisualStyleBackColor = true;
			this._btnOk.Click += new System.EventHandler(this._btnOk_Click);
			// 
			// _pnlMain
			// 
			this._pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
			this._pnlMain.Location = new System.Drawing.Point(0, 0);
			this._pnlMain.Name = "_pnlMain";
			this._pnlMain.Size = new System.Drawing.Size(371, 365);
			this._pnlMain.TabIndex = 0;
			// 
			// CustomizeViewerActionModelsComponentControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._pnlMain);
			this.Controls.Add(this._pnlButtons);
			this.Name = "CustomizeViewerActionModelsComponentControl";
			this.Size = new System.Drawing.Size(371, 401);
			this._pnlButtons.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.FlowLayoutPanel _pnlButtons;
		private System.Windows.Forms.Panel _pnlMain;
		private System.Windows.Forms.Button _btnApply;
		private System.Windows.Forms.Button _btnCancel;
		private System.Windows.Forms.Button _btnOk;
	}
}
