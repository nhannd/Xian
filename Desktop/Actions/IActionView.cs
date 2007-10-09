using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Actions
{
    /// <summary>
    /// Defines the interface for a view onto an action.
    /// </summary>
    public interface IActionView : IView
    {
        /// <summary>
        /// Called by the framework to set the action that the view looks at.
        /// </summary>
        /// <param name="action"></param>
        void SetAction(IAction action);
    }
}
