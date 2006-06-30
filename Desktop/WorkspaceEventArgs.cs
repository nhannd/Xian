using System;
using ClearCanvas.Common;

namespace ClearCanvas.Desktop
{
	/// <summary>
	/// Summary description for EventArgs.
	/// </summary>
    public class WorkspaceEventArgs : CollectionEventArgs<IWorkspace>
	{
		public WorkspaceEventArgs()
		{
		}

        public WorkspaceEventArgs(IWorkspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			base.Item = workspace;
		}

        public IWorkspace Workspace
        {
            get { return base.Item; }
        }
	}
}
