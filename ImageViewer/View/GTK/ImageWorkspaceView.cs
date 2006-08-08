using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
//using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.GTK
{
	[ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceViewExtensionPoint))]
	public class ImageWorkspaceView : GtkView, IWorkspaceView
	{
		//private ImageWorkspaceControl _imageWorkspaceControl;
		private ImageWorkspaceDrawingArea _imageWorkspaceControl;
		private ImageWorkspace _imageWorkspace;

		public ImageWorkspaceView()
		{
		}

		public void SetWorkspace(Workspace workspace)
		{
			_imageWorkspace = workspace as ImageWorkspace;
			Platform.CheckForInvalidCast(_imageWorkspace, "workspace", "ImageWorkspace");
		}

		public override object GuiElement
		{
			get 
			{
				if (_imageWorkspaceControl == null)
					//_imageWorkspaceControl = new ImageWorkspaceControl(_imageWorkspace);
					_imageWorkspaceControl = new ImageWorkspaceDrawingArea(_imageWorkspace);
	
				return _imageWorkspaceControl; 
			}
		}
	}
}
