using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Desktop
{
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

    public interface IDesktopToolContext : IToolContext
    {
        IDesktopWindow DesktopWindow { get; }
    }

    public class DesktopToolContext : ToolContext, IDesktopToolContext
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


    [AssociateView(typeof(DesktopWindowViewExtensionPoint))]
    public class DesktopWindow : ClearCanvas.Desktop.IDesktopWindow
    {
        private WorkspaceManager _workspaceManager;
        private ShelfManager _shelfManager;
        private IToolSet _desktopTools;
        
        public DesktopWindow()
        {
		}

        ~DesktopWindow()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
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
        }

        public WorkspaceManager WorkspaceManager
        {
            get
            {
                if (_workspaceManager == null)
                    _workspaceManager = new WorkspaceManager(this);

                return _workspaceManager;
            }
        }

        public ShelfManager ShelfManager
        {
            get
            {
                if (_shelfManager == null)
                    _shelfManager = new ShelfManager(this);

                return _shelfManager;
            }
        }

        public ActionModelNode MenuModel
        {
            get
            {
                return GetActionModel(ActionPath.GlobalMenus);
            }
        }

        public ActionModelNode ToolbarModel
        {
            get
            {
                return GetActionModel(ActionPath.GlobalToolbars);
            }
        }

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
    }
}
