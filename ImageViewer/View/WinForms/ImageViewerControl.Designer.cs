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
					_physicalWorkspace.LayoutCompleted -= new EventHandler(OnLayoutCompleted);
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
			this.SuspendLayout();
			// 
			// ImageViewerControl
			// 
			this.Name = "ImageViewerControl";
			this.ResumeLayout(false);

		}

		#endregion

	}
}
