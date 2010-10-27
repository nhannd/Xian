#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common
{
    /// <summary>
    /// Extension point interface.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This interface provides a means for a client of an extension point to reference
    /// the extension point and call methods on it without knowing the type of the extension point class.
	/// </para>
	/// <para>
    /// Extension point classes should never implement this interface directly.
    /// Instead, subclass <see cref="ExtensionPoint" />.
	/// </para>
    /// </remarks>
    public interface IExtensionPoint
    {
        /// <summary>
        /// Lists the available extensions.
        /// </summary>
        /// <returns>An array of <see cref="ExtensionInfo" /> objects describing the available extensions.</returns>
        ExtensionInfo[] ListExtensions();

        /// <summary>
        /// Lists the available extensions, that also match the specified <see cref="ExtensionFilter"/>.
        /// </summary>
        /// <returns>An array of <see cref="ExtensionInfo" /> objects describing the available extensions.</returns>
        ExtensionInfo[] ListExtensions(ExtensionFilter filter);

        /// <summary>
        /// Lists the available extensions that match the specified filter.
        /// </summary>
        ExtensionInfo[] ListExtensions(Predicate<ExtensionInfo> filter);
 
        /// <summary>
        /// Instantiates one extension.
        /// </summary>
        /// <remarks>
        /// If more than one extension exists, then the type of the extension that is
        /// returned is non-deterministic.  If no extensions exist that can be successfully
        /// instantiated, an exception is thrown.
        /// </remarks>
        /// <returns>A reference to the extension.</returns>
        /// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
        object CreateExtension();

        /// <summary>
        /// Instantiates an extension that also matches the specified <see cref="ExtensionFilter" />.
        /// </summary>
        /// <remarks>
        /// If more than one extension exists, then the type of the extension that is
        /// returned is non-deterministic.  If no extensions exist that can be successfully
        /// instantiated, an exception is thrown.
        /// </remarks>
        /// <returns>A reference to the extension.</returns>
        /// <exception cref="NotSupportedException">Failed to instantiate an extension.</exception>
        object CreateExtension(ExtensionFilter filter);

        /// <summary>
        /// Instantiates an extension that matches the specified filter.
        /// </summary>
        object CreateExtension(Predicate<ExtensionInfo> filter);
        
        /// <summary>
        /// Instantiates each available extension.
        /// </summary>
        /// <remarks>
        /// Attempts to instantiate each available extension.  If an extension fails to instantiate
        /// for any reason, the failure is logged and it is ignored.
        /// </remarks>
        /// <returns>An array of references to the created extensions.  If no extensions were created
        /// the array will be empty.</returns>
        object[] CreateExtensions();

        /// <summary>
        /// Instantiates each available extension that also matches the specified <see cref="ExtensionFilter" />.
        /// </summary>
        /// <remarks>
        /// Attempts to instantiate each matching extension.  If an extension fails to instantiate
        /// for any reason, the failure is logged and it is ignored.
        /// </remarks>
        /// <returns>An array of references to the created extensions.  If no extensions were created
        /// the array will be empty.</returns>
        object[] CreateExtensions(ExtensionFilter filter);

        /// <summary>
        /// Instantiates each available extension that matches the specified filter.
        /// </summary>
        object[] CreateExtensions(Predicate<ExtensionInfo> filter);
    }
}
