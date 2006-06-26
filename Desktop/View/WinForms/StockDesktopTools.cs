using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
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
        [EnabledStateObserver("closeWorkspace", "Enabled", "EnabledChanged")]
        [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class CloseWorkspaceTool : StockTool
        {
            private event EventHandler _enabledChanged;
            
            public CloseWorkspaceTool()
            {
                DesktopApplication.WorkspaceManager.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(Workspaces_Changed);
                DesktopApplication.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(Workspaces_Changed);
            }

            public void CloseWorkspace()
            {
                MainForm.RemoveActiveWorkspace();
            }

            public bool Enabled
            {
                get { return DesktopApplication.WorkspaceManager.Workspaces.Count > 0; }
            }

            public event EventHandler EnabledChanged
            {
                add { _enabledChanged += value; }
                remove { _enabledChanged -= value; }
            }

            private void Workspaces_Changed(object sender, WorkspaceEventArgs e)
            {
                EventsHelper.Fire(_enabledChanged, this, new EventArgs());
            }
        }
    }
}
