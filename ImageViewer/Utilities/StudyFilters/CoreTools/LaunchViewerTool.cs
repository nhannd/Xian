#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[ButtonAction("launch", DefaultToolbarActionSite + "/ToolbarLaunchInViewer", "Launch")]
	[MenuAction("launch", DefaultContextMenuActionSite + "/MenuLaunchInViewer", "Launch")]
	[EnabledStateObserver("launch", "AtLeastOneSelected", "AtLeastOneSelectedChanged")]
	[IconSet("launch", "OpenToolSmall.png", "OpenToolSmall.png", "OpenToolSmall.png")]
	[ViewerActionPermission("launch", ImageViewer.AuthorityTokens.Study.Open)]
	[ExtensionOf(typeof(StudyFilterToolExtensionPoint))]
	public class LaunchViewerTool : StudyFilterTool
	{
		public void Launch()
		{
			if (base.SelectedItems == null || base.SelectedItems.Count == 0)
				return;

			int n = 0;
			string[] selection = new string[base.SelectedItems.Count];
			foreach (IStudyItem item in base.SelectedItems)
			{
				if (!string.IsNullOrEmpty(item.Filename))
					selection[n++] = item.Filename;
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

			try
			{
				LaunchImageViewerArgs launchArgs = new LaunchImageViewerArgs(WindowBehaviour.Auto);
				ImageViewerComponent.Launch(viewer, launchArgs);
			}
			catch (Exception ex)
			{
				base.DesktopWindow.ShowMessageBox(ex.Message, MessageBoxActions.Ok);
				Platform.Log(LogLevel.Error, ex, "ImageViewerComponent launch failure.");
			}
		}
	}
}