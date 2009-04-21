using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("add", "folderexplorer-folders-toolbar/New Worklist", "Add")]
	[MenuAction("add", "folderexplorer-folders-contextmenu/New Worklist", "Add")]
	[Tooltip("add", "Add a new worklist")]
	[EnabledStateObserver("add", "AddEnabled", "EnablementChanged")]

	[ButtonAction("edit", "folderexplorer-folders-toolbar/Edit Worklist", "Edit")]
	[MenuAction("edit", "folderexplorer-folders-contextmenu/Edit Worklist", "Edit")]
	[Tooltip("edit", "Edit this worklist")]
	[EnabledStateObserver("edit", "EditEnabled", "EnablementChanged")]

	[ButtonAction("delete", "folderexplorer-folders-toolbar/Delete Worklist", "Delete")]
	[MenuAction("delete", "folderexplorer-folders-contextmenu/Delete Worklist", "Delete")]
	[Tooltip("delete", "Delete this worklist")]
	[EnabledStateObserver("delete", "DeleteEnabled", "EnablementChanged")]

	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class UserWorklistTool : Tool<IFolderExplorerGroupToolContext>
	{

		public event EventHandler EnablementChanged
		{
			add { this.Context.SelectedFolderChanged += value; }
			remove { this.Context.SelectedFolderChanged -= value; }
		}

		public bool AddEnabled
		{
			//TODO: add more conditions
			get { return this.Context.SelectedFolderSystem is IWorklistFolderSystem; }
		}

		public bool EditEnabled
		{
			//TODO: add more conditions
			get { return this.Context.SelectedFolderSystem is IWorklistFolderSystem; }
		}

		public bool DeleteEnabled
		{
			//TODO: add more conditions
			get { return this.Context.SelectedFolderSystem is IWorklistFolderSystem; }
		}


		public void Add()
		{
			
		}
		public void Edit()
		{

		}
		public void Delete()
		{

		}
	}
}
