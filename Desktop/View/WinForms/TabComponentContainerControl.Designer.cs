namespace ClearCanvas.Desktop.View.WinForms
{
	partial class TabComponentContainerControl
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
			this._tabControl = new Crownwood.DotNetMagic.Controls.TabControl();
			this.SuspendLayout();
			// 
			// _tabControl
			// 
			this._tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tabControl.Location = new System.Drawing.Point(4, 4);
			this._tabControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this._tabControl.MaximumHeaderWidth = 256;
			this._tabControl.Name = "_tabControl";
			this._tabControl.OfficeDockSides = false;
			this._tabControl.OfficeExtraTabInset = 3;
			this._tabControl.OfficeStyle = Crownwood.DotNetMagic.Controls.OfficeStyle.SoftWhite;
			this._tabControl.PositionTop = true;
			this._tabControl.ShowDropSelect = false;
			this._tabControl.Size = new System.Drawing.Size(1029, 550);
			this._tabControl.Style = Crownwood.DotNetMagic.Common.VisualStyle.IDE2005;
			this._tabControl.TabIndex = 0;
			this._tabControl.TextTips = true;
			// 
			// TabComponentContainerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this._tabControl);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "TabComponentContainerControl";
			this.Padding = new System.Windows.Forms.Padding(4, 4, 0, 4);
			this.Size = new System.Drawing.Size(1033, 558);
			this.ResumeLayout(false);

		}

		#endregion

		private Crownwood.DotNetMagic.Controls.TabControl _tabControl;




	}
}
