using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.View.WinForms
{
    public class StockDesktopTools
    {
		[MenuAction("exit", "global-menus/MenuFile/MenuFileExitApplication", KeyStroke = XKeys.Alt | XKeys.F4)]
        [ClickHandler("exit", "ExitApp")]
        [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class ExitAppTool : StockTool
        {
            public ExitAppTool()
            {
            }
			
			public void ExitApp()
			{
                Application.Quit();
			}
        }

		[MenuAction("closeWorkspace", "global-menus/MenuFile/MenuFileCloseWorkspace", KeyStroke = XKeys.Control | XKeys.F4)]
		[ClickHandler("closeWorkspace", "CloseWorkspace")]
        [EnabledStateObserver("closeWorkspace", "Enabled", "EnabledChanged")]
        [ClearCanvas.Common.ExtensionOf(typeof(DesktopToolExtensionPoint))]
        public class CloseWorkspaceTool : StockTool
        {
            private event EventHandler _enabledChanged;
            
            public CloseWorkspaceTool()
            {
            }

            public override void Initialize()
            {
                base.Initialize();

                this.Context.DesktopWindow.WorkspaceManager.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(Workspaces_Changed);
                this.Context.DesktopWindow.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(Workspaces_Changed);
            }

            public void CloseWorkspace()
            {
                IDesktopWindow window = this.Context.DesktopWindow;
                IWorkspace activeWorkspace = window.ActiveWorkspace;
                if (activeWorkspace != null)
                {
                    window.WorkspaceManager.Workspaces.Remove(activeWorkspace);
                }
            }

            public bool Enabled
            {
                get { return this.Context.DesktopWindow.WorkspaceManager.Workspaces.Count > 0; }
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
