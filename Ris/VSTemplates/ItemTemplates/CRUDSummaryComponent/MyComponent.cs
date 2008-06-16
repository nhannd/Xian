using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;

namespace $rootnamespace$
{
    [MenuAction("launch", "global-menus/Admin/$fileinputname$", "Launch")]
    [ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.$fileinputname$)]
    [ExtensionOf(typeof(DesktopToolExtensionPoint))]
    public class $fileinputname$AdminTool : Tool<IDesktopToolContext>
    {
        private IWorkspace _workspace;

        public void Launch()
        {
            if (_workspace == null)
            {
                try
                {
                    $fileinputname$SummaryComponent component = new $fileinputname$SummaryComponent();

                    _workspace = ApplicationComponent.LaunchAsWorkspace(
                        this.Context.DesktopWindow,
                        component,
                        "$fileinputname$");
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
	
	public class $fileinputname$SummaryTable : Table<$fileinputname$Summary>
    {
        public $fileinputname$SummaryTable()
        {
			//TODO: Add table columns
        }
    }



    /// <summary>
    /// $fileinputname$SummaryComponent class.
    /// </summary>
    public class $fileinputname$SummaryComponent : SummaryComponentBase<$fileinputname$Summary, $fileinputname$SummaryTable>
    {
		/// <summary>
		/// Override this method to perform custom initialization of the action model,
		/// such as adding permissions or adding custom actions.
		/// </summary>
		/// <param name="model"></param>
		protected override void InitializeActionModel(CrudActionModel model)
		{
			base.InitializeActionModel(model);

			model.Add.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.$fileinputname$);
			model.Edit.SetPermissibility(ClearCanvas.Ris.Application.Common.AuthorityTokens.Admin.Data.$fileinputname$);
		}
		
        /// <summary>
        /// Gets the list of items to show in the table, according to the specifed first and max items.
        /// </summary>
        /// <param name="firstItem"></param>
        /// <param name="maxItems"></param>
        /// <returns></returns>
        protected override IList<$fileinputname$Summary> ListItems(int firstItem, int maxItems)
        {
            List$fileinputname$sResponse listResponse = null;
            Platform.GetService<I$fileinputname$AdminService>(
                delegate(I$fileinputname$AdminService service)
                {
                    listResponse = service.List$fileinputname$s(new List$fileinputname$sRequest(new SearchResultPage(firstItem, maxItems)));
                });

            return listResponse.$fileinputname$s;
        }

        /// <summary>
        /// Called to handle the "add" action.
        /// </summary>
        /// <param name="addedItems"></param>
        /// <returns>True if items were added, false otherwise.</returns>
        protected override bool AddItems(out IList<$fileinputname$Summary> addedItems)
        {
			// TODO: implement this method
            throw new NotImplementedException();
        }

        /// <summary>
        /// Called to handle the "edit" action.
        /// </summary>
        /// <param name="items">A list of items to edit.</param>
        /// <param name="editedItems">The list of items that were edited.</param>
        /// <returns>True if items were edited, false otherwise.</returns>
        protected override bool EditItems(IList<$fileinputname$Summary> items, out IList<$fileinputname$Summary> editedItems)
        {
 			// TODO: implement this method
           throw new NotImplementedException();
        }

        /// <summary>
        /// Called to handle the "delete" action, if supported.
        /// </summary>
        /// <param name="items"></param>
        /// <returns>True if items were deleted, false otherwise.</returns>
        protected override bool DeleteItems(IList<$fileinputname$Summary> items)
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
        protected override bool IsSameItem($fileinputname$Summary x, $fileinputname$Summary y)
        {
			// TODO: implement this method
            throw new NotImplementedException();
        }
    }
}
