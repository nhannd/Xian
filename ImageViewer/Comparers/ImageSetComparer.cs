#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Comparers
{
	public interface IImageSetComparer : IComparer<IImageSet>, IEquatable<IImageSetComparer>
	{ }

	/// <summary>
	/// Base class for comparers that compare some aspect of <see cref="IImageSet"/>s.
	/// </summary>
	public abstract class ImageSetComparer : ComparerBase, IImageSetComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ImageSetComparer"/>.
		/// </summary>
		protected ImageSetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IImageSet> Members

		/// <summary>
		/// Compares two <see cref="IImageSet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IImageSet x, IImageSet y);

		#endregion

		public bool Equals(IImageSetComparer other)
		{
			return other is ImageSetComparer && Equals((ImageSetComparer)other);
		}

		public virtual bool Equals(ImageSetComparer other)
		{
			return base.Equals(other);
		}
	}
}
