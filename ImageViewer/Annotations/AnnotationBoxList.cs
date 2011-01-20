#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Annotations
{
	/// <summary>
	/// An observable container for <see cref="AnnotationBox"/>es.
	/// </summary>
	[Cloneable(true)]
	public class AnnotationBoxList : ObservableList<AnnotationBox>
	{
		internal AnnotationBoxList()
		{
		}

		[CloneInitialize]
		private void Initialize(AnnotationBoxList source, ICloningContext context)
		{
			foreach (AnnotationBox box in source)
				this.Add(box.Clone());
		}

		/// <summary>
		/// Creates a deep clone of this <see cref="AnnotationBoxList"/>.
		/// </summary>
		/// <returns>A deep clone of this <see cref="AnnotationBoxList"/>.</returns>
		public AnnotationBoxList Clone()
		{
			AnnotationBoxList clone = new AnnotationBoxList();
			clone.Initialize(this, null);
			return clone;
		}
	}
}
