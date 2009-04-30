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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.NoteCategoryAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Patient Note Categories", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory)]

    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class NoteCategorySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    NoteCategorySummaryComponent component = new NoteCategorySummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleNoteCategories);
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch component
                    ExceptionHandler.Report(e, this.Context.DesktopWindow);
                }
            }
            else
            {
                _workspace.Activate();
            }
        }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="NoteCategorySummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class NoteCategorySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// NoteCategorySummaryComponent class
    /// </summary>
	public class NoteCategorySummaryComponent : SummaryComponentBase<PatientNoteCategorySummary, NoteCategoryTable, ListAllNoteCategoriesRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NoteCategorySummaryComponent()
        {
        }

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.PatientNoteCategory);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}
		
		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<PatientNoteCategorySummary> ListItems(ListAllNoteCategoriesRequest request)
		{
			ListAllNoteCategoriesResponse listResponse = null;
			Platform.GetService<INoteCategoryAdminService>(
				delegate(INoteCategoryAdminService service)
				{
					listResponse = service.ListAllNoteCategories(request);
				});

			return listResponse.NoteCategories;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<PatientNoteCategorySummary> addedItems)
		{
			addedItems = new List<PatientNoteCategorySummary>();
			NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddNoteCategory);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.NoteCategorySummary);
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
		protected override bool EditItems(IList<PatientNoteCategorySummary> items, out IList<PatientNoteCategorySummary> editedItems)
		{
			editedItems = new List<PatientNoteCategorySummary>();
			PatientNoteCategorySummary item = CollectionUtils.FirstElement(items);

			NoteCategoryEditorComponent editor = new NoteCategoryEditorComponent(item.NoteCategoryRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateNoteCategory + " - " + item.Name);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.NoteCategorySummary);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <param name="deletedItems">The list of items that were deleted.</param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<PatientNoteCategorySummary> items, out IList<PatientNoteCategorySummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<PatientNoteCategorySummary>();

			foreach (PatientNoteCategorySummary item in items)
			{
				try
				{
					Platform.GetService<INoteCategoryAdminService>(
						delegate(INoteCategoryAdminService service)
						{
							service.DeleteNoteCategory(new DeleteNoteCategoryRequest(item.NoteCategoryRef));
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
		protected override bool UpdateItemsActivation(IList<PatientNoteCategorySummary> items, out IList<PatientNoteCategorySummary> editedItems)
		{
			List<PatientNoteCategorySummary> results = new List<PatientNoteCategorySummary>();
			foreach (PatientNoteCategorySummary item in items)
			{
				Platform.GetService<INoteCategoryAdminService>(
					delegate(INoteCategoryAdminService service)
					{
						PatientNoteCategoryDetail detail = service.LoadNoteCategoryForEdit(
							new LoadNoteCategoryForEditRequest(item.NoteCategoryRef)).NoteCategoryDetail;
						detail.Deactivated = !detail.Deactivated;
						PatientNoteCategorySummary summary = service.UpdateNoteCategory(
							new UpdateNoteCategoryRequest(detail)).NoteCategory;

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
		protected override bool IsSameItem(PatientNoteCategorySummary x, PatientNoteCategorySummary y)
		{
			return x.NoteCategoryRef.Equals(y.NoteCategoryRef, true);
		}
	}
}
