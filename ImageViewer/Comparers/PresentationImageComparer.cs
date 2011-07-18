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
	public interface IPresentationImageComparer : IComparer<IPresentationImage>, IEquatable<IPresentationImageComparer>
	{}

	/// <summary>
	/// Base class for comparing <see cref="IPresentationImage"/>s.
	/// </summary>
	public abstract class PresentationImageComparer : ComparerBase, IPresentationImageComparer, IEquatable<PresentationImageComparer>
	{
		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageComparer"/>.
		/// </summary>
		protected PresentationImageComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="PresentationImageComparer"/>.
		/// </summary>
		protected PresentationImageComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IPresentationImage> Members

		/// <summary>
		/// Compares two <see cref="IPresentationImage"/>s.
		/// </summary>
		public abstract int Compare(IPresentationImage x, IPresentationImage y);

		#endregion

		public bool Equals(IPresentationImageComparer other)
		{
			return other is PresentationImageComparer && Equals((PresentationImageComparer) other);
		}
		
		public virtual bool Equals(PresentationImageComparer other)
		{
			return base.Equals(other);
		}
	}
}
