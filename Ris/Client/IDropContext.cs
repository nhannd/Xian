using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Desktop;

namespace ClearCanvas.Ris.Client
{
    /// <summary>
    /// Defines the base drop context interface for use with <see cref="IDropHandler"/>.  Normally this interface will
    /// be extended to provide the drop handler with additional data.  The drop handler will have to cast to the expected type.
    /// </summary>
    public interface IDropContext
    {
        /// <summary>
        /// Gets the desktop window in which the drop is occuring.
        /// </summary>
        IDesktopWindow DesktopWindow { get; }
    }
}
