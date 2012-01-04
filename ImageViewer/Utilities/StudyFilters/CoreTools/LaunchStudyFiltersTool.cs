#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.CoreTools
{
	[MenuAction("Open", "global-menus/MenuTools/MenuUtilities/MenuStudyFilters", "Open")]
	[IconSet("Open", "Icons.StudyFilterToolSmall.png", "Icons.StudyFilterToolMedium.png", "Icons.StudyFilterToolLarge.png")]
	[ViewerActionPermission("Open", AuthorityTokens.StudyFilters)]
	[ExtensionOf(typeof (DesktopToolExtensionPoint))]
	public class LaunchStudyFiltersTool : Tool<IDesktopToolContext>
	{
		private string _lastFolder = string.Empty;

		public void Open()
		{
			SelectFolderDialogCreationArgs args = new SelectFolderDialogCreationArgs();
			args.AllowCreateNewFolder = false;
			args.Path = _lastFolder;
			args.Prompt = SR.MessageSelectFolderToFilter;

			FileDialogResult result = base.Context.DesktopWindow.ShowSelectFolderDialogBox(args);
			if (result.Action == DialogBoxAction.Ok)
			{
				_lastFolder = result.FileName;

				StudyFilterComponent component = new StudyFilterComponent();
				component.BulkOperationsMode = true;

				if (component.Load(base.Context.DesktopWindow, true, result.FileName))
				{
					component.Refresh(true);
					base.Context.DesktopWindow.Workspaces.AddNew(component, SR.StudyFilters);
				}

				component.BulkOperationsMode = false;
			}
		}
	}
}