using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Workstation.Model.Actions;
using ClearCanvas.Workstation.Model.Tools;

namespace ClearCanvas.Workstation.View.GTK
{
    public class StockWorkstationTools
    {
		[MenuAction("activate", "MenuFile/MenuFileExitApplication")]
		[ClickHandler("activate", "Activate")]
		
		[ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Workstation.Model.WorkstationToolExtensionPoint))]
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
