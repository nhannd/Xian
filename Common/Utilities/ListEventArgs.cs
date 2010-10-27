#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Event used to notify observers of a change in a collection.
	/// </summary>
	/// <remarks>
	/// This class is used internally by the <see cref="ObservableList{TItem}"/>, but can be used
	/// for any collection-related event.
	/// </remarks>
	/// <typeparam name="TItem">The type of item in the collection.</typeparam>
	public class ListEventArgs<TItem> : EventArgs
	{
		private TItem _item;
		private int _index;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="item">The item that has changed.</param>
		/// <param name="index">The index of the item that has changed.</param>
		public ListEventArgs(TItem item, int index)
		{
			_item = item;
			_index = index;
		}

		/// <summary>
		/// Gets the item that has somehow changed in the related collection.
		/// </summary>
		public TItem Item
		{
			get { return _item; }
		}

		/// <summary>
		/// Gets the index of the item that has somehow changed in the related collection.
		/// </summary>
		public int Index
		{
			get { return _index; }
		}
	}
}
