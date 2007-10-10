using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using Crownwood.DotNetMagic.Common;
using Crownwood.DotNetMagic.Controls;
using Crownwood.DotNetMagic.Docking;
using Crownwood.DotNetMagic.Forms;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Form used by the <see cref="DesktkopWindowView"/> class.
    /// </summary>
    /// <remarks>
    /// This class may be subclassed.
    /// </remarks>
    public partial class DesktopForm : DotNetMagicForm
    {
        private ActionModelNode _menuModel;
        private ActionModelNode _toolbarModel;

		private DockingManager _dockingManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public DesktopForm()
        {
#if !MONO
			SplashScreenManager.DismissSplashScreen(this);
#endif
			InitializeComponent();

            _dockingManager = new DockingManager(_toolStripContainer.ContentPanel, VisualStyle.IDE2005);
            _dockingManager.ActiveColor = SystemColors.Control;
            _dockingManager.InnerControl = _tabbedGroups;
			_dockingManager.TabControlCreated += new DockingManager.TabControlCreatedHandler(OnDockingManagerTabControlCreated);

			_tabbedGroups.DisplayTabMode = DisplayTabModes.HideAll;
			_tabbedGroups.TabControlCreated += new TabbedGroups.TabControlCreatedHandler(OnTabbedGroupsTabControlCreated);

			if (_tabbedGroups.ActiveLeaf != null)
			{
				InitializeTabControl(_tabbedGroups.ActiveLeaf.TabControl);
			}
        }

        #region Public properties

        /// <summary>
        /// Gets or sets the menu model.
        /// </summary>
        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
            set
            {
                _menuModel = value;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Menu, _mainMenu, _menuModel);
            }
        }

        /// <summary>
        /// Gets or sets the toolbar model.
        /// </summary>
        public ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
            set
            {
                _toolbarModel = value;
                BuildToolStrip(ToolStripBuilder.ToolStripKind.Toolbar, _toolbar, _toolbarModel);
            }
        }

        /// <summary>
        /// Gets the <see cref="TabbedGroups"/> object that manages workspace tab groups.
        /// </summary>
        public TabbedGroups TabbedGroups
        {
            get { return _tabbedGroups; }
        }

        /// <summary>
        /// Gets the <see cref="DockingManager"/> object that manages shelf docking windows.
        /// </summary>
        public DockingManager DockingManager
        {
            get { return _dockingManager; }
        }

        #endregion

        #region Form event handlers

        private void OnTabbedGroupsTabControlCreated(TabbedGroups tabbedGroups, Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }

        private void OnDockingManagerTabControlCreated(Crownwood.DotNetMagic.Controls.TabControl tabControl)
        {
            InitializeTabControl(tabControl);
        }


        #endregion

        #region Window Settings

        internal void LoadWindowSettings()
		{
			Rectangle screenRectangle = Screen.PrimaryScreen.Bounds;

			// If the bounds of the primary screen is different from what was saved
			// either because there was a screen resolution change or because the app
			// is being started for the first time, get Windows to properly position the window.
			if (screenRectangle != Settings.PrimaryScreenRectangle)
			{
				// Make the window size 75% of the primary screen
				float scale = 0.75f;
				this.Width = (int) (screenRectangle.Width * scale);
				this.Height = (int) (screenRectangle.Height * scale);

				// Center the window (for some reason, FormStartPosition.CenterScreen doesn't seem
				// to work.)
				//int x = (screenRectangle.Width - this.Width) / 2;
				//int y = (screenRectangle.Height - this.Height) / 2;
				//this.Location = new Point(x, y);
                this.StartPosition = FormStartPosition.CenterScreen;
			}
			else
			{
				this.Location = Settings.WindowRectangle.Location;
				this.Size = Settings.WindowRectangle.Size;
                this.StartPosition = FormStartPosition.Manual;
			}

			// If window was last closed when minimized, don't open it up minimized,
			// but rather just open it normally
			if (Settings.WindowState == FormWindowState.Minimized)
				this.WindowState = FormWindowState.Normal;
			else
				this.WindowState = Settings.WindowState;
		}

		internal void SaveWindowSettings()
		{
			// If the window state is normal, just save its location and size
			if (this.WindowState == FormWindowState.Normal)
				Settings.WindowRectangle = new Rectangle(this.Location, this.Size);
			// But, if it's minimized or maximized, save the restore bounds instead
			else
				Settings.WindowRectangle = this.RestoreBounds;

			Settings.WindowState = this.WindowState;
			Settings.PrimaryScreenRectangle = Screen.PrimaryScreen.Bounds;
			Settings.Save();
        }

        #endregion

        #region Helper methods

        internal DesktopViewSettings Settings
        {
            get { return DesktopViewSettings.Default; }
        }

        /// <summary>
        /// Called to initialize a <see cref="Crownwood.DotNetMagic.Controls.TabControl"/>. Override
        /// this method to perform custom initialization.
        /// </summary>
        /// <param name="tabControl"></param>
        protected virtual void InitializeTabControl(Crownwood.DotNetMagic.Controls.TabControl tabControl)
		{
			if (tabControl == null)
				return;

			tabControl.TextTips = true;
			tabControl.ToolTips = false;
			tabControl.MaximumHeaderWidth = 256;
        }

        /// <summary>
        /// Called to build menus and toolbars.  Override this method to customize menu and toolbar building.
        /// </summary>
        /// <remarks>
        /// The default implementation simply clears and re-creates the toolstrip using methods on the
        /// utility class <see cref="ToolStripBuilder"/>.
        /// </remarks>
        /// <param name="kind"></param>
        /// <param name="toolStrip"></param>
        /// <param name="actionModel"></param>
        protected virtual void BuildToolStrip(ToolStripBuilder.ToolStripKind kind, ToolStrip toolStrip, ActionModelNode actionModel)
        {
            // avoid flicker
            toolStrip.SuspendLayout();
            // very important to clean up the existing ones first
            ToolStripBuilder.Clear(toolStrip.Items);

            if (actionModel != null)
            {
                ToolStripBuilder.BuildToolStrip(kind, toolStrip.Items, actionModel.ChildNodes);
            }

            toolStrip.ResumeLayout();
        }

        #endregion
    }
}
