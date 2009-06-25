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
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;

namespace ClearCanvas.Ris.Client
{
	[MenuAction("launch", "global-menus/Admin/Procedure Types", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ProcedureTypeAdminTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    ProcedureTypeSummaryComponent component = new ProcedureTypeSummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        "Procedure Types");
                    _workspace.Closed += delegate { _workspace = null; };

                }
                catch (Exception e)
                {
                    // could not launch editor
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
	/// Extension point for views onto <see cref="ProcedureTypeSummaryComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class ProcedureTypeSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// ProcedureTypeSummaryComponent class.
	/// </summary>
	[AssociateView(typeof(ProcedureTypeSummaryComponentViewExtensionPoint))]
	public class ProcedureTypeSummaryComponent : SummaryComponentBase<ProcedureTypeSummary, ProcedureTypeSummaryTable, ListProcedureTypesRequest>
	{
		private string _id;
		private string _name;

		public ProcedureTypeSummaryComponent()
		{

		}

		public ProcedureTypeSummaryComponent(bool dialogMode)
			: base(dialogMode)
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

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
			model.Delete.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
		}

		protected override bool SupportsDelete
		{
			get
			{
				return true;
			}
		}

		#region Presentation Model

		public string Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		#endregion
		
		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<ProcedureTypeSummary> ListItems(ListProcedureTypesRequest request)
		{
			ListProcedureTypesResponse _response = null;
			Platform.GetService<IProcedureTypeAdminService>(
				delegate(IProcedureTypeAdminService service)
				{
					request.Id = _id;
					request.Name = _name;
					_response = service.ListProcedureTypes(request);
				});
			return _response.ProcedureTypes;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ProcedureTypeSummary> addedItems)
		{
			addedItems = new List<ProcedureTypeSummary>();
			ProcedureTypeEditorComponent editor = new ProcedureTypeEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddProcedureType);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ProcedureTypeSummary);
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
		protected override bool EditItems(IList<ProcedureTypeSummary> items, out IList<ProcedureTypeSummary> editedItems)
		{
			editedItems = new List<ProcedureTypeSummary>();
			ProcedureTypeSummary item = CollectionUtils.FirstElement(items);

			ProcedureTypeEditorComponent editor = new ProcedureTypeEditorComponent(item.ProcedureTypeRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleUpdateProcedureType + " - " + "("+item.Id+") " +item.Name);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ProcedureTypeSummary);
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
		protected override bool DeleteItems(IList<ProcedureTypeSummary> items, out IList<ProcedureTypeSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<ProcedureTypeSummary>();

			foreach (ProcedureTypeSummary item in items)
			{
				try
				{
					Platform.GetService<IProcedureTypeAdminService>(
						delegate(IProcedureTypeAdminService service)
						{
							service.DeleteProcedureType(new DeleteProcedureTypeRequest(item.ProcedureTypeRef));
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
		protected override bool UpdateItemsActivation(IList<ProcedureTypeSummary> items, out IList<ProcedureTypeSummary> editedItems)
		{
			List<ProcedureTypeSummary> results = new List<ProcedureTypeSummary>();
			foreach (ProcedureTypeSummary item in items)
			{
				Platform.GetService<IProcedureTypeAdminService>(
					delegate(IProcedureTypeAdminService service)
					{
						ProcedureTypeDetail detail = service.LoadProcedureTypeForEdit(
							new LoadProcedureTypeForEditRequest(item.ProcedureTypeRef)).ProcedureType;
						detail.Deactivated = !detail.Deactivated;
						ProcedureTypeSummary summary = service.UpdateProcedureType(
							new UpdateProcedureTypeRequest(detail)).ProcedureType;

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
		protected override bool IsSameItem(ProcedureTypeSummary x, ProcedureTypeSummary y)
		{
			if (x != null && y != null)
			{
				return x.ProcedureTypeRef.Equals(y.ProcedureTypeRef, true);
			}
			return false;
		}
	}
}
