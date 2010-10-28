#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.Common
{
    /// <summary>
    /// Interface defining a factory for extensions of arbitrary <see cref="ExtensionPoint"/>s.
    /// </summary>
    /// <remarks>
    /// Implementations of this interface are expected to be thread-safe.
    /// </remarks>
	public interface IExtensionFactory
    {
		/// <summary>
		/// Creates one of each type of object that extends the input <paramref name="extensionPoint" />, 
		/// matching the input <paramref name="filter" />; creates a single extension if <paramref name="justOne"/> is true.
		/// </summary>
		/// <param name="extensionPoint">The <see cref="ExtensionPoint"/> to create extensions for.</param>
		/// <param name="filter">The filter used to match each extension that is discovered.</param>
		/// <param name="justOne">Indicates whether or not to return only the first matching extension that is found.</param>
		/// <returns></returns>
		object[] CreateExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter, bool justOne);

		/// <summary>
		/// Gets metadata describing all extensions of the input <paramref name="extensionPoint"/>, 
		/// matching the given <paramref name="filter"/>.
		/// </summary>
		/// <param name="extensionPoint">The <see cref="ExtensionPoint"/> whose extension metadata is to be retrieved.</param>
		/// <param name="filter">An <see cref="ExtensionFilter"/> used to filter out extensions with particular characteristics.</param>
		/// <returns></returns>
        ExtensionInfo[] ListExtensions(ExtensionPoint extensionPoint, ExtensionFilter filter);
    }
}
