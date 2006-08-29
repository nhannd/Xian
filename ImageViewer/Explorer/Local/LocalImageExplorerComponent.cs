using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Explorer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Controls.WinForms;

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

			string studyInstanceUID = _imageLoader.Load(path);

			if (studyInstanceUID == "" ||
				ImageViewerComponent.StudyManager.StudyTree.GetStudy(studyInstanceUID) == null)
			{
				Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorUnableToLoadStudy);
				return;
			}

			ImageViewerComponent viewer = new ImageViewerComponent(studyInstanceUID);
			ApplicationComponent.LaunchAsWorkspace(this.Host.DesktopWindow, viewer, "Study", null);
		}
	}
}
