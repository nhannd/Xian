#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using System.Collections.Generic;
using ClearCanvas.ImageViewer.Comparers;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer
{
	/// <summary>
	/// A collection of <see cref="IPresentationImage"/> objects.
	/// </summary>
	public class PresentationImageCollection : ObservableList<IPresentationImage>
	{
		/// <summary>
		/// Instantiates a new instance of <see cref="PresentationImageCollection"/>.
		/// </summary>
		internal PresentationImageCollection()
		{
		}

		internal static IComparer<IPresentationImage> GetDefaultComparer()
		{
			return new InstanceAndFrameNumberComparer();
		}

		/// <summary>
		/// The comparer that was last used to sort the collection, via <see cref="Sort"/>.
		/// </summary>
		/// <remarks>
		/// When an item is added to or replaced, this value is set to null.  When an item is
		/// simply removed, the sort order is maintained, so this value also will not change.
		/// </remarks>
		public IComparer<IPresentationImage> SortComparer { get; internal set; }

		/// <summary>
		/// Sorts the collection using <see cref="SortComparer"/>.
		/// </summary>
		/// <remarks>
		/// If <see cref="SortComparer"/> is null, it is first set to a default one.
		/// </remarks>
		public void Sort()
		{
			if (SortComparer == null)
				SortComparer = GetDefaultComparer();
			Sort(SortComparer);
		}

		/// <summary>
		/// Sorts the collection with the given comparer.
		/// </summary>
		public sealed override void Sort(IComparer<IPresentationImage> comparer)
		{
			Platform.CheckForNullReference(comparer, "comparer");
			SortComparer = comparer;
			base.Sort(SortComparer);
		}

		protected override void OnItemAdded(ListEventArgs<IPresentationImage> e)
		{
			SortComparer = null; //we don't know the sort order anymore.
			base.OnItemAdded(e);
		}

		protected override void  OnItemChanged(ListEventArgs<IPresentationImage> e)
		{
			SortComparer = null;//we don't know the sort order anymore.
 			 base.OnItemChanged(e);
		}
	}
}
