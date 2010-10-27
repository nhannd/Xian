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
	/// Provides text to be rendered to the overlay by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/>.
	/// </summary>
	/// <seealso cref="AnnotationBox"/>
	/// <seealso cref="AnnotationItemConfigurationOptions"/>
	/// <seealso cref="IAnnotationItemProvider"/>
	/// <seealso cref="IAnnotationLayout"/>
	/// <seealso cref="IAnnotationLayoutProvider"/>
	public interface IAnnotationItem
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
		/// Gets the label that can be shown on the overlay depending on the <see cref="AnnotationBox"/>'s 
		/// configuration (<see cref="AnnotationItemConfigurationOptions"/>).
		/// </summary>
		string GetLabel();

		/// <summary>
		/// Gets the annotation text to display on the overlay for <paramref name="presentationImage"/>.
		/// </summary>
		string GetAnnotationText(IPresentationImage presentationImage);
	}
}
