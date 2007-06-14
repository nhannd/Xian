using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop 
{
	/// <summary>
    /// Manages a collection of <see cref="IWorkspace"/> objects.
    /// </summary>
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

        /// <summary>
        /// Implementation of the <see cref="IDisposable"/> pattern
        /// </summary>
        /// <param name="disposing">True if this object is being disposed, false if it is being finalized</param>
        protected void Dispose(bool disposing)
        {
            if (disposing && _workspaces != null)
            {
                _workspaces.Clear();
                _workspaces = null;
            }
        }

        /// <summary>
        /// The collection of <see cref="IShelf"/> objects that are currently being managed
        /// </summary>
        public WorkspaceCollection Workspaces
		{
			get { return _workspaces; }
		}

		/// <summary>
		/// Gets or sets the currently active <see cref="IWorkspace"/>.
		/// </summary>
		/// <remarks>
        /// This property may return <b>null</b> in the case where there are no workspaces.  However,
        /// attempting to set it to <b>null</b> will throw an exception; there must always be an active workspace.
        /// When a new workspace is added, that workspace is set as active.
		/// </remarks>
		/// <value>The currently active workspace or <b>null</b> if
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
						throw new ArgumentNullException(SR.ExceptionActiveWorkspaceCannotBeNull);

                    InternalSetActiveWorkspace(value);
                }
			}
		}

		/// <summary>
		/// Occurs when the value of the <see cref="ActiveWorkspace"/> property changes.
		/// </summary>
        /// <remarks>The event handler receives an argument of type <see cref="WorkspaceActivationChangedEventArgs"/>.</remarks>
        public event EventHandler<WorkspaceActivationChangedEventArgs> ActiveWorkspaceChanged
		{
			add { _activeWorkspaceChanged += value; }
			remove { _activeWorkspaceChanged -= value; }
		}

		internal void WorkspaceAdded(IWorkspace workspace)
		{
            // initialize the new workspace and make it the active workspace
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

        #endregion
    }
}