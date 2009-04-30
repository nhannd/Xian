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
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin;

namespace ClearCanvas.Ris.Client.Admin
{
	[MenuAction("launch", "global-menus/Admin/Protocol/Codes", "Launch")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProtocolGroups)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class ProtocolCodeAdminTool : Tool<IDesktopToolContext>
	{
		private IWorkspace _workspace;

		public void Launch()
		{
			if (_workspace == null)
			{
				try
				{
					ProtocolCodeSummaryComponent component = new ProtocolCodeSummaryComponent();

					_workspace = ApplicationComponent.LaunchAsWorkspace(
						this.Context.DesktopWindow,
						component,
						"Protocol Codes");
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

	public class ProtocolCodeSummaryTable : Table<ProtocolCodeSummary>
	{
		public ProtocolCodeSummaryTable()
		{
			this.Columns.Add(new TableColumn<ProtocolCodeSummary, string>("Name",
				delegate(ProtocolCodeSummary item)
				{
					 return item.Name;
				},
				0.3f));

			this.Columns.Add(new TableColumn<ProtocolCodeSummary, string>("Description",
				delegate(ProtocolCodeSummary item)
				{
					return item.Description;
				},
				0.6f));
		}
	}



	/// <summary>
	/// ProtocolCodeSummaryComponent class.
	/// </summary>
	public class ProtocolCodeSummaryComponent : SummaryComponentBase<ProtocolCodeSummary, ProtocolCodeSummaryTable, ListProtocolCodesRequest>
	{
		protected override bool SupportsDelete
		{
			get { return true; }
		}

		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(AdminActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProtocolGroups);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProtocolGroups);
			model.ToggleActivation.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProtocolGroups);
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <returns></returns>
		protected override IList<ProtocolCodeSummary> ListItems(ListProtocolCodesRequest request)
		{
			ListProtocolCodesResponse listResponse = null;
			Platform.GetService<IProtocolAdminService>(
				delegate(IProtocolAdminService service)
				{
					listResponse = service.ListProtocolCodes(request);
				});

			return listResponse.ProtocolCodes;
		}

		/// <summary>
		/// Called to handle the "add" action.
		/// </summary>
		/// <param name="addedItems"></param>
		/// <returns>True if items were added, false otherwise.</returns>
		protected override bool AddItems(out IList<ProtocolCodeSummary> addedItems)
		{
			addedItems = new List<ProtocolCodeSummary>();
			ProtocolCodeEditorComponent editor = new ProtocolCodeEditorComponent();
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, SR.TitleAddProtocolCode);
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				addedItems.Add(editor.ProtocolCode);
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
		protected override bool EditItems(IList<ProtocolCodeSummary> items, out IList<ProtocolCodeSummary> editedItems)
		{
			editedItems = new List<ProtocolCodeSummary>();
			ProtocolCodeSummary item = CollectionUtils.FirstElement(items);

			ProtocolCodeEditorComponent editor = new ProtocolCodeEditorComponent(item.ProtocolCodeRef);
			ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				this.Host.DesktopWindow, editor, string.Format("Update code '{0}'", item.Name));
			if (exitCode == ApplicationComponentExitCode.Accepted)
			{
				editedItems.Add(editor.ProtocolCode);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Called to handle the "delete" action, if supported.
		/// </summary>
		/// <param name="items"></param>
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<ProtocolCodeSummary> items, out IList<ProtocolCodeSummary> deletedItems, out string failureMessage)
		{
			failureMessage = null;
			deletedItems = new List<ProtocolCodeSummary>();

			foreach (ProtocolCodeSummary item in items)
			{
				try
				{
					Platform.GetService<IProtocolAdminService>(
						delegate(IProtocolAdminService service)
						{
							service.DeleteProtocolCode(new DeleteProtocolCodeRequest(item.ProtocolCodeRef));
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
		protected override bool UpdateItemsActivation(IList<ProtocolCodeSummary> items, out IList<ProtocolCodeSummary> editedItems)
		{
			List<ProtocolCodeSummary> results = new List<ProtocolCodeSummary>();
			foreach (ProtocolCodeSummary item in items)
			{
				Platform.GetService<IProtocolAdminService>(
					delegate(IProtocolAdminService service)
					{
						ProtocolCodeDetail detail = service.LoadProtocolCodeForEdit(
							new LoadProtocolCodeForEditRequest(item.ProtocolCodeRef)).ProtocolCode;
						detail.Deactivated = !detail.Deactivated;
						ProtocolCodeSummary summary = service.UpdateProtocolCode(
							new UpdateProtocolCodeRequest(detail)).ProtocolCode;

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
		protected override bool IsSameItem(ProtocolCodeSummary x, ProtocolCodeSummary y)
		{
			return x.ProtocolCodeRef.Equals(y.ProtocolCodeRef, true);
		}
	}
}
