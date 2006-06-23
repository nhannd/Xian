using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Common.Application.Tools;

namespace ClearCanvas.Common.Application
{
    [ExtensionPoint()]
    public class WorkstationViewExtensionPoint : ExtensionPoint<IWorkstationView>
    {
    }

    [ClearCanvas.Common.ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class WorkstationModel : IApplicationRoot
    {
        private static WorkstationModel _instance;
        private static WorkspaceManager _workspaceManager;
        private static ToolManager _toolManager;
        private static ToolContext _workbenchToolContext;
        private static IWorkstationView _view;

        /// <summary>
        /// Initializes a new instance of the <see cref="WorkstationModel"/> class.
        /// </summary>
        public WorkstationModel()
        {
            _instance = this;

            // Create the view
            WorkstationViewExtensionPoint xp = new WorkstationViewExtensionPoint();
            _view = (IWorkstationView)xp.CreateExtension();
		}

        /// <summary>
        /// Runs the application by running the view's message pump.  Typically this method will
        /// block until the message pump terminates.
        /// </summary>
        public void RunApplication()
        {
            _view.RunMessagePump();
        }

        /// <summary>
        /// Returns the view associated with this <see cref="WorkstationModel"/>.
        /// </summary>
        public static IWorkstationView View
        {
            get { return _view; }
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
        public static ToolManager ToolManager
        {
            get
            {
                if (_toolManager == null)
                    CreateWorkstationTools();

                return _toolManager;
            }
        }
		
		/// <summary>
        /// Gets the currently active <see cref="Workspace"/>.
        /// </summary>
        /// <value>The currently active <see cref="Workspace"/>, or <b>null</b> if there are
        /// no workspaces in the <see cref="WorkspaceManager"/>.</value>
        public static Workspace ActiveWorkspace
        {
            get
            {
                return WorkspaceManager.ActiveWorkspace;
            }
        }

        private static void CreateWorkstationTools()
        {
            if (_toolManager != null)
                return;
            _workbenchToolContext = new WorkstationToolContext();
            _toolManager = new ToolManager(_workbenchToolContext);
            _workbenchToolContext.Activate(true);
        }
    }
}
