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
			this.tabControl.ClosePressed += new EventHandler(OnCloseWorkspaceTab);
        }

		protected override void OnLoad(EventArgs e)
		{
			LoadWindowSettings();

			base.OnLoad(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			SaveWindowSettings();

			base.OnClosing(e);
		}

		private void LoadWindowSettings()
		{
#if !MONO
			this.SuspendLayout();

			Rectangle screenRectangle = Screen.PrimaryScreen.Bounds;

			// If the bounds of the primary screen is different from what was saved
			// either because there was a screen resolution change or because the app
			// is being started for the first time, get Windows to properly position the window.
			if (screenRectangle != DesktopView.Settings.PrimaryScreenRectangle)
			{
				// Make the window size 75% of the primary screen
				float scale = 0.75f;
				this.Width = (int) (screenRectangle.Width * scale);
				this.Height = (int) (screenRectangle.Height * scale);

				// Center the window (for some reason, FormStartPosition.CenterScreen doesn't seem
				// to work.)
				int x = (screenRectangle.Width - this.Width) / 2;
				int y = (screenRectangle.Height - this.Height) / 2;
				this.Location = new Point(x, y);
			}
			else
			{
				this.Location = DesktopView.Settings.WindowRectangle.Location;
				this.Size = DesktopView.Settings.WindowRectangle.Size;
			}

			// If window was last closed when minimized, don't open it up minimized,
			// but rather just open it normally
			if (DesktopView.Settings.WindowState == FormWindowState.Minimized)
				this.WindowState = FormWindowState.Normal;
			else
				this.WindowState = DesktopView.Settings.WindowState;

			this.ResumeLayout();
#endif
		}

		private void SaveWindowSettings()
		{
#if !MONO
			// If the window state is normal, just save its location and size
			if (this.WindowState == FormWindowState.Normal)
				DesktopView.Settings.WindowRectangle = new Rectangle(this.Location, this.Size);
			// But, if it's minimized or maximized, save the restore bounds instead
			else
				DesktopView.Settings.WindowRectangle = this.RestoreBounds;

			DesktopView.Settings.WindowState = this.WindowState;
			DesktopView.Settings.PrimaryScreenRectangle = Screen.PrimaryScreen.Bounds;
			DesktopView.Settings.Save();
#endif
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
        // WorkspaceManager.  Not to be confused with OnCloseWorkspaceTab
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

        private void OnCloseWorkspaceTab(object sender, EventArgs e)
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
            DesktopApplication.ActiveWorkspace.Close();
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