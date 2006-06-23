using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class StockDesktopTools
    {
        [MenuAction("exit", "MenuFile/MenuFileExitApplication")]
        [ClickHandler("exit", "ExitApp")]
        [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class ExitAppTool : StockTool
        {
            public ExitAppTool()
            {
            }
			
			public void ExitApp()
			{
                MainForm.Close();
			}
        }

        [MenuAction("closeWorkspace", "MenuFile/MenuFileCloseWorkspace")]
        [ClickHandler("closeWorkspace", "CloseWorkspace")]
        [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class CloseWorkspaceTool : StockTool
        {
            public CloseWorkspaceTool()
            {
            }

            public void CloseWorkspace()
            {
                MainForm.RemoveActiveWorkspace();
            }
        }
    }
}
