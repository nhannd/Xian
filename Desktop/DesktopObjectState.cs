using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the possible states for a <see cref="DesktopObject"/>
    /// </summary>
    public enum DesktopObjectState
    {
        /// <summary>
        /// The object has been created, but not opened.
        /// </summary>
        New,

        /// <summary>
        /// The object is in the process of opening.
        /// </summary>
        Opening,

        /// <summary>
        /// The object is open.
        /// </summary>
        Open,

        /// <summary>
        /// The object is in the process of closing, but may return to the <see cref="Open"/> state if the operation is cancelled.
        /// </summary>
        Closing,

        /// <summary>
        /// The object has closed and is disposed.
        /// </summary>
        Closed
    }
}
