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
	/// <summary>
	/// Base class for comparers that are used for sorting collections.
	/// </summary>
	public abstract class ComparerBase : IEquatable<ComparerBase>
	{
		private int _returnValue;

		/// <summary>
		/// Initializes a new instance of <see cref="ComparerBase"/>.
		/// </summary>
		protected ComparerBase()
		{
			Reverse = false;
		}

		/// <summary>
		/// Initializes a new instance of <see cref="ComparerBase"/>.
		/// </summary>
		protected ComparerBase(bool reverse)
		{
			this.Reverse = reverse;
		}

		/// <summary>
		/// Gets or sets a value indicating whether or not the collection will be
		/// sorted in ascending or descending order.
		/// </summary>
		/// <remarks>
		/// The default is false, or descending.
		/// </remarks>
		protected bool Reverse
		{
			get
			{ 
				return ( this.ReturnValue == 1); 
			}
			set
			{
				if (!value)
					this.ReturnValue = -1;
				else
					this.ReturnValue = 1;
			}
		}

		/// <summary>
		/// Gets a value indicating whether or not the collection will be
		/// sorted in ascending or descending order.
		/// </summary>
		/// <remarks>
		/// Inheritors should return this value appropriately in order for
		/// the reversible sorting mechanism to work properly.
		/// </remarks>
		/// <value>1 if <see cref="Reverse"/> is <b>true</b></value>
		/// <value>-1 if <see cref="Reverse"/> is <b>false</b></value>
		protected int ReturnValue
		{
			get { return _returnValue; }
			private set { _returnValue = value; }
		}

		/// <summary>
		/// Compares two sets of values, where the position of the items
		/// in <paramref name="x"/> corresponds with the position of the items
		/// in <paramref name="y"/>.
		/// </summary>
		/// <remarks>
		/// Item pairs are compared in order until the first non-equal pair
		/// is found, at which point comparison stops and a result is returned.
		/// The number of items in <paramref name="x"/> and <paramref name="y"/> <b>must</b>
		/// always yield the same number of entries, or an <see cref="ArgumentException"/> may be thrown.
		/// </remarks>
		/// <returns>Zero when <b>all</b> entry pairs are equal, otherwise +-<see cref="ReturnValue"/>.</returns>
		protected int Compare(IEnumerable<IComparable> x, IEnumerable<IComparable> y)
		{
			IEnumerator<IComparable> enumeratorX = x.GetEnumerator();
			IEnumerator<IComparable> enumeratorY = y.GetEnumerator();

			while (true)
			{
				bool xExists = enumeratorX.MoveNext();
				bool yExists = enumeratorY.MoveNext();

				if (!xExists && !yExists)
					break;
				else if (xExists != yExists)
					throw new ArgumentException("The input arguments must have the same number of entries.");

				IComparable xValue = enumeratorX.Current;
				IComparable yValue = enumeratorY.Current;

				if (!ReferenceEquals(xValue, yValue))
				{
					if (xValue == null)
						return ReturnValue;
					if (yValue == null)
						return -ReturnValue;

					int compare = xValue.CompareTo(yValue);
					if (compare < 0)
						return ReturnValue;
					else if (compare > 0)
						return -ReturnValue;
				}
			}

			return 0;
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
		
		public sealed override bool Equals(object obj)
		{
			if (obj is ComparerBase)
				return Equals((ComparerBase) obj);

			return base.Equals(obj);
		}

		#region IEquatable<ComparerBase> Members

		public virtual bool Equals(ComparerBase other)
		{
			return other != null && GetType().Equals(other.GetType()) && Reverse == other.Reverse;
		}

		#endregion
	}
}