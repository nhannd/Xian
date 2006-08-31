namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	partial class StudyBrowserControl
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
			this._titleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this._studyTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this.SuspendLayout();
			// 
			// _titleBar
			// 
			this._titleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._titleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._titleBar.Location = new System.Drawing.Point(0, 0);
			this._titleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._titleBar.Name = "_titleBar";
			this._titleBar.Size = new System.Drawing.Size(661, 26);
			this._titleBar.TabIndex = 1;
			this._titleBar.Text = "Search";
			// 
			// _studyTableView
			// 
			this._studyTableView.DataSource = null;
			this._studyTableView.Dock = System.Windows.Forms.DockStyle.Bottom;
			this._studyTableView.Location = new System.Drawing.Point(0, 228);
			this._studyTableView.MenuModel = null;
			this._studyTableView.Name = "_studyTableView";
			this._studyTableView.Size = new System.Drawing.Size(661, 373);
			this._studyTableView.TabIndex = 0;
			this._studyTableView.ToolbarModel = null;
			this._studyTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// StudyBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._titleBar);
			this.Controls.Add(this._studyTableView);
			this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
			this.Name = "StudyBrowserControl";
			this.Size = new System.Drawing.Size(661, 601);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _studyTableView;
		private Crownwood.DotNetMagic.Controls.TitleBar _titleBar;


	}
}
