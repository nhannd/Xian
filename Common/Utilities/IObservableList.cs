#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Interface to an observable list.
	/// </summary>
	public interface IObservableList<TItem> : IList<TItem>
	{
		/// <summary>
		/// Fired when an item is added to the list.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemAdded;

		/// <summary>
		/// Fired when an item is removed from the list.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemRemoved;

		/// <summary>
		/// Fired when an item in the list has changed.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemChanged;

		/// <summary>
		/// Fires when an item in the list is about to change.
		/// </summary>
		event EventHandler<ListEventArgs<TItem>> ItemChanging;
	}
}