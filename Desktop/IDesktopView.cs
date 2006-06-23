using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface for the main view of the workstation.
    /// </summary>
    public interface IDesktopView : IView
    {
        /// <summary>
        /// Starts the message pump of the underlying GUI toolkit.  Typically this method is expected to
        /// block for the duration of the application's execution.
        /// </summary>
        /// <remarks>
        /// The method assumes that the view relies on an underlying message pump, as most 
        /// desktop GUI toolkits do.
        /// </remarks>
        void RunMessagePump();

        /// <summary>
        /// Terminates the message pump of the underlying GUI toolkit, typically resulting
        /// in the termination of the application.
        /// </summary>
        void QuitMessagePump();
    }
}
