using System;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Provides information about a change in workspace activation.
    /// </summary>
	public class WorkspaceActivationChangedEventArgs : EventArgs
	{
        private IWorkspace _deactivatedWorkspace;
        private IWorkspace _activatedWorkspace;

		public WorkspaceActivationChangedEventArgs(IWorkspace activatedWorkspace, IWorkspace deactivatedWorkspace)
		{
            _activatedWorkspace = activatedWorkspace;
            _deactivatedWorkspace = deactivatedWorkspace;
        }

        /// <summary>
        /// Gets the workspace that was activated
        /// </summary>
        public IWorkspace ActivatedWorkspace
        {
            get { return _activatedWorkspace; }
        }

        /// <summary>
        /// Gets the workspace that was deactivated (the previously active workspace)
        /// </summary>
        public IWorkspace DeactivatedWorkspace
        {
            get { return _deactivatedWorkspace; }
        }
	}
}
