using System;

namespace ClearCanvas.Desktop
{
	public class WorkspaceActivationChangedEventArgs : EventArgs
	{
        private IWorkspace _deactivatedWorkspace;
        private IWorkspace _activatedWorkspace;

		public WorkspaceActivationChangedEventArgs(IWorkspace activatedWorkspace, IWorkspace deactivatedWorkspace)
		{
            _activatedWorkspace = activatedWorkspace;
            _deactivatedWorkspace = deactivatedWorkspace;
        }

        public IWorkspace ActivatedWorkspace
        {
            get { return _activatedWorkspace; }
        }

        public IWorkspace DeactivatedWorkspace
        {
            get { return _deactivatedWorkspace; }
        }
	}
}
