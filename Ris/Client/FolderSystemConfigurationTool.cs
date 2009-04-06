using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Configuration;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "folderexplorer-folders-toolbar/Configure", "Configure")]
	[Tooltip("apply", "Configure Folder Systems")]
	[IconSet("apply", IconScheme.Colour, "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png", "Icons.OptionsToolSmall.png")]
	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class FolderSystemConfigurationTool : Tool<IFolderExplorerGroupToolContext>
	{
		public void Configure()
		{
			try
			{
				ConfigurationDialog.Show(this.Context.DesktopWindow, SR.TitleFolderSystems);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}
