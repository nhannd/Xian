using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application
{
    /// <summary>
    /// Extends the <see cref="IView"/> interface for views that look at workspaces.
    /// </summary>
    public interface IWorkspaceView : IView
	{
        /// <summary>
        /// Sets the workspace which the view should look at.
        /// </summary>
        void SetWorkspace(Workspace workspace);
	}
}
