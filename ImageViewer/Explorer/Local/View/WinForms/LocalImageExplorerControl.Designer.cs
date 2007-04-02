namespace ClearCanvas.ImageViewer.Explorer.Local.View.WinForms
{
	partial class LocalImageExplorerControl
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
			this.components = new System.ComponentModel.Container();
			this._fileBrowser = new ClearCanvas.Controls.WinForms.FileBrowser.Browser();
			this._shellBrowser = new ClearCanvas.Controls.WinForms.FileBrowser.ShellDll.ShellBrowser();
			this.SuspendLayout();
			// 
			// _fileBrowser
			// 
			this._fileBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this._fileBrowser.ListViewMode = System.Windows.Forms.View.Details;
			this._fileBrowser.Location = new System.Drawing.Point(0, 0);
			this._fileBrowser.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
			this._fileBrowser.Name = "_fileBrowser";
			this._fileBrowser.ShellBrowser = this._shellBrowser;
			this._fileBrowser.Size = new System.Drawing.Size(959, 597);
			this._fileBrowser.SplitterDistance = 162;
			this._fileBrowser.TabIndex = 0;
			// 
			// LocalImageExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this._fileBrowser);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "LocalImageExplorerControl";
			this.Size = new System.Drawing.Size(959, 597);
			this.ResumeLayout(false);

		}

		#endregion

		private ClearCanvas.Controls.WinForms.FileBrowser.Browser _fileBrowser;
		private ClearCanvas.Controls.WinForms.FileBrowser.ShellDll.ShellBrowser _shellBrowser;
		private System.Windows.Forms.ContextMenuStrip _contextMenu;
	}
}
