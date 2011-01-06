#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System;
using ClearCanvas.ImageViewer;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// <see cref="DisplaySetComparer"/> for sorting on <see cref="IDisplaySet.Number"/>.
	/// </summary>
	public class DisplaySetNumberComparer : DisplaySetComparer
	{
		/// <summary>
		/// Defalt constructor.
		/// </summary>
		public DisplaySetNumberComparer()
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public DisplaySetNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		/// <summary>
		/// Gets the values to be compared for a given <see cref="IDisplaySet"/>.
		/// </summary>
		/// <remarks>
		/// Returns <see cref="IDisplaySet.Number"/>, then <see cref="IDisplaySet.Name"/>.
		/// </remarks>
		protected IEnumerable<IComparable> GetCompareValues(DisplaySet displaySet)
		{
			yield return displaySet.Number;
			yield return displaySet.Name;
		}

		/// <summary>
		/// Compares two <see cref="IDisplaySet"/>s.
		/// </summary>
		public override int Compare(IDisplaySet x, IDisplaySet y)
		{
			DisplaySet displaySet1 = x as DisplaySet;
			DisplaySet displaySet2 = y as DisplaySet;
			
			if (ReferenceEquals(displaySet1, displaySet2))
				return 0; //same object or both are null

			//at this point, at least one of x or y is non-null and they are not the same object

			if (displaySet1 == null)
				return -ReturnValue; // x > y (because we want x at the end for non-reverse sorting)
			if (displaySet2 == null)
				return ReturnValue; // x < y (because we want y at the end for non-reverse sorting)

			return base.Compare(GetCompareValues(displaySet1), GetCompareValues(displaySet2));
		}
	}
}
