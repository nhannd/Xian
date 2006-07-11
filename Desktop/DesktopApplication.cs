using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Desktop
{
    [ExtensionPoint()]
    public class DesktopViewExtensionPoint : ExtensionPoint<IDesktopView>
    {
    }

    /// <summary>
    /// Defines an extension point for tools that are applicable to the desktop as a whole.
    /// </summary>
    [ExtensionPoint()]
    public class DesktopToolExtensionPoint : ExtensionPoint<ITool>
    {
    }



    [ClearCanvas.Common.ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class DesktopApplication : IApplicationRoot
    {
        private static DesktopApplication _instance;
        private static WorkspaceManager _workspaceManager;
        private static ToolSet _toolSet;
        private static IDesktopView _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkstationModel"/> class.
        /// </summary>
        public DesktopApplication()
        {
            _instance = this;

            // Create the view
            DesktopViewExtensionPoint xp = new DesktopViewExtensionPoint();
            _view = (IDesktopView)xp.CreateExtension();
		}

        /// <summary>
        /// Runs the application by running the view's message pump.  Typically this method will
        /// block until the message pump terminates.
        /// </summary>
        public void RunApplication(string[] args)
        {
            _view.RunMessagePump();
        }

        /// <summary>
        /// Returns the view associated with this <see cref="WorkstationModel"/>.
        /// </summary>
        public static IDesktopView View
        {
            get { return _view; }
        }

        public static string ApplicationName
        {
            get { return SR.ApplicationName; }
        }

        public static string ApplicationVersion
        {
            get { return SR.ApplicationVersion; }
        }

        /// <summary>
        /// Gets the <see cref="WorkspaceManager"/>.
        /// </summary>
        public static WorkspaceManager WorkspaceManager
        {
            get
            {
                if (_workspaceManager == null)
                    _workspaceManager = new WorkspaceManager();

                return _workspaceManager;
            }
        }


        /// <summary>
        /// Gets the collection of workstation tools.
        /// </summary>
        public static ToolSet ToolSet
        {
            get
            {
                if (_toolSet == null)
                    CreateDesktopTools();

                return _toolSet;
            }
        }
		
		/// <summary>
        /// Gets the currently active <see cref="Workspace"/>.
        /// </summary>
        /// <value>The currently active <see cref="Workspace"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/>.</value>
        public static IWorkspace ActiveWorkspace
        {
            get
            {
                return WorkspaceManager.ActiveWorkspace;
            }
        }

        private static void CreateDesktopTools()
        {
            if (_toolSet != null)
                return;
            _toolSet = new ToolSet(new DesktopToolExtensionPoint(), new ToolContext());
            _toolSet.Activate(true);
        }
    }
}
