using System;
using System.Collections.Generic;
using System.Text;

//using ClearCanvas.ImageViewer.Tools;
//using ClearCanvas.ImageViewer.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    public class StockWorkspaceTools
    {
		[MenuAction("activate", "MenuFile/MenuFileCloseWorkspace")]
		[ClickHandler("activate", "Activate")]

  		//[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.ImageWorkspaceToolExtensionPoint))]
  		[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
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
