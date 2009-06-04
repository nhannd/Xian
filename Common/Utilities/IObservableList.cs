using System;
using System.Collections.Generic;

namespace ClearCanvas.Common.Utilities
{
	/// <summary>
	/// Interface for an observable list.
	/// </summary>
	/// <typeparam name="TItem"></typeparam>
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