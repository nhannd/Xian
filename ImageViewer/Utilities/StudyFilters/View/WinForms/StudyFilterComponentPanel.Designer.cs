namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	partial class StudyFilterComponentPanel {
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
			this._tableView = new ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.StudyFilterTableView();
			this._workspaceDivisor = new System.Windows.Forms.SplitContainer();
			this._toolbar = new System.Windows.Forms.ToolStrip();
			this._workspaceDivisor.Panel1.SuspendLayout();
			this._workspaceDivisor.SuspendLayout();
			this.SuspendLayout();
			// 
			// _tableView
			// 
			this._tableView.Dock = System.Windows.Forms.DockStyle.Fill;
			this._tableView.Location = new System.Drawing.Point(0, 33);
			this._tableView.Name = "_tableView";
			this._tableView.ReadOnly = false;
			this._tableView.ShowToolbar = false;
			this._tableView.Size = new System.Drawing.Size(790, 528);
			this._tableView.TabIndex = 0;
			this._tableView.SelectionChanged += new System.EventHandler(this._tableView_SelectionChanged);
			// 
			// _workspaceDivisor
			// 
			this._workspaceDivisor.Dock = System.Windows.Forms.DockStyle.Fill;
			this._workspaceDivisor.Location = new System.Drawing.Point(0, 0);
			this._workspaceDivisor.Name = "_workspaceDivisor";
			this._workspaceDivisor.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// _workspaceDivisor.Panel1
			// 
			this._workspaceDivisor.Panel1.Controls.Add(this._tableView);
			this._workspaceDivisor.Panel1.Controls.Add(this._toolbar);
			this._workspaceDivisor.Panel2Collapsed = true;
			this._workspaceDivisor.Size = new System.Drawing.Size(790, 561);
			this._workspaceDivisor.SplitterDistance = 107;
			this._workspaceDivisor.TabIndex = 1;
			// 
			// _toolbar
			// 
			this._toolbar.AutoSize = false;
			this._toolbar.ImageScalingSize = new System.Drawing.Size(24, 24);
			this._toolbar.Location = new System.Drawing.Point(0, 0);
			this._toolbar.Name = "_toolbar";
			this._toolbar.Size = new System.Drawing.Size(790, 33);
			this._toolbar.TabIndex = 1;
			this._toolbar.Text = "toolStrip1";
			// 
			// StudyFilterComponentPanel
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._workspaceDivisor);
			this.Name = "StudyFilterComponentPanel";
			this.Size = new System.Drawing.Size(790, 561);
			this._workspaceDivisor.Panel1.ResumeLayout(false);
			this._workspaceDivisor.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms.StudyFilterTableView _tableView;
		private System.Windows.Forms.SplitContainer _workspaceDivisor;
		private System.Windows.Forms.ToolStrip _toolbar;
	}
}