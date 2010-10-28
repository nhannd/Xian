#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Represents a single or multiple selection.
    /// </summary>
    public interface ISelection
    {
        /// <summary>
        /// Returns the set of items that are currently selected.
        /// </summary>
        object[] Items { get; }

        /// <summary>
        /// Convenience method to obtain the currently selected item in a single-select scenario.
        /// </summary>
		/// <remarks>
		/// If no rows are selected, the method returns null.  If more than one row is selected,
        /// it is undefined which item will be returned.
		/// </remarks>
		object Item { get; }

		/// <summary>
		/// Computes the union of this selection with another and returns it.
		/// </summary>
        ISelection Union(ISelection other);

		/// <summary>
		/// Computes the intersection of this selection with another and returns it.
		/// </summary>
		ISelection Intersect(ISelection other);
        
		/// <summary>
		/// Returns an <see cref="ISelection"/> that contains every item contained
		/// in this one that doesn't exist in <param name="other" />.
		/// </summary>
		ISelection Subtract(ISelection other);

		/// <summary>
		/// Determines whether this selection contains the input object.
		/// </summary>
		bool Contains(object item);
    }
}
