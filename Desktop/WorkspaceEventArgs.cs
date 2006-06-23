using System;
using ClearCanvas.Common;

namespace ClearCanvas.Common.Application
{
	/// <summary>
	/// Summary description for EventArgs.
	/// </summary>
	public class WorkspaceEventArgs : CollectionEventArgs<Workspace>
	{
		public WorkspaceEventArgs()
		{
		}

		public WorkspaceEventArgs(Workspace workspace)
		{
			Platform.CheckForNullReference(workspace, "workspace");

			base.Item = workspace;
		}

		public Workspace Workspace { get { return base.Item; } }
	}
}
