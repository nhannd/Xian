using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
	class WindowManager
	{
		private DockingManager _dockingManager;
		private Crownwood.DotNetMagic.Controls.TabControl _tabControl;
		private ToolViewManager _workspaceToolViewManager;
		private ToolViewManager _workbenchToolViewManager;

		public WindowManager(Panel panel, Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			Platform.CheckForNullReference(panel, "panel");
			Platform.CheckForNullReference(tabControl, "tabControl");

			_dockingManager = new DockingManager(panel, VisualStyle.Office2003);
			_dockingManager.ActiveColor = SystemColors.Control;
			_dockingManager.InnerControl = tabControl;
			_tabControl = tabControl;
			_workbenchToolViewManager = new ToolViewManager(_dockingManager);
            _workbenchToolViewManager.ToolSet = DesktopApplication.ToolSet;
			_workspaceToolViewManager = new ToolViewManager(_dockingManager);
		}

		public void AddWorkpace(IWorkspace workspace)
		{
			// Add the new tab
			WorkspaceTab workspaceTab = new WorkspaceTab(workspace);
			workspaceTab.Selected = true;
			_tabControl.TabPages.Add(workspaceTab);

			// Change the appearance of the tab control from grey to something lighter
			// when we have at least one tab
			if (_tabControl.TabPages.Count == 1)
			{
				_tabControl.HideTabsMode = HideTabsModes.ShowAlways;
				_tabControl.BackColor = SystemColors.ControlLightLight;
				_tabControl.ControlTopOffset = 2;
				_tabControl.ControlLeftOffset = 2;
				_tabControl.ControlBottomOffset = 2;
				_tabControl.ControlRightOffset = 2;

				if (_tabControl.Style == VisualStyle.IDE2005)
					_tabControl.IDE2005PixelBorder = true;
				else if (_tabControl.Style == VisualStyle.Office2003)
					_tabControl.OfficePixelBorder = true;
			}

			HideDockingWindows();
		}

		public void RemoveWorkspace(IWorkspace workspace)
		{
            // Find the tab that owns the workspace and remove it
            foreach (WorkspaceTab workspaceTab in _tabControl.TabPages)
            {
                if (workspaceTab.Workspace == workspace)
                {
                    _tabControl.TabPages.Remove(workspaceTab);
                    break;
                }
            }

			// If we have no more tabs left, revert to the plain gray look of the tab control
			// when we first started the app
			if (_tabControl.TabPages.Count == 0)
			{
				_tabControl.HideTabsMode = HideTabsModes.HideAlways;
				_tabControl.BackColor = SystemColors.ControlDark;
				_tabControl.ControlTopOffset = 0;
				_tabControl.ControlLeftOffset = 0;
				_tabControl.ControlBottomOffset = 0;
				_tabControl.ControlRightOffset = 0;

				if (_tabControl.Style == VisualStyle.IDE2005)
					_tabControl.IDE2005PixelBorder = false;
				else if (_tabControl.Style == VisualStyle.Office2003)
					_tabControl.OfficePixelBorder = false;

				_workspaceToolViewManager.ToolSet = null;

				// We MUST do this because of a bug in DotNetMagic where the TabControl
				// has a private member field called _oldPage that doesn't get nulled
				// when all the pages have been removed via TabPages.Remove.  Fortunately
				// Clear() does set _oldPage to null, so we do it here when all the pages
				// have been removed.
				_tabControl.TabPages.Clear();
			}
		}

		public void ActivateWorkspace(IWorkspace workspace)
		{
			_workspaceToolViewManager.ToolSet = workspace.ToolSet;
		}

		private void HideDockingWindows()
		{
			// When we add a new workspace, we need to:
			// 1) Retract all visible autohide windows
			// 2) Put docked windows in autohide mode if the tool has specified so

			_dockingManager.RemoveShowingAutoHideWindows();

			for (int i = 0; i < _dockingManager.Contents.Count; i++)
			{
				Content content = _dockingManager.Contents[i];

				ToolViewProxy toolViewProxy = content.Tag as ToolViewProxy;

				if (toolViewProxy != null)
				{
					if ((toolViewProxy.DisplayHint & ToolViewDisplayHint.HideOnWorkspaceOpen) != 0)
					{
						if (!content.IsAutoHidden)
							_dockingManager.ToggleContentAutoHide(content);
					}
				}
			}
		}
	}
}
