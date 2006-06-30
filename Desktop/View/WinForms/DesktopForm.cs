using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Controls.WinForms;

using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Controls;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class DesktopForm : Form
    {
        private ActionModelRoot _menuModel;
        private ActionModelRoot _toolbarModel;
		private WindowManager _windowManager;

        public DesktopForm()
        {
			if (SplashScreen.SplashForm != null)
				SplashScreen.SplashForm.Owner = this;
			
			InitializeComponent();
			this.Size = new Size(1024, 768);
			this.Text = String.Format("{0} {1}",
                DesktopApplication.ApplicationName,
                DesktopApplication.ApplicationVersion);
 
            // Subscribe to WorkspaceManager events so we know when workspaces are being
            // added, removed and activated
			DesktopApplication.WorkspaceManager.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
            DesktopApplication.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);
            DesktopApplication.WorkspaceManager.WorkspaceActivated += new EventHandler<WorkspaceEventArgs>(OnWorkspaceActivated);

			_windowManager = new WindowManager(this.toolStripContainer.ContentPanel, this.tabControl);

			RebuildMenusAndToolbars(null);
			this.tabControl.ClosePressed += new EventHandler(OnCloseWorkspace);
        }

        private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
        {
            try
            {
                RebuildMenusAndToolbars(e.Workspace);
				_windowManager.AddWorkpace(e.Workspace);
            }
            catch (Exception ex)
            {
				Platform.Log(ex, LogLevel.Error);
			}
        }

        // This is the event handler for when a workspace is removed from the
        // WorkspaceManager.  Not to be confused with OnCloseWorkspace
        private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
        {
            try
            {
                // If the form that owns the workspace that was removed is still around, close it
                // (if the command was invoked from the menu, the form will not have closed
                // on its own)
				_windowManager.RemoveWorkspace(e.Workspace);
			}
            catch (Exception ex)
            {
				Platform.Log(ex, LogLevel.Error);
			}
        }

        private void OnWorkspaceActivated(object sender, WorkspaceEventArgs e)
        {
            try
            {
                // When the user switches between workspace, we need to rebuild
                // the menu and toolbars to reflect the tools in use for the active workspace
                RebuildMenusAndToolbars(e.Workspace);
                _windowManager.ActivateWorkspace(e.Workspace);
            }
            catch (Exception ex)
            {
				Platform.Log(ex, LogLevel.Error);
			}
        }

		private void OnCloseWorkspace(object sender, EventArgs e)
		{
			try
			{
				RemoveActiveWorkspace();
                RebuildMenusAndToolbars(DesktopApplication.ActiveWorkspace);
				GC.Collect();
			}
			catch (Exception ex)
			{
				Platform.Log(ex, LogLevel.Error);
			}
		}
		
		internal void RemoveActiveWorkspace()
        {
            _windowManager.RemoveWorkspace(DesktopApplication.ActiveWorkspace);
		}

        private void RebuildMenusAndToolbars(IWorkspace activeWorkspace)
        {
			// Suspend the layouts so we avoid the flicker when we empty
			// and refill the menus and toolbars
			this.mainMenu.SuspendLayout();
			this.toolbar.SuspendLayout();
			// very important to clean up the existing ones first
            ToolStripBuilder.Clear(this.mainMenu.Items);
            ToolStripBuilder.Clear(this.toolbar.Items);

            _menuModel = new ActionModelRoot(null);
            _toolbarModel = new ActionModelRoot(null);

            _menuModel.Merge(DesktopApplication.ToolSet.MenuModel);
            _toolbarModel.Merge(DesktopApplication.ToolSet.ToolbarModel);

            if (activeWorkspace != null)
            {
                _menuModel.Merge(DesktopApplication.ActiveWorkspace.ToolSet.MenuModel);
                _toolbarModel.Merge(DesktopApplication.ActiveWorkspace.ToolSet.ToolbarModel);
            }

            ToolStripBuilder.BuildMenu(this.mainMenu.Items, _menuModel.ChildNodes);
			ToolStripBuilder.BuildToolbar(this.toolbar.Items, _toolbarModel.ChildNodes);
			this.toolbar.ResumeLayout();
			this.mainMenu.ResumeLayout();
		}
	}
}