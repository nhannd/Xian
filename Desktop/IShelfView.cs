using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the interface for a view onto a shelf, as seen by the desktop.
    /// </summary>
    public interface IShelfView : IView
    {
        /// <summary>
        /// Sets the shelf that this view looks at
        /// </summary>
        /// <param name="shelf">The shelf to look at</param>
        void SetShelf(IShelf shelf);
    }
}
