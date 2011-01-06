#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Provides access to an <see cref="IAnnotationLayout"/>.
	/// </summary>
	/// <seealso cref="IAnnotationLayout"/>
	public interface IAnnotationLayoutProvider
	{
		/// <summary>
		/// Gets or sets the <see cref="IAnnotationLayout"/>.
		/// </summary>
		/// <remarks>
		/// Like other provider interfaces, it is not recommended that this property return null.
		/// </remarks>
		IAnnotationLayout AnnotationLayout { get; set; }
	}
}
