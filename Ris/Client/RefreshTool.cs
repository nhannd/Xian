using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using System;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("apply", "folderexplorer-folders-contextmenu/Refresh", "Refresh")]
	[ButtonAction("apply", "folderexplorer-folders-toolbar/Refresh", "Refresh")]
	[Tooltip("apply", "Refresh Folder")]
	[IconSet("apply", IconScheme.Colour, "Icons.RefreshToolSmall.png", "Icons.RefreshToolMedium.png", "Icons.RefreshToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	public abstract class RefreshTool<TWorkflowFolderToolContext> : Tool<TWorkflowFolderToolContext>
		where TWorkflowFolderToolContext : IWorkflowFolderToolContext
	{
		public bool Enabled
		{
			get { return this.Context.SelectedFolder != null; }
		}

		public event EventHandler EnabledChanged
		{
			add { this.Context.SelectedFolderChanged += value; }
			remove { this.Context.SelectedFolderChanged -= value; }
		}

		public void Refresh()
		{
			if (this.Context.SelectedFolder != null)
				this.Context.SelectedFolder.Refresh();
		}
	}
}