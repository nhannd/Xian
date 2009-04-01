using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.Tools
{
	[ButtonAction("launch", DefaultToolbarActionSite + "/ToolbarLaunchInViewer", "Launch")]
	[ExtensionOf(typeof (StudyFilterToolExtensionPoint))]
	public class LaunchViewerTool : StudyFilterTool
	{
		public void Launch()
		{
			if (base.Selection == null || base.Selection.Count == 0)
				return;

			int n = 0;
			string[] selection = new string[base.Selection.Count];
			foreach (StudyItem item in base.Selection)
			{
				selection[n++] = item.File.FullName;
			}

			bool cancelled = true;
			ImageViewerComponent viewer = new ImageViewerComponent();
			try
			{
				viewer.LoadImages(selection, base.Context.DesktopWindow, out cancelled);
			}
			catch (Exception ex)
			{
				base.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
			}

			if (cancelled)
			{
				viewer.Dispose();
				return;
			}

			LaunchImageViewerArgs launchArgs = new LaunchImageViewerArgs(WindowBehaviour.Auto);
			ImageViewerComponent.Launch(viewer, launchArgs);
		}
	}
}