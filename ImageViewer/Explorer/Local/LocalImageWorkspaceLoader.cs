using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Local
{
	public class LocalImageWorkspaceLoader
	{
		private LocalImageLoader _imageLoader;

		public LocalImageWorkspaceLoader()
		{ 
		}

		public void Load(IEnumerable<string> paths, IDesktopWindow desktopWindow)
		{
			if (_imageLoader == null)
				_imageLoader = new LocalImageLoader();

			try
			{
				IEnumerable<string> studyInstanceUIDs = _imageLoader.Load(paths);

				foreach (string studyInstanceUID in studyInstanceUIDs)
				{
					ImageViewerComponent viewer = new ImageViewerComponent(studyInstanceUID);
					ApplicationComponent.LaunchAsWorkspace(
						desktopWindow,
						viewer,
						"Study",
						delegate
						{
							viewer.Dispose();
						});
				}
			}
			catch (OpenStudyException ex)
			{
				if (ex.StudyCouldNotBeLoaded)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorUnableToLoadStudy);
					return;
				}

				if (ex.AtLeastOneImageFailedToLoad)
				{
					Platform.ShowMessageBox(ClearCanvas.ImageViewer.SR.ErrorAtLeastOneImageFailedToLoad);
					return;
				}
			}
			catch (Exception ex)
			{
				// Just in case.  It's unlikely, but we could also catch:
				//    - DirectoryNotFoundException
				//    - ArgumentNullException
				//    - ArgumentException
				Platform.ShowMessageBox(ex.Message);
			}
		}
	}
}
