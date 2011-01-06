#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
	/// <summary>
    /// Defines the public interface to a <see cref="Shelf"/>.
    /// </summary>
    /// <remarks>
    /// This interface exists mainly for backward compatibility.  New application
    /// code should use the <see cref="Shelf"/> class.
    /// </remarks>
    public interface IShelf : IDesktopObject
    {
        /// <summary>
        /// Gets the desktop window that owns this shelf.
        /// </summary>
        IDesktopWindow DesktopWindow { get; }

        /// <summary>
        /// Gets the hosted component.
        /// </summary>
        object Component { get; }

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
