using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Admin
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
	/// ProcedureTypeSummaryComponent class.
	/// </summary>
	public class ProcedureTypeSummaryComponent : SummaryComponentBase<ProcedureTypeSummary, ProcedureTypeTable>
	{
		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(CrudActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.ProcedureType);
		}

		/// <summary>
		/// Gets the list of items to show in the table, according to the specifed first and max items.
		/// </summary>
		/// <param name="firstItem"></param>
		/// <param name="maxItems"></param>
		/// <returns></returns>
		protected override IList<ProcedureTypeSummary> ListItems(int firstItem, int maxItems)
		{
			ListProcedureTypesResponse _response = null;
			Platform.GetService<IProcedureTypeAdminService>(
				delegate(IProcedureTypeAdminService service)
				{
					_response = service.ListProcedureTypes(new ListProcedureTypesRequest(new SearchResultPage(firstItem, maxItems)));
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
				this.Host.DesktopWindow, editor, SR.TitleUpdateProcedureType + " - " + item.Name);
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
		/// <returns>True if items were deleted, false otherwise.</returns>
		protected override bool DeleteItems(IList<ProcedureTypeSummary> items)
		{
			// TODO: implement this method if delete is supported, otherwise don't 
			throw new NotImplementedException();
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
