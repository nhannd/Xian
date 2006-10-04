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

		public void Load(IEnumerable<string> paths)
		{
			if (_imageLoader == null)
				_imageLoader = new LocalImageLoader();

			IEnumerable<string> studyInstanceUIDs = _imageLoader.Load(paths);

			foreach (string studyInstanceUID in studyInstanceUIDs)
			{
				ImageViewerComponent viewer = new ImageViewerComponent(studyInstanceUID);
				ApplicationComponent.LaunchAsWorkspace(this.Host.DesktopWindow, viewer, "Study", null);
			}
		}
	}
}
