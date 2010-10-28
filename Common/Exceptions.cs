#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;

namespace ClearCanvas.Common
{
	/// <summary>
	/// Used by the framework to relay errors regarding plugins.
	/// </summary>
	/// <seealso cref="PluginManager"/>
    [SerializableAttribute]
	public class PluginException : ApplicationException
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(SerializationInfo info, StreamingContext context) : base(info, context) {}
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal PluginException(string message, Exception inner) : base(message, inner) { }
	}

	/// <summary>
	/// Used by the framework to relay errors regarding <see cref="IExtensionPoint"/>s.
	/// </summary>
	/// <seealso cref="PluginInfo"/>
    public class ExtensionPointException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionPointException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionPointException(string message, Exception inner) : base(message, inner) { }
    }

	/// <summary>
	/// Used by the framework to relay errors regarding extensions (created via <see cref="IExtensionPoint"/>s).
	/// </summary>
    public class ExtensionException : Exception
    {
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionException(string message) : base(message) { }
		/// <summary>
		/// Constructor.
		/// </summary>
		internal ExtensionException(string message, Exception inner) : base(message, inner) { }
    }
}
