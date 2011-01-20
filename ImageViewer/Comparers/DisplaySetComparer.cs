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
	public interface IDisplaySetComparer : IComparer<IDisplaySet>, IEquatable<IDisplaySetComparer>
	{}

	/// <summary>
	/// Base class for comparers that compare some aspect of <see cref="IDisplaySet"/>s.
	/// </summary>
	public abstract class DisplaySetComparer : ComparerBase, IDisplaySetComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetComparer"/>.
		/// </summary>
		protected DisplaySetComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="DisplaySetComparer"/>.
		/// </summary>
		protected DisplaySetComparer(bool reverse)
			: base(reverse)
		{
		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="IDisplaySet"/>s.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
		public abstract int Compare(IDisplaySet x, IDisplaySet y);

		#endregion

		public bool Equals(IDisplaySetComparer other)
		{
			return other is DisplaySetComparer && Equals((DisplaySetComparer)other);
		}

		public virtual bool Equals(DisplaySetComparer other)
		{
			return base.Equals(other);
		}
	}
}
