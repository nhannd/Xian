using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface for a view onto a <see cref="IDesktopWindow"/>, as seen by the <see cref="Application"/>.
    /// This interface is preliminary and subject to change.
    /// </summary>
    public interface IDesktopWindowView : IView
    {
        /// <summary>
        /// Set the window which this view looks at.
        /// </summary>
        /// <param name="window"></param>
        void SetDesktopWindow(IDesktopWindow window);

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
