using System;
using System.Drawing;
using ClearCanvas.Workstation.Model;
using ClearCanvas.Common;
using ClearCanvas.Common.Application;
using ClearCanvas.Common.Application.Tools;
using ClearCanvas.Common.Application.Actions;

namespace ClearCanvas.Workstation.Dashboard
{
    [ExtensionPoint()]
    public class DashboardToolViewExtensionPoint : ExtensionPoint<IToolView>
    {
    }

    [MenuAction("activate", "MenuFile/MenuFileSearch")]
    [ButtonAction("activate", "ToolbarStandard/ToolbarToolsStandardStudyCentre")]
    [ClickHandler("activate", "ShowHide")]
    [CheckedStateObserver("activate", "IsViewActive", "ViewActivationChanged")]
	[IconSet("activate", IconScheme.Colour, "", "Icons.DashboardMedium.png", "Icons.DashboardLarge.png")]

    [ToolView(typeof(DashboardToolViewExtensionPoint), 
		"Dashboard", 
		ToolViewDisplayHint.DockTop | 
		ToolViewDisplayHint.MaximizeOnDock | 
		ToolViewDisplayHint.HideOnWorkspaceOpen,
		"IsViewActive", 
		"ViewActivationChanged")]

    [ClearCanvas.Common.ExtensionOf(typeof(ClearCanvas.Common.Application.WorkstationToolExtensionPoint))]
	public class DashboardTool : Tool
	{
		private static bool _showView;
		private event EventHandler _viewActivationChanged;

		public DashboardTool()
		{
			// Turn on a startup
			_showView = true;
		}

		public override void Initialize()
		{
			WorkstationModel.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);
		}

		private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
		{
			if (WorkstationModel.WorkspaceManager.Workspaces.Count == 0)
				this.IsViewActive = true;
		}

		public void ShowHide()
		{
            this.IsViewActive = !this.IsViewActive;
		}

        public bool IsViewActive
        {
            get { return _showView; }
            set
            {
                if (_showView != value)
                {
                    _showView = value;
                    EventsHelper.Fire(_viewActivationChanged, this, new EventArgs());
                }
            }
        }

		public event EventHandler ViewActivationChanged
		{
			add { _viewActivationChanged += value; }
			remove { _viewActivationChanged -= value; }
		}
	}
}
