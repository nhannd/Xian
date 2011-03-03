#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// Base class for <see cref="IAnnotationLayout"/>s.
	/// </summary>
	[Cloneable(true)]
	public class AnnotationLayout : IAnnotationLayout
	{
		private readonly AnnotationBoxList _annotationBoxes;

		/// <summary>
		/// Constructor.
		/// </summary>
		public AnnotationLayout()
		{
			_annotationBoxes = new AnnotationBoxList();
			Visible = true;
		}

		/// <summary>
		/// Gets the <see cref="AnnotationBox"/>es that define the layout.
		/// </summary>
		public AnnotationBoxList AnnotationBoxes
		{
			get { return _annotationBoxes; }
		}

		#region IAnnotationLayout Members

		/// <summary>
		/// Gets the <see cref="AnnotationBox"/>es that define the layout.
		/// </summary>
		IEnumerable<AnnotationBox> IAnnotationLayout.AnnotationBoxes
		{
			get { return _annotationBoxes; }
		}

		/// <summary>
		/// Gets or sets whether the <see cref="IAnnotationLayout"/> is visible.
		/// </summary>
		public bool Visible { get; set; }

		IAnnotationLayout IAnnotationLayout.Clone()
		{
			return Clone();
		}

		/// <summary>
		/// Creates a deep clone of this <see cref="AnnotationLayout"/>.
		/// </summary>
		public AnnotationLayout Clone()
		{
			return CloneBuilder.Clone(this) as AnnotationLayout;
		}
		
		#endregion
	}
}
