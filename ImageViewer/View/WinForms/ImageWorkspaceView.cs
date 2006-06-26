using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.View.WinForms
{
	[ExtensionOf(typeof(ClearCanvas.ImageViewer.ImageWorkspaceViewExtensionPoint))]
	public class ImageWorkspaceView : WinFormsView, IWorkspaceView
	{
		private ImageWorkspaceControl _imageWorkspaceControl;
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
					_imageWorkspaceControl = new ImageWorkspaceControl(_imageWorkspace);
	
				return _imageWorkspaceControl; 
			}
		}
	}
}
