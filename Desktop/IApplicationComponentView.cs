using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface to an application component view as seen by the application component host.
    /// </summary>
    public interface IApplicationComponentView : IView
    {
        /// <summary>
        /// Called by the host to assign this view to a component.
        /// </summary>
        /// <param name="component">The component that this view should serve.</param>
        void SetComponent(IApplicationComponent component);
    }
}
