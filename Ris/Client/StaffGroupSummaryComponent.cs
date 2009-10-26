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

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.StaffGroupAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client
{
    [MenuAction("launch", "global-menus/Admin/Staff Groups", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.StaffGroup)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class StaffGroupAdminTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    StaffGroupSummaryComponent component = new StaffGroupSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        "Staff Groups");
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // failed to launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }
    
    public class StaffGroupSummaryTable : Table<StaffGroupSummary>
    {
        public StaffGroupSummaryTable()
        {
            this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Name",
                delegate(StaffGroupSummary item) { return item.Name; }, 0.5f));
            this.Columns.Add(new TableColumn<StaffGroupSummary, string>("Description",
                delegate(StaffGroupSummary item) { return item.Description; }, 1.0f));
			this.Columns.Add(new TableColumn<StaffGroupSummary, bool>("Elective",
				delegate(StaffGroupSummary item) { return item.IsElective; }, 0.2f));
		}
    }

    /// <summary>
    /// StaffGroupSummaryComponent class
    /// </summary>
    public class StaffGroupSummaryComponent : SummaryComponentBase<StaffGroupSummary, StaffGroupSummaryTable, ListStaffGroupsRequest>
    {
    	private readonly string _initialFilterText;
    	private readonly bool _electiveGroupsOnly;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffGroupSummaryComponent()
        {
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="dialogMode"></param>
		/// <param name="intialFilterText"></param>
		/// <param name="electiveGroupsOnly"></param>
		public StaffGroupSummaryComponent(bool dialogMode, string intialFilterText, bool electiveGroupsOnly)
			:base(dialogMode)
		{
			_initialFilterText = intialFilterText;
			_electiveGroupsOnly = electiveGroupsOnly;
		}

    	/// <summary>
    	/// Initializes the table.  Override this method to perform custom initialization of the table,
    	/// such as adding columns or setting other properties that control the table behaviour.
    	/// </summary>
    	/// <param name="table"></param>
    	protected override void InitializeTable(StaffGroupSummaryTable table)
		{
			base.InitializeTable(table);

			if (!string.IsNullOrEmpty(_initialFilterText))
			{
				table.Filter(new TableFilterParams(null, _initialFilterText));
			}
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.StaffGroup);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
        /// Gets the list of items to show in the table, according to the specifed first and max items.
        /// </summary>
        /// <returns></returns>
        protected override IList<StaffGroupSummary> ListItems(ListStaffGroupsRequest request)
        {
			request.ElectiveGroupsOnly = _electiveGroupsOnly;

            ListStaffGroupsResponse listResponse = null;
            Platform.GetService<IStaffGroupAdminService>(
                delegate(IStaffGroupAdminService service)
                {
                    listResponse = service.ListStaffGroups(request);
                });

            return listResponse.StaffGroups;
        }

        /// <summary>
        /// Called to handle the "add" action.
        /// </summary>
        /// <param name="addedItems"></param>
        /// <returns>True if items were added, false otherwise.</returns>
        protected override bool AddItems(out IList<StaffGroupSummary> addedItems)
        {
            addedItems = new List<StaffGroupSummary>();
            StaffGroupEditorComponent editor = new StaffGroupEditorComponent();
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, "Add Staff Group", null, DialogSizeHint.Large));
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                addedItems.Add(editor.StaffGroupSummary);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called to handle the "edit" action.
        /// </summary>
        /// <param name="items">A list of items to edit.</param>
        /// <param name="editedItems">The list of items that were edited.</param>
        /// <returns>True if items were edited, false otherwise.</returns>
        protected override bool EditItems(IList<StaffGroupSummary> items, out IList<StaffGroupSummary> editedItems)
        {
            editedItems = new List<StaffGroupSummary>();
            StaffGroupSummary item = CollectionUtils.FirstElement(items);

            StaffGroupEditorComponent editor = new StaffGroupEditorComponent(item.StaffGroupRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
                this.Host.DesktopWindow,
				new DialogBoxCreationArgs(editor, "Edit Staff Group - " + item.Name, null, DialogSizeHint.Large));
            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                editedItems.Add(editor.StaffGroupSummary);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Called to handle the "delete" action, if supported.
        /// </summary>
        /// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <param name="failureMessage">The message if there any errors that occurs during deletion.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<StaffGroupSummary> items, out IList<StaffGroupSummary> deletedItems, out string failureMessage)
        {
			failureMessage = null;
			deletedItems = new List<StaffGroupSummary>();

			foreach (StaffGroupSummary item in items)
			{
				try
				{
					Platform.GetService<IStaffGroupAdminService>(
						delegate(IStaffGroupAdminService service)
							{
								service.DeleteStaffGroup(new DeleteStaffGroupRequest(item.StaffGroupRef));
							});

					deletedItems.Add(item);
				}
				catch (Exception e)
				{
					failureMessage = e.Message;
				}
			}

			return deletedItems.Count > 0;
		}

		/// <summary>
		/// Called to handle the "toggle activation" action, if supported
		/// </summary>
		/// <param name="items">A list of items to edit.</param>
		/// <param name="editedItems">The list of items that were edited.</param>
		/// <returns>True if items were edited, false otherwise.</returns>
		protected override bool UpdateItemsActivation(IList<StaffGroupSummary> items, out IList<StaffGroupSummary> editedItems)
		{
			List<StaffGroupSummary> results = new List<StaffGroupSummary>();
			foreach (StaffGroupSummary item in items)
			{
				Platform.GetService<IStaffGroupAdminService>(
					delegate(IStaffGroupAdminService service)
					{
						StaffGroupDetail detail = service.LoadStaffGroupForEdit(
							new LoadStaffGroupForEditRequest(item.StaffGroupRef)).StaffGroup;
						detail.Deactivated = !detail.Deactivated;
						StaffGroupSummary summary = service.UpdateStaffGroup(
							new UpdateStaffGroupRequest(detail)).StaffGroup;

						results.Add(summary);
					});
			}

			editedItems = results;
			return true;
		}

        /// <summary>
        /// Compares two items to see if they represent the same item.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        protected override bool IsSameItem(StaffGroupSummary x, StaffGroupSummary y)
        {
            return x.StaffGroupRef.Equals(y.StaffGroupRef, true);
        }
    }
}
