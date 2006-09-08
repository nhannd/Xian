using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	[ExtensionPoint()]
	public class LocalImageExplorerComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	[AssociateView(typeof(LocalImageExplorerComponentViewExtensionPoint))]
	public class LocalImageExplorerComponent : ApplicationComponent
	{
		LocalImageLoader _imageLoader;

		public LocalImageExplorerComponent()
		{

		}

		public void Load(string path)
		{
			if (_imageLoader == null)
				_imageLoader = new LocalImageLoader();

			string studyInstanceUID;

			studyInstanceUID = _imageLoader.Load(path);

			ImageViewerComponent viewer = new ImageViewerComponent(studyInstanceUID);
			ApplicationComponent.LaunchAsWorkspace(this.Host.DesktopWindow, viewer, "Study", null);
		}
	}
}
