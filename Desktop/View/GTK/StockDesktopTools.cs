using System;
using System.Collections.Generic;
using System.Text;

//using ClearCanvas.ImageViewer.Actions;
//using ClearCanvas.ImageViewer.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

//namespace ClearCanvas.ImageViewer.View.GTK
namespace ClearCanvas.Desktop.View.GTK
{
    //public class StockWorkstationTools
    public class StockDesktopTools
    {
		[MenuAction("activate", "MenuFile/MenuFileExitApplication")]
		[ClickHandler("activate", "Activate")]
		
		//[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.WorkstationToolExtensionPoint))]
		[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Desktop.DesktopToolExtensionPoint))]
        public class ExitAppTool : StockTool
        {
            public ExitAppTool()
            {
            }
			
			public void Activate()
			{
				_mainView.QuitMessagePump();
			}
        }
    }
}
