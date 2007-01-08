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
using Crownwood.DotNetMagic.Forms;
using System.Reflection;

namespace ClearCanvas.Desktop.View.WinForms
{
    public partial class DesktopForm : DotNetMagicForm
    {
        private IDesktopWindow _desktopWindow;

 		private WorkspaceViewManager _workspaceViewManager;
        private ShelfViewManager _shelfViewManager;

		private DockingManager _dockingManager;

        public DesktopForm(IDesktopWindow desktopWindow)
        {
            _desktopWindow = desktopWindow;

			if (SplashScreen.SplashForm != null)
				SplashScreen.SplashForm.Owner = this;
			
			InitializeComponent();
			this.Text = String.Format("{0} {1}", Application.ApplicationName, GetVersion());

			this.Style = WinFormsView.VisualStyle;

            // Subscribe to WorkspaceManager events so we know when workspaces are being
            // added, removed and activated
            _desktopWindow.WorkspaceManager.Workspaces.ItemAdded += new EventHandler<WorkspaceEventArgs>(OnWorkspaceAdded);
            _desktopWindow.WorkspaceManager.Workspaces.ItemRemoved += new EventHandler<WorkspaceEventArgs>(OnWorkspaceRemoved);
            _desktopWindow.WorkspaceManager.ActiveWorkspaceChanged += new EventHandler<WorkspaceActivationChangedEventArgs>(OnWorkspaceActivated);

            _dockingManager = new DockingManager(this._toolStripContainer.ContentPanel, VisualStyle.Office2003);
            _dockingManager.ActiveColor = SystemColors.Control;
            _dockingManager.InnerControl = _tabbedGroups;
			_dockingManager.TabControlCreated += new DockingManager.TabControlCreatedHandler(OnDockingManagerTabControlCreated);

			_tabbedGroups.Style = WinFormsView.VisualStyle; 
			_tabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;
			_tabbedGroups.TabControlCreated += new TabbedGroups.TabControlCreatedHandler(OnTabbedGroupsTabControlCreated);

			if (_tabbedGroups.ActiveLeaf != null)
			{
				InitializeTabControl(_tabbedGroups.ActiveLeaf.TabControl);
			}
			
			_workspaceViewManager = new WorkspaceViewManager(this, _tabbedGroups);
			_shelfViewManager = new ShelfViewManager(_desktopWindow.ShelfManager, _dockingManager);

			RebuildMenusAndToolbars();
        }

        private void UpdateTitleBar()
        {
            string title = string.Format("{0} {1}", Application.ApplicationName, Application.ApplicationVersion);
            if (_desktopWindow.WorkspaceManager.ActiveWorkspace != null)
            {
                title = string.Format("{0} - {1}", title, _desktopWindow.WorkspaceManager.ActiveWorkspace.Title);
            }
            this.Text = title;
        }

		internal IDesktopWindow DesktopWindow
		{
			get { return _desktopWindow; }
		}

		protected override void OnLoad(EventArgs e)
		{
			LoadWindowSettings();

			base.OnLoad(e);
		}

		protected override void OnClosing(CancelEventArgs e)
		{
            if (_desktopWindow.CanClose())
            {
                SaveWindowSettings();
            }
            else
            {
                // cancel the request
                e.Cancel = true;
            }

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


		private void InitializeTabControl(Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			if (tabControl == null)
				return;

			tabControl.TextTips = true;
			tabControl.ToolTips = false;
			tabControl.MaximumHeaderWidth = 256;
		}

		void OnTabbedGroupsTabControlCreated(TabbedGroups tabbedGroups, Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			InitializeTabControl(tabControl);
		}

		void OnDockingManagerTabControlCreated(Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			InitializeTabControl(tabControl);
		}
		
		private void OnWorkspaceAdded(object sender, WorkspaceEventArgs e)
        {
			_workspaceViewManager.AddWorkpaceTab(e.Workspace);

            // When we add a new workspace, we need to
            _shelfViewManager.HideShelves();  
        }

        // This is the event handler for when a workspace is removed from the
        // WorkspaceManager.  Not to be confused with OnCloseWorkspaceTab
        private void OnWorkspaceRemoved(object sender, WorkspaceEventArgs e)
        {
			_dockingManager.Container.Focus();
			_workspaceViewManager.RemoveWorkspaceTab(e.Workspace);
        }

        private void OnWorkspaceActivated(object sender, WorkspaceActivationChangedEventArgs e)
        {
            _workspaceViewManager.ActivateWorkspace(e.ActivatedWorkspace);
            UpdateTitleBar();
        }

        internal void RebuildMenusAndToolbars()
        {
			// Suspend the layouts so we avoid the flicker when we empty
			// and refill the menus and toolbars
			this._mainMenu.SuspendLayout();
			this._toolbar.SuspendLayout();
			// very important to clean up the existing ones first
            ToolStripBuilder.Clear(this._mainMenu.Items);
            ToolStripBuilder.Clear(this._toolbar.Items);

            if (_desktopWindow.MenuModel != null)
            {
                ToolStripBuilder.BuildMenu(this._mainMenu.Items, _desktopWindow.MenuModel.ChildNodes);
            }

            if (_desktopWindow.ToolbarModel != null)
            {
                ToolStripBuilder.BuildToolbar(this._toolbar.Items, _desktopWindow.ToolbarModel.ChildNodes, ToolStripItemDisplayStyle.Image);
            }

			this._toolbar.ResumeLayout();
			this._mainMenu.ResumeLayout();
		}

		private string GetVersion()
		{
			int major = Assembly.GetExecutingAssembly().GetName().Version.Major;
			int minor = Assembly.GetExecutingAssembly().GetName().Version.Minor;

			return String.Format("{0}.{1}", major, minor);
	    }
    }
}
