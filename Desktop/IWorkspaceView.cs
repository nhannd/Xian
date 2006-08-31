using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface for a view onto a workspace, as seen by the desktop.
    /// </summary>
    public interface IWorkspaceView : IView
	{
        /// <summary>
        /// Sets the workspace which the view looks at.
        /// </summary>
        void SetWorkspace(IWorkspace workspace);
	}
}
