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
    /// Defines an extension point for tools that are applicable to a desktop window.
    /// </summary>
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
        /// Gets the desktop window that the tool acts on
        /// </summary>
        DesktopWindow DesktopWindow { get; }
    }

    #endregion

    /// <summary>
    /// Represents an application window.
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

        protected const string GlobalMenus = "global-menus";
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
            _workspaces = CreateWorkspaceCollection();
            _shelves = CreateShelfCollection();
            _dialogs = CreateDialogBoxCollection();

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
            AssertState(new DesktopObjectState[] { DesktopObjectState.Open, DesktopObjectState.Closing });

            return this.DesktopWindowView.ShowMessageBox(message, buttons);
        }

        /// <summary>
        /// Shows a dialog box in front of this window.
        /// </summary>
        /// <param name="component"></param>
        /// <param name="title"></param>
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
        /// <param name="args"></param>
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

        protected internal override bool CanClose(UserInteraction interactive)
        {
            // should never be called in this mode
            if (interactive == UserInteraction.Allowed)
                throw new InvalidOperationException();

            // we can close if all workspaces can close without interacting
            return CollectionUtils.TrueForAll<Workspace>(_workspaces,
                delegate(Workspace w) { return w.CanClose(UserInteraction.NotAllowed); });
        }

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

        protected virtual string MakeTitle(string baseTitle, Workspace activeWorkspace)
        {
            if (activeWorkspace != null)
            {
                return string.Format("{0} - {1}", baseTitle, activeWorkspace.Title);
            }
            else
            {
                return baseTitle;
            }
        }
        
        protected override IDesktopObjectView CreateView()
        {
            return _application.OpenWindowView(this);
        }

        #endregion

        #region Helpers

        private WorkspaceCollection CreateWorkspaceCollection()
        {
            return new WorkspaceCollection(this);
        }

        private ShelfCollection CreateShelfCollection()
        {
            return new ShelfCollection(this);
        }

        private DialogBoxCollection CreateDialogBoxCollection()
        {
            return new DialogBoxCollection(this);
        }

        internal IWorkspaceView CreateWorkspaceView(Workspace workspace)
        {
            return this.DesktopWindowView.CreateWorkspaceView(workspace);
        }

        internal IShelfView CreateShelfView(Shelf shelf)
        {
            return this.DesktopWindowView.CreateShelfView(shelf);
        }

        internal IDialogBoxView CreateDialogView(DialogBox dialog)
        {
            return this.DesktopWindowView.CreateDialogBoxView(dialog);
        }

        protected IDesktopWindowView DesktopWindowView
        {
            get { return (IDesktopWindowView)this.View; }
        }

        protected IToolSet DesktopTools
        {
            get { return _desktopTools; }
        }

        protected ActionModelNode MenuModel
        {
            get { return _menuModel; }
        }

        protected ActionModelNode ToolbarModel
        {
            get { return _toolbarModel; }
        }

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
