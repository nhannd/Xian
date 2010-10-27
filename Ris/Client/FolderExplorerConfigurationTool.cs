#region License

// Copyright (c) 2010, ClearCanvas Inc.
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
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "folderexplorer-folders-toolbar/Configure", "Configure")]
	[Tooltip("apply", "Organize Folders")]
	[IconSet("apply", IconScheme.Colour, "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png")]
	[ActionPermission("apply", AuthorityTokens.Desktop.FolderOrganization)]
	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class FolderExplorerConfigurationTool : Tool<IFolderExplorerGroupToolContext>
	{
		public void Configure()
		{
			try
			{
				ConfigurationDialog.Show(this.Context.DesktopWindow, SR.FolderExplorerConfigurationPagePath);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
