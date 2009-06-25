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
using ClearCanvas.Enterprise.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.FacilityAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
    [MenuAction("launch", "global-menus/Admin/Facilities", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class FacilitySummaryTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    FacilitySummaryComponent component = new FacilitySummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        SR.TitleFacilities);
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
    /// Extension point for views onto <see cref="FacilitySummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class FacilitySummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// FacilitySummaryComponent class
    /// </summary>
    public class FacilitySummaryComponent : SummaryComponentBase<FacilitySummary, FacilityTable, ListAllFacilitiesRequest>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public FacilitySummaryComponent()
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

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.Facility);
		}

		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<FacilitySummary> ListItems(ListAllFacilitiesRequest request)
		{
			ListAllFacilitiesResponse listResponse = null;
			Platform.GetService<IFacilityAdminService>(
				delegate(IFacilityAdminService service)
				{
					listResponse = service.ListAllFacilities(request);
				});

			return listResponse.Facilities;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<FacilitySummary> addedItems)
		{
			addedItems = new List<FacilitySummary>();
			FacilityEditorComponent editor = new FacilityEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddFacility);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.FacilitySummary);
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
		protected override bool EditItems(IList<FacilitySummary> items, out IList<FacilitySummary> editedItems)
		{
			editedItems = new List<FacilitySummary>();
			FacilitySummary item = CollectionUtils.FirstElement(items);

			FacilityEditorComponent editor = new FacilityEditorComponent(item.FacilityRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateFacility + " - " + item.Name);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.FacilitySummary);
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
		protected override bool DeleteItems(IList<FacilitySummary> items, out IList<FacilitySummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<FacilitySummary>();

			foreach (FacilitySummary item in items)
			{
				try
				{
					Platform.GetService<IFacilityAdminService>(
						delegate(IFacilityAdminService service)
						{
							service.DeleteFacility(new DeleteFacilityRequest(item.FacilityRef));
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
    	protected override bool UpdateItemsActivation(IList<FacilitySummary> items, out IList<FacilitySummary> editedItems)
		{
			List<FacilitySummary> results = new List<FacilitySummary>();
			foreach (FacilitySummary item in items)
			{
				Platform.GetService<IFacilityAdminService>(
					delegate(IFacilityAdminService service)
					{
						FacilityDetail detail = service.LoadFacilityForEdit(
							new LoadFacilityForEditRequest(item.FacilityRef)).FacilityDetail;
						detail.Deactivated = !detail.Deactivated;
						FacilitySummary summary = service.UpdateFacility(
							new UpdateFacilityRequest(detail)).Facility;

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
		protected override bool IsSameItem(FacilitySummary x, FacilitySummary y)
		{
			return x.FacilityRef.Equals(y.FacilityRef, true);
		}
    }
}
