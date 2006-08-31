using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines an extension point for views onto a desktop window
    /// </summary>
    [ExtensionPoint()]
    public class DesktopWindowViewExtensionPoint : ExtensionPoint<IDesktopWindowView>
    {
    }

    /// <summary>
    /// Defines an extension point for tools that are applicable to the desktop as a whole.
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
        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// Implementation of <see cref="IDesktopWindow"/>
    /// </summary>
    [AssociateView(typeof(DesktopWindowViewExtensionPoint))]
    public class DesktopWindow : ClearCanvas.Desktop.IDesktopWindow
    {
        class DesktopToolContext : ToolContext, IDesktopToolContext
        {
            private IDesktopWindow _window;

            internal DesktopToolContext(IDesktopWindow window)
            {
                _window = window;
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _window; }
            }
        }

        private WorkspaceManager _workspaceManager;
        private ShelfManager _shelfManager;
        private IToolSet _desktopTools;
        
        /// <summary>
        /// Constructor
        /// </summary>
        public DesktopWindow()
        {
		}

        public void Dispose()
        {
            try
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
            catch (Exception e)
            {
                // shouldn't throw anything from inside Dispose()
                Platform.Log(e);
            }
        }

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_workspaceManager != null)
                {
                    _workspaceManager.Dispose();
                    _workspaceManager = null;
                }

                if (_shelfManager != null)
                {
                    _shelfManager.Dispose();
                    _shelfManager = null;
                }

                if (_desktopTools != null)
                {
                    _desktopTools.Dispose();
                    _desktopTools = null;
                }
            }
        }

        /// <summary>
        /// Gets the workspace manager associated with this desktop window
        /// </summary>
        public WorkspaceManager WorkspaceManager
        {
            get
            {
                if (_workspaceManager == null)
                    _workspaceManager = new WorkspaceManager(this);

                return _workspaceManager;
            }
        }

        /// <summary>
        /// Gets the shelf manager associated with this desktop window
        /// </summary>
        public ShelfManager ShelfManager
        {
            get
            {
                if (_shelfManager == null)
                    _shelfManager = new ShelfManager(this);

                return _shelfManager;
            }
        }

        /// <summary>
        /// Gets the current menu model for this desktop window.  Note that the menu
        /// model changes depending on the currently active workspace.  Therefore
        /// the return value of this property should not be cached.
        /// </summary>
        public ActionModelNode MenuModel
        {
            get
            {
                return GetActionModel(ActionPath.GlobalMenus);
            }
        }

        /// <summary>
        /// Gets the current menu model for this desktop window.  Note that the menu
        /// model changes depending on the currently active workspace.  Therefore
        /// the return value of this property should not be cached.
        /// </summary>
        public ActionModelNode ToolbarModel
        {
            get
            {
                return GetActionModel(ActionPath.GlobalToolbars);
            }
        }

        /// <summary>
        /// Gets the currently active <see cref="IWorkspace"/>.
        /// </summary>
        /// <value>The currently active <see cref="IWorkspace"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/>.</value>
        public IWorkspace ActiveWorkspace
        {
            get
            {
                return this.WorkspaceManager.ActiveWorkspace;
            }
        }

        /// <summary>
        /// Calls the <see cref="IWorkspace.CanClose"/> method on all existing workspaces. If any workspace
        /// returns false, this method returns false.
        /// </summary>
        /// <returns>True if all workspaces indicate they are in a closable state, otherwise false</returns>
        public bool CanClose()
        {
            foreach (IWorkspace workspace in this.WorkspaceManager.Workspaces)
            {
                // make this workspace active, so the user is not confused if it brings up a message box
                workspace.Activate();

                if (!workspace.CanClose())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Builds the action model for the specified action site.
        /// </summary>
        /// <param name="site"></param>
        /// <returns></returns>
        private ActionModelNode GetActionModel(string site)
        {
            ActionSelectorDelegate selector = delegate(IAction action) { return (action.Path.Site == site); };

            IActionSet actions = this.DesktopTools.Actions.Select(selector);

            if (this.ActiveWorkspace != null)
            {
                actions = actions.Add(this.ActiveWorkspace.Actions.Select(selector));
            }

            return ActionModelRoot.CreateModel(typeof(DesktopWindow).FullName, site, actions).ChildNodes[site];
        }


        /// <summary>
        /// Gets the collection of desktop tools.
        /// </summary>
        private IToolSet DesktopTools
        {
            get
            {
                if (_desktopTools == null)
                {
                    _desktopTools = new ToolSet(new DesktopToolExtensionPoint(), new DesktopToolContext(this));
                }

                return _desktopTools;
            }
        }
		
    }
}
