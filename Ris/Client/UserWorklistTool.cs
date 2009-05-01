#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.Admin.WorklistAdmin;
using System.Threading;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Allows users to add/edit/delete user-defined worklists directly in the folder explorer.
    /// </summary>
	[ButtonAction("add", "folderexplorer-folders-toolbar/New Worklist", "Add")]
	[MenuAction("add", "folderexplorer-folders-contextmenu/New Worklist", "Add")]
	[IconSet("add", CrudActionModel.IconAddResource)]
	[Tooltip("add", "Add a new worklist")]
	[EnabledStateObserver("add", "AddEnabled", "EnablementChanged")]
    [VisibleStateObserver("add", "Visible", "VisibleChanged")]

	[ButtonAction("edit", "folderexplorer-folders-toolbar/Edit Worklist", "Edit")]
	[MenuAction("edit", "folderexplorer-folders-contextmenu/Edit Worklist", "Edit")]
	[IconSet("edit", CrudActionModel.IconEditResource)]
	[Tooltip("edit", "Edit worklist")]
	[EnabledStateObserver("edit", "EditEnabled", "EnablementChanged")]
    [VisibleStateObserver("edit", "Visible", "VisibleChanged")]

    [ButtonAction("duplicate", "folderexplorer-folders-toolbar/Duplicate Worklist", "Duplicate")]
    [MenuAction("duplicate", "folderexplorer-folders-contextmenu/Duplicate Worklist", "Duplicate")]
	[IconSet("duplicate", "Icons.DuplicateSmall.png")]
	[Tooltip("duplicate", "Duplicate worklist")]
    [EnabledStateObserver("duplicate", "DuplicateEnabled", "EnablementChanged")]
    [VisibleStateObserver("duplicate", "Visible", "VisibleChanged")]

	[ButtonAction("delete", "folderexplorer-folders-toolbar/Delete Worklist", "Delete")]
	[MenuAction("delete", "folderexplorer-folders-contextmenu/Delete Worklist", "Delete")]
	[IconSet("delete", CrudActionModel.IconDeleteResource)]
	[Tooltip("delete", "Delete worklist")]
	[EnabledStateObserver("delete", "DeleteEnabled", "EnablementChanged")]
    [VisibleStateObserver("delete", "Visible", "VisibleChanged")]

	[ExtensionOf(typeof(FolderExplorerGroupToolExtensionPoint))]
	public class UserWorklistTool : Tool<IFolderExplorerGroupToolContext>
    {
        #region Public API

        public bool Visible
        {
            get { return IsWorklistFolderSystem && (HasGroupAdminAuthority || HasPersonalAdminAuthority); }
        }

        public event EventHandler VisibleChanged
        {
            add { this.Context.SelectedFolderSystemChanged += value; }
            remove { this.Context.SelectedFolderSystemChanged -= value; }
        }


		public event EventHandler EnablementChanged
		{
			add
            {
                this.Context.SelectedFolderChanged += value;
                this.Context.SelectedFolderSystemChanged += value;
            }
			remove
            {
                this.Context.SelectedFolderChanged -= value;
                this.Context.SelectedFolderSystemChanged -= value;
            }
		}

		public bool AddEnabled
		{
			get { return CanAdd(); }
		}

		public bool EditEnabled
		{
			get { return CanEdit(this.Context.SelectedFolder); }
		}

        public bool DuplicateEnabled
        {
            get { return CanDuplicate(this.Context.SelectedFolder); }
        }

		public bool DeleteEnabled
		{
			get { return CanDelete(this.Context.SelectedFolder); }
		}

		public void Add()
		{
			if (!CanAdd())
				return;

			try
			{
                IWorklistFolderSystem fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;
                IWorklistFolder folder = this.Context.SelectedFolder as IWorklistFolder;
                string initialWorklistClassName = folder == null ? null : folder.WorklistClassName;


                WorklistEditorComponent editor = new WorklistEditorComponent(false, fs.SupportedWorklistClasses, initialWorklistClassName);
                ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Context.DesktopWindow,
                    new DialogBoxCreationArgs(editor, "New Worklist", null, DialogSizeHint.Medium));
				if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					AddNewWorklistsToFolderSystem(editor.EditedWorklistSummaries, fs);
				}

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		public void Edit()
		{
            IWorklistFolder folder = (IWorklistFolder)this.Context.SelectedFolder;
            if (!CanEdit(folder))
                return;

			try
			{
				WorklistEditorComponent editor = new WorklistEditorComponent(folder.WorklistRef, false);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Context.DesktopWindow,
                    new DialogBoxCreationArgs(editor, "Edit Worklist" + " - " + folder.Name, null, DialogSizeHint.Medium));
                if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					// refresh the folder
					IWorklistFolderSystem fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;
					fs.InvalidateFolder(folder);
				}

			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

        public void Duplicate()
        {
            IWorklistFolder folder = (IWorklistFolder)this.Context.SelectedFolder;
            if (!CanDuplicate(folder))
                return;

			try
			{
				IWorklistFolderSystem fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;
				string initialWorklistClassName = folder == null ? null : folder.WorklistClassName;

				WorklistEditorComponent editor = new WorklistEditorComponent(folder.WorklistRef, false, fs.SupportedWorklistClasses, initialWorklistClassName);
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                    this.Context.DesktopWindow,
                    new DialogBoxCreationArgs(editor, "New Worklist", null, DialogSizeHint.Medium));
                if (exitCode == ApplicationComponentExitCode.Accepted)
				{
					AddNewWorklistsToFolderSystem(editor.EditedWorklistSummaries, fs);
				}
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		public void Delete()
		{
            IWorklistFolder folder = (IWorklistFolder)this.Context.SelectedFolder;
            if (!CanDelete(folder))
                return;

            // confirm deletion
            if (this.Context.DesktopWindow.ShowMessageBox(
                "Are you sure you want to delete the selected worklist?", MessageBoxActions.OkCancel)
                != DialogBoxAction.Ok)
                return;

            try
            {
                // delete worklist
                Platform.GetService<IWorklistAdminService>(
                    delegate(IWorklistAdminService service)
                    {
                        service.DeleteWorklist(
                            new DeleteWorklistRequest(folder.WorklistRef));
                    });

                // if successful, remove folder from folder system
                IWorklistFolderSystem fs = (IWorklistFolderSystem)this.Context.SelectedFolderSystem;
                fs.Folders.Remove(folder);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Context.DesktopWindow);
            }
        }

        #endregion

        #region Helpers

        private static void AddNewWorklistsToFolderSystem(IEnumerable<WorklistAdminSummary> worklists, IWorklistFolderSystem fs)
        {
			foreach (WorklistAdminSummary worklist in worklists)
            {
                // try to add worklist to this folder system
                IWorklistFolder folder = fs.AddWorklistFolder(worklist);

                // if add was successful, refresh the folder
                if (folder != null)
                {
                    fs.InvalidateFolder(folder);
                }
            }
        }

        private bool CanAdd()
		{
            return IsWorklistFolderSystem && (HasGroupAdminAuthority || HasPersonalAdminAuthority);
		}

		private bool CanEdit(IFolder folder)
		{
            return CheckAccess(folder);
		}

        private bool CanDuplicate(IFolder folder)
        {
            // must be able to add, folder must be a worklist folder, and must not be static
            return CanAdd() && (folder is IWorklistFolder) && !folder.IsStatic;
        }

        private bool CanDelete(IFolder folder)
		{
            return CheckAccess(folder);
        }

        /// <summary>
        /// Checks if the current user can modify/delete this folder.
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        private static bool CheckAccess(IFolder folder)
        {
            IWorklistFolder wf = folder as IWorklistFolder;

            // if not a worklist folder, or not user defined, can't edit it
            if (wf == null || wf.Ownership == WorklistOwnership.Admin)
                return false;

            // if staff owned, must have personal authority
            if (wf.Ownership == WorklistOwnership.Staff && !HasPersonalAdminAuthority)
                return false;

            // if group owned, must have group authority
            if (wf.Ownership == WorklistOwnership.Group && !HasGroupAdminAuthority)
                return false;

            return true;
        }

        private bool IsWorklistFolderSystem
        {
            get { return this.Context.SelectedFolderSystem is IWorklistFolderSystem; }
        }

        private static bool HasGroupAdminAuthority
        {
            get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Worklist.Group); }
        }

        private static bool HasPersonalAdminAuthority
        {
            get { return Thread.CurrentPrincipal.IsInRole(ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Worklist.Personal); }
        }

        #endregion
    }
}
