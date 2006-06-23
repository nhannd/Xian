using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Application.Tools
{
    /// <summary>
    /// Extends the <see cref="IView"/> interface for views that look at tools.
    /// </summary>
    public interface IToolView : IView
    {
        /// <summary>
        /// Sets the tool which the view should look at.
        /// </summary>
        void SetTool(ITool tool);
    }
}
