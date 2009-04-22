using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;

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
			get { return this.Context.SelectedFolderSystem is IWorklistFolderSystem; }
		}

		public bool EditEnabled
		{
			get { return CanEdit(this.Context.SelectedFolder); }
		}

		public bool DeleteEnabled
		{
			//TODO: add more conditions
			get { return CanDelete(this.Context.SelectedFolder); }
		}


		public void Add()
		{
			if (!CanAdd())
				return;

			WorklistEditorComponent editor = new WorklistEditorComponent(false);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, "New Worklist");
			if(exitCode == ApplicationComponentExitCode.Accepted)
			{
				IWorklistFolderSystem fs = (IWorklistFolderSystem) this.Context.SelectedFolderSystem;
				foreach (WorklistAdminSummary worklist in editor.EditedWorklistSummaries)
				{
					fs.AddWorklistFolder(worklist);
				}
			}
		}

		public void Edit()
		{
			if(CanEdit(this.Context.SelectedFolder))
			{
				IWorklistFolder folder = (IWorklistFolder)this.Context.SelectedFolder;
				WorklistEditorComponent editor = new WorklistEditorComponent(folder.WorklistRef, false);
				ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, editor, "Edit Worklist");
			}
		}

		public void Delete()
		{

		}

		private bool CanAdd()
		{
			return this.Context.SelectedFolderSystem is IWorklistFolderSystem;
		}

		private bool CanEdit(IFolder folder)
		{
			IWorklistFolder wf = folder as IWorklistFolder;
			return wf != null && !wf.IsStatic;
		}

		private bool CanDelete(IFolder folder)
		{
			return false;
		}
	}
}
