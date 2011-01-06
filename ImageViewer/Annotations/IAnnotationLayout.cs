#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Defines an entire layout of <see cref="AnnotationBox"/>es to be rendered to
	/// the overlay by an <see cref="ClearCanvas.ImageViewer.Rendering.IRenderer"/>.
	/// </summary>
	/// <seealso cref="AnnotationBox"/>
	public interface IAnnotationLayout
	{
		/// <summary>
		/// Gets the <see cref="AnnotationBox"/>es that define the layout.
		/// </summary>
		IEnumerable<AnnotationBox> AnnotationBoxes { get; }

		/// <summary>
		/// Gets or sets whether the <see cref="IAnnotationLayout"/> is visible.
		/// </summary>
		bool Visible { get; set; }
	}
}
