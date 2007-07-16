using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Defines the public interface to a <see cref="Shelf"/>
    /// </summary>
    public interface IShelf : IDesktopObject
    {
        /// <summary>
        /// Gets the desktop window that owns this shelf.
        /// </summary>
        IDesktopWindow DesktopWindow { get; }

        /// <summary>
        /// Gets the current display hint.
        /// </summary>
        ShelfDisplayHint DisplayHint { get; }

        /// <summary>
        /// Makes the shelf visible.
        /// </summary>
        void Show();

        /// <summary>
        /// Hides the shelf from view.
        /// </summary>
        void Hide();
    }
}
