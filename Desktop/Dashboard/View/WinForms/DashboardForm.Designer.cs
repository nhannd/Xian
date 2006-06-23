namespace ClearCanvas.Desktop.Dashboard
{
	partial class DashboardForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._splitContainer1 = new System.Windows.Forms.SplitContainer();
			this._outlookSidebar = new ClearCanvas.Controls.WinForms.OutlookSidebar();
			this._splitContainer1.Panel1.SuspendLayout();
			this._splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// _splitContainer1
			// 
			this._splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this._splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this._splitContainer1.Location = new System.Drawing.Point(0, 0);
			this._splitContainer1.Name = "_splitContainer1";
			// 
			// _splitContainer1.Panel1
			// 
			this._splitContainer1.Panel1.Controls.Add(this._outlookSidebar);
			this._splitContainer1.Panel1.Padding = new System.Windows.Forms.Padding(3, 3, 0, 3);
			// 
			// _splitContainer1.Panel2
			// 
			this._splitContainer1.Panel2.Padding = new System.Windows.Forms.Padding(0, 3, 3, 3);
			this._splitContainer1.Size = new System.Drawing.Size(732, 476);
			this._splitContainer1.SplitterDistance = 224;
			this._splitContainer1.TabIndex = 0;
			// 
			// _outlookSidebar
			// 
			this._outlookSidebar.BackColor = System.Drawing.Color.Transparent;
			this._outlookSidebar.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this._outlookSidebar.Dock = System.Windows.Forms.DockStyle.Fill;
			this._outlookSidebar.Location = new System.Drawing.Point(3, 3);
			this._outlookSidebar.MainHeaderText = "";
			this._outlookSidebar.Name = "_outlookSidebar";
			this._outlookSidebar.Size = new System.Drawing.Size(221, 470);
			this._outlookSidebar.SubHeaderText = "";
			this._outlookSidebar.TabIndex = 0;
			this._outlookSidebar.TabStop = false;
			// 
			// DashboardForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._splitContainer1);
			this.Name = "DashboardForm";
			this.Size = new System.Drawing.Size(732, 476);
			this._splitContainer1.Panel1.ResumeLayout(false);
			this._splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer _splitContainer1;
		private ClearCanvas.Controls.WinForms.OutlookSidebar _outlookSidebar;
	}
}