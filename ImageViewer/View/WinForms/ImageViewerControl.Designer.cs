using System;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	partial class ImageViewerControl
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

				if (_physicalWorkspace != null)
				{
					_physicalWorkspace.Drawing -= new EventHandler(OnPhysicalWorkspaceDrawing);
					_physicalWorkspace.ImageBoxAdded -= new EventHandler<ImageBoxEventArgs>(OnImageBoxAdded);
					_physicalWorkspace.ImageBoxRemoved -= new EventHandler<ImageBoxEventArgs>(OnImageBoxRemoved);
				}
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
			this._contextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.SuspendLayout();
			// 
			// _contextMenu
			// 
			this._contextMenu.Name = "_contextMenu";
			this._contextMenu.Size = new System.Drawing.Size(153, 26);
			this._contextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.OnContextMenuOpening);
			// 
			// ImageViewerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Name = "ImageViewerControl";
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ContextMenuStrip _contextMenu;
	}
}
