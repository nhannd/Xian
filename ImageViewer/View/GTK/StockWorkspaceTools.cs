using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Workstation.Model.Tools;
using ClearCanvas.Workstation.Model.Actions;

namespace ClearCanvas.Workstation.View.GTK
{
    public class StockWorkspaceTools
    {
		[MenuAction("activate", "MenuFile/MenuFileCloseWorkspace")]
		[ClickHandler("activate", "Activate")]

  		[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
		public class CloseWorkspaceTool : StockTool
        {
            public CloseWorkspaceTool()
            {
            }
			
			/// <summary>
			/// Method OnActivated
			/// </summary>
			/// <param name="sender">An object</param>
			/// <param name="e">An EventArgs</param>
			public void Activate()
			{
				_mainView.CloseActiveWorkspace();
			}
        }
    }
}
