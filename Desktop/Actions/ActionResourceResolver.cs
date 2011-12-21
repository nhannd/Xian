#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Reflection;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Actions
{
	/// <summary>
	/// A specialization of the <see cref="ResourceResolver"/> class for use in resolving resources
	/// related to actions.
	/// </summary>
	public class ActionResourceResolver : ResourceResolver
	{
		/// <summary>
		/// Constructs an instance of this object for the specified action target.
		/// </summary>
		/// <remarks>
		/// The class of the target object determines the primary assembly that will be used to resolve resources.
		/// </remarks>
		/// <param name="actionTarget">The action target for which resources will be resolved.</param>
		public ActionResourceResolver(object actionTarget)
			: base(actionTarget.GetType(), true) {}

		/// <summary>
		/// Constructs an instance of this object for the specified action target.
		/// </summary>
		/// <remarks>
		/// The class of the target object determines the primary assembly that will be used to resolve resources.
		/// </remarks>
		/// <param name="targetType">The action target type for which resources will be resolved.</param>
		public ActionResourceResolver(Type targetType)
			: base(targetType, true) {}

		protected override Stream OpenResource(string resourceFullName, Assembly assembly)
		{
			var resource = ApplicationThemeManager.OpenResource(resourceFullName, assembly);
			return resource ?? base.OpenResource(resourceFullName, assembly);
		}
	}
}