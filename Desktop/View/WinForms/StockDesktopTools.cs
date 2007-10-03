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
		[MenuAction("exit", "global-menus/MenuFile/MenuExitApplication", "ExitApp", KeyStroke = XKeys.Alt | XKeys.F4)]
		[GroupHint("exit", "Application.Exit")]

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

		[MenuAction("closeWorkspace", "global-menus/MenuFile/MenuCloseWorkspace", "CloseWorkspace", KeyStroke = XKeys.Control | XKeys.F4)]
        [EnabledStateObserver("closeWorkspace", "Enabled", "EnabledChanged")]
		[GroupHint("closeWorkspace", "Application.Workspace.Close")]
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

                this.Context.DesktopWindow.Workspaces.ItemOpened += Workspaces_Changed;
                this.Context.DesktopWindow.Workspaces.ItemClosed += Workspaces_Changed;
            }

            public void CloseWorkspace()
            {
                IDesktopWindow window = this.Context.DesktopWindow;
                IWorkspace activeWorkspace = window.ActiveWorkspace;
                if (activeWorkspace != null)
                {
                    activeWorkspace.Close();
                }
            }

            public bool Enabled
            {
                get
                {
                    return this.Context.DesktopWindow.Workspaces.Count > 0;
                }
            }

            public event EventHandler EnabledChanged
            {
                add { _enabledChanged += value; }
                remove { _enabledChanged -= value; }
            }

            private void Workspaces_Changed(object sender, ItemEventArgs<Workspace> e)
            {
                EventsHelper.Fire(_enabledChanged, this, new EventArgs());
            }
        }
    }
}
