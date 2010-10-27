#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Resolves the display name and label from an <see cref="IAnnotationItem"/>'s 
	/// unique identifier (<see cref="IAnnotationItem.GetIdentifier"/>, usually by looking the values
	/// up in assembly resources.
	/// </summary>
	public interface IAnnotationResourceResolver
	{
		/// <summary>
		/// Resolves the <see cref="IAnnotationItem"/>'s label (see <see cref="IAnnotationItem.GetLabel()"/>).
		/// </summary>
		string ResolveDisplayName(string annotationIdentifier);

		/// <summary>
		/// Resolves the <see cref="IAnnotationItem"/>'s (or <see cref="IAnnotationItemProvider"/>'s) display name 
		/// (see <see cref="IAnnotationItem.GetDisplayName"/> and <see cref="IAnnotationItemProvider.GetDisplayName"/>).
		/// </summary>
		string ResolveLabel(string annotationIdentifier);
	}
}
