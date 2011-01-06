#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An <see cref="ExtensionPoint"/> for <see cref="IAnnotationItemProvider"/>s.
	/// </summary>
	[ExtensionPoint()]
	public sealed class AnnotationItemProviderExtensionPoint : ExtensionPoint<IAnnotationItemProvider>
	{
	}

	/// <summary>
	/// An <see cref="IAnnotationItemProvider"/> provides a logical grouping of 
	/// <see cref="IAnnotationItem"/>s simply because there can be so many of them.
	/// </summary>
	/// <seealso cref="AnnotationBox"/>
	/// <seealso cref="IAnnotationItem"/>
	/// <seealso cref="AnnotationItemConfigurationOptions"/>
	/// <seealso cref="IAnnotationLayout"/>
	/// <seealso cref="IAnnotationLayoutProvider"/>
	/// <seealso cref="AnnotationItemProviderExtensionPoint"/>
	public interface IAnnotationItemProvider
	{
		/// <summary>
		/// Gets a unique identifier.
		/// </summary>
		string GetIdentifier();

		/// <summary>
		/// Gets a user friendly display name.
		/// </summary>
		string GetDisplayName();

		/// <summary>
		/// Gets the logical group of <see cref="IAnnotationItem"/>s.
		/// </summary>
		IEnumerable<IAnnotationItem> GetAnnotationItems();
	}
}
