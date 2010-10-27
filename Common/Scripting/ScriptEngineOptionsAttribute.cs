#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Scripting
{
	/// <summary>
	/// When applied to an extension of <see cref="ScriptEngineExtensionPoint"/>, specifies
	/// how that extension will be handled by the scripting framework.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class ScriptEngineOptionsAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets a value indicating whether this engine should be limited to a single instance.
		/// </summary>
		public bool Singleton { get; set; }

		/// <summary>
		/// Gets or sets a value indicating whether this engine requires access from multiple threads to be synchronized.
		/// </summary>
		public bool SynchronizeAccess { get; set; }
	}
}
