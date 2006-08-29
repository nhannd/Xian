using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop 
{
	/// <summary>
	/// Manages workspaces.
	/// </summary>
	/// <remarks>
	/// <b>WorkspaceManager</b> manages a collection of <see cref="Workspace"/> objects.
	/// It is accessed via the <see cref="ModelPlugin.WorkspaceManager"/> static property.  
	/// </remarks>
	public class WorkspaceManager : IDisposable
	{
        private IDesktopWindow _desktopWindow;
        private event EventHandler<WorkspaceActivationChangedEventArgs> _activeWorkspaceChanged;
		private IWorkspace _activeWorkspace;
        private WorkspaceCollection _workspaces;

		internal WorkspaceManager(IDesktopWindow desktopWindow)
		{
            _desktopWindow = desktopWindow;
            _workspaces = new WorkspaceCollection(this);
		}

        ~WorkspaceManager()
        {
            Dispose(false);
        }

        protected void Dispose(bool disposing)
        {
            foreach (IWorkspace workspace in _workspaces)
            {
                workspace.Dispose();
            }
            _workspaces.Clear();
        }

        public WorkspaceCollection Workspaces
		{
			get { return _workspaces; }
		}

		/// <summary>
		/// Gets or sets the currently active <see cref="Workspace"/>.
		/// </summary>
		/// <remarks>
        /// This property may return <b>null</b> in the case where there are no workspaces.  However,
        /// attempting to set it to <b>null</b> will throw an exception; there must always be an active workspace.
        /// When a new workspace is added, that workspace is set as active.
		/// </remarks>
		/// <value>The currently active <see cref="Workspace"/> or <b>null</b> if
		/// there are no workspaces in the <see cref="WorkspaceManager"/>.</value>
		/// <exception cref="ArgumentNullException"><paramref name="ActiveWorkspace"/> is set to <b>null</b>.</exception>
		public IWorkspace ActiveWorkspace
		{
			get
			{
				return _activeWorkspace;
			}
			set
			{
				// Don't bother if nothing's changed
                if (value != _activeWorkspace)
                {
                    if (value == null)
                        throw new ArgumentNullException("Cannot set the active workspace to null");

                    InternalSetActiveWorkspace(value);
                }
			}
		}

		/// <summary>
		/// Occurs when the value of the <see cref="ActiveWorkspace"/> property changes.
		/// </summary>
		/// <remarks>The event handler receives an argument of type <see cref="WorkspaceEventArgs"/>.</remarks>
        public event EventHandler<WorkspaceActivationChangedEventArgs> ActiveWorkspaceChanged
		{
			add { _activeWorkspaceChanged += value; }
			remove { _activeWorkspaceChanged -= value; }
		}

		internal void WorkspaceAdded(IWorkspace workspace)
		{
            // initialize the new workspace and make it the active workspace
            workspace.Initialize(_desktopWindow);
            this.ActiveWorkspace = workspace;
		}

        internal void WorkspaceRemoved(IWorkspace workspace)
		{
            // dispose of the workspace
            workspace.Dispose();

			// Make sure to set the active workspace to null if there are no more workspaces
			if (_workspaces.Count == 0)
                InternalSetActiveWorkspace(null);
		}

        private void InternalSetActiveWorkspace(IWorkspace workspace)
        {
            IWorkspace old = _activeWorkspace;
            _activeWorkspace = workspace;

            EventsHelper.Fire(_activeWorkspaceChanged, this, new WorkspaceActivationChangedEventArgs(_activeWorkspace, old));
        }

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}