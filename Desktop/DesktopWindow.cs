using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop
{
    #region Extension points

    /// <summary>
    /// Defines an extension point for desktop tools, which are instantiated once per desktop window.
    /// </summary>
    /// <remarks>
    /// Desktop tools are owned by a desktop window. A desktop tool is instantiated once per desktop window.
    /// Extensions should expect to recieve a tool context of type <see cref="IDesktopToolContext"/>.
    /// </remarks>
    [ExtensionPoint()]
    public class DesktopToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Tool context interface provided to tools that extend <see cref="DesktopToolExtensionPoint"/>
    /// </summary>
    public interface IDesktopToolContext : IToolContext
    {
        /// <summary>
        /// Gets the desktop window that the tool is associated with.
        /// </summary>
        DesktopWindow DesktopWindow { get; }
    }

    #endregion

    /// <summary>
    /// Represents a desktop window (an application main window).
    /// </summary>
    public class DesktopWindow : DesktopObject, IDesktopWindow
    {
        #region DesktopToolContext

        class DesktopToolContext : ToolContext, IDesktopToolContext
        {
            private DesktopWindow _window;

            internal DesktopToolContext(DesktopWindow window)
            {
                _window = window;
            }

            public DesktopWindow DesktopWindow
            {
                get { return _window; }
            }
        }

        #endregion

        /// <summary>
        /// Defines the global menu action site.
        /// </summary>
        protected const string GlobalMenus = "global-menus";

        /// <summary>
        /// Defines the global toolbar action site.
        /// </summary>
        protected const string GlobalToolbars = "global-toolbars";

        private Application _application;
        private WorkspaceCollection _workspaces;
        private ShelfCollection _shelves;
        private DialogBoxCollection _dialogs;
        private string _baseTitle;

        private IToolSet _desktopTools;
        private string _menuActionSite;
        private string _toolbarActionSite;

        private ActionModelNode _menuModel;
        private ActionModelNode _toolbarModel;
        
        /// <summary>
        /// Constructor
        /// </summary>
        protected internal DesktopWindow(DesktopWindowCreationArgs args, Application application)
            :base(args)
        {
            _application = application;
            _workspaces = new WorkspaceCollection(this);
            _shelves = new ShelfCollection(this);
            _dialogs = new DialogBoxCollection(this);

            // if no title supplied, create default title
            _baseTitle = !string.IsNullOrEmpty(args.Title) ?
                args.Title : string.Format("{0} {1}.{2}",
                    Application.Name, Application.Version.Major, Application.Version.Minor);

            _menuActionSite = args.MenuSite ?? GlobalMenus;
            _toolbarActionSite = args.ToolbarSite ?? GlobalToolbars;
        }

        #region Public properties

        /// <summary>
        /// Gets the collection of workspaces associated with this window.
        /// </summary>
        public WorkspaceCollection Workspaces
        {
            get { return _workspaces; }
        }

        /// <summary>
        /// Gets the currently active workspace, or null if there are no workspaces.
        /// </summary>
        public Workspace ActiveWorkspace
        {
            get { return _workspaces.ActiveWorkspace; }
        }

        /// <summary>
        /// Gets the collection of shelves associated with this window.
        /// </summary>
        public ShelfCollection Shelves
        {
            get { return _shelves; }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Shows a message box in front of this window.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public DialogBoxAction ShowMessageBox(string message, MessageBoxActions buttons)
        {
            return this.DesktopWindowView.ShowMessageBox(message, null, buttons);
        }

        public DialogBoxAction ShowMessageBox(string message, string title, MessageBoxActions buttons)
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open, DesktopObjectState.Closing });

            return this.DesktopWindowView.ShowMessageBox(message, title, buttons);
        }

        /// <summary>
        /// Shows a dialog box in front of this window.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public DialogBoxAction ShowDialogBox(DialogBoxCreationArgs args)
        {
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open, DesktopObjectState.Closing });

            DialogBox dialog = _dialogs.AddNew(args);
            return dialog.RunModal();
        }

        /// <summary>
        /// Shows a dialog box in front of this window.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public DialogBoxAction ShowDialogBox(IApplicationComponent component, string title)
        {
            return ShowDialogBox(new DialogBoxCreationArgs(component, title, null));
        }

        #endregion

        #region Protected overridables

        protected override void OnOpened(EventArgs args)
        {
            // note that we can't do this initialize in the Initialize override because the view has not been created yet
            // initialization of desktop tools must occur after the desktop view has been created
            _desktopTools = new ToolSet(new DesktopToolExtensionPoint(), new DesktopToolContext(this));

            // initialize menu and toolbar models
            UpdateView();

            // when the active workspace changes, assume that the menu/toolbar models have also changed
            _workspaces.ItemActivationChanged += delegate(object sender, ItemEventArgs<Workspace> e)
            {
                UpdateView();
            };

            base.OnOpened(args);
        }

        /// <summary>
        /// Checks if all workspaces can close.
        /// </summary>
        /// <param name="interactive"></param>
        /// <returns></returns>
        protected internal override bool CanClose(UserInteraction interactive)
        {
            // should never be called in this mode
            if (interactive == UserInteraction.Allowed)
                throw new InvalidOperationException();

            // we can close if all workspaces can close without interacting
            return CollectionUtils.TrueForAll<Workspace>(_workspaces,
                delegate(Workspace w) { return w.CanClose(UserInteraction.NotAllowed); });
        }

        /// <summary>
        /// Attempts to close all workspaces and shelves.
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected override bool PrepareClose(CloseReason reason)
        {
            List<Workspace> workspaces = new List<Workspace>(_workspaces);
            foreach (Workspace workspace in workspaces)
            {
                // try to close it
                if (!workspace.Close(UserInteraction.Allowed, reason))
                    return false;
            }

            List<Shelf> shelves = new List<Shelf>(_shelves);
            foreach (Shelf shelf in shelves)
            {
                // close it
                shelf.Close(UserInteraction.Allowed, reason);
            }

            return true;
        }

        /// <summary>
        /// Disposes of this object.
        /// </summary>
        /// <param name="disposing"></param>
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                if (_workspaces != null)
                {
                    (_workspaces as IDisposable).Dispose();
                    _workspaces = null;
                }

                if (_shelves != null)
                {
                    (_shelves as IDisposable).Dispose();
                    _shelves = null;
                }

                if (_desktopTools != null)
                {
                    (_desktopTools as IDisposable).Dispose();
                    _desktopTools = null;
                }
            }
        }

        /// <summary>
        /// Creates the title that is displayed in the title bar.  Override this method to customize the title. 
        /// </summary>
        /// <param name="baseTitle"></param>
        /// <param name="activeWorkspace"></param>
        /// <returns></returns>
        protected virtual string MakeTitle(string baseTitle, Workspace activeWorkspace)
        {
            if (activeWorkspace != null)
            {
				return string.Format(SR.FormatDesktopWindowTitle, activeWorkspace.Title, baseTitle);
            }
            else
            {
                return baseTitle;
            }
        }
        
        /// <summary>
        /// Creates a view for this object.
        /// </summary>
        /// <returns></returns>
        protected sealed override IDesktopObjectView CreateView()
        {
            return _application.CreateDesktopWindowView(this);
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Creates a workspace view for the specified workspace.
        /// </summary>
        /// <param name="workspace"></param>
        /// <returns></returns>
        internal IWorkspaceView CreateWorkspaceView(Workspace workspace)
        {
            return this.DesktopWindowView.CreateWorkspaceView(workspace);
        }

        /// <summary>
        /// Creates a shelf view for the specified shelf.
        /// </summary>
        /// <param name="shelf"></param>
        /// <returns></returns>
        internal IShelfView CreateShelfView(Shelf shelf)
        {
            return this.DesktopWindowView.CreateShelfView(shelf);
        }

        /// <summary>
        /// Creates a dialog box view for the specified dialog box.
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        internal IDialogBoxView CreateDialogView(DialogBox dialog)
        {
            return this.DesktopWindowView.CreateDialogBoxView(dialog);
        }

        /// <summary>
        /// Gets the view for this object as an <see cref="IDesktopWindowView"/>
        /// </summary>
        protected IDesktopWindowView DesktopWindowView
        {
            get { return (IDesktopWindowView)this.View; }
        }

        /// <summary>
        /// Gets the tool set associated with this desktop window.
        /// </summary>
        protected IToolSet DesktopTools
        {
            get { return _desktopTools; }
        }

        /// <summary>
        /// Gets the current menu model.
        /// </summary>
        protected ActionModelNode MenuModel
        {
            get { return _menuModel; }
        }

        /// <summary>
        /// Gets the current toolbar model.
        /// </summary>
        protected ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
        }

        /// <summary>
        /// Updates the view's title, menu and toolbars.
        /// </summary>
        private void UpdateView()
        {
            if (this.DesktopWindowView != null)
            {
                this.Title = MakeTitle(_baseTitle, _workspaces.ActiveWorkspace);

                _menuModel = BuildActionModel(_menuActionSite);
                _toolbarModel = BuildActionModel(_toolbarActionSite);

                this.DesktopWindowView.SetMenuModel(_menuModel);
                this.DesktopWindowView.SetToolbarModel(_toolbarModel);
            }
        }

        /// <summary>
        /// Builds the action model for the specified site.
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        private ActionModelNode BuildActionModel(string site)
        {
            IActionSet actions = this.DesktopTools.Actions;
            if (this.ActiveWorkspace != null)
            {
                actions = actions.Union(this.Workspaces.ActiveWorkspace.Actions);
            }

            return ActionModelRoot.CreateModel(typeof(DesktopWindow).FullName, site, actions);
        }


        #endregion
    }
}
