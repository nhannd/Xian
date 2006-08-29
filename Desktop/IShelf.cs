using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    public interface IShelf : IDisposable
    {

        /// <summary>
        /// Called by the framework when the shelf is added to a <see cref="ShelfManager"/>
        /// </summary>
        void Initialize(IDesktopWindow desktopWindow);

        /// <summary>
        /// Gets or sets the desktop window that owns this shelf.  
        /// </summary>
        IDesktopWindow DesktopWindow { get; }

        /// <summary>
        /// Indicates that the <see cref="IShelf.Title"/> property has changed.
        /// </summary>
        event EventHandler TitleChanged;

        /// <summary>
        /// Returns the title that should be displayed for the shelf in the user-interface
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// Returns a set of flags that provide hints as to how the shelf wishes to be displayed
        /// </summary>
        ShelfDisplayHint DisplayHint { get; }
    }
}
