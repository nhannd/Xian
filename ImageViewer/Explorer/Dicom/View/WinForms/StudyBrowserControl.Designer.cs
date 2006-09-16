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
			this._studyTableView = new ClearCanvas.Desktop.View.WinForms.TableView();
			this._resultsTitleBar = new Crownwood.DotNetMagic.Controls.TitleBar();
			this.SuspendLayout();
			// 
			// _studyTableView
			// 
			this._studyTableView.DataSource = null;
			this._studyTableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._studyTableView.Location = new System.Drawing.Point(0, 23);
			this._studyTableView.MenuModel = null;
			this._studyTableView.Name = "_studyTableView";
			this._studyTableView.ReadOnly = false;
			this._studyTableView.Size = new System.Drawing.Size(623, 332);
			this._studyTableView.TabIndex = 0;
			this._studyTableView.ToolbarModel = null;
			this._studyTableView.ToolStripItemDisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this._studyTableView.ToolStripRightToLeft = System.Windows.Forms.RightToLeft.No;
			// 
			// _resultsTitleBar
			// 
			this._resultsTitleBar.Dock = System.Windows.Forms.DockStyle.Top;
			this._resultsTitleBar.Font = new System.Drawing.Font("Arial", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this._resultsTitleBar.Location = new System.Drawing.Point(0, 0);
			this._resultsTitleBar.MouseOverColor = System.Drawing.Color.Empty;
			this._resultsTitleBar.Name = "_resultsTitleBar";
			this._resultsTitleBar.Size = new System.Drawing.Size(623, 23);
			this._resultsTitleBar.TabIndex = 3;
			this._resultsTitleBar.Text = "10 results found on server";
			// 
			// StudyBrowserControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this._studyTableView);
			this.Controls.Add(this._resultsTitleBar);
			this.Margin = new System.Windows.Forms.Padding(2);
			this.Name = "StudyBrowserControl";
			this.Size = new System.Drawing.Size(623, 355);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Desktop.View.WinForms.TableView _studyTableView;
		private Crownwood.DotNetMagic.Controls.TitleBar _resultsTitleBar;


	}
}
