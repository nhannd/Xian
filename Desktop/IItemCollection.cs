#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Enumerates the types of item changes.
    /// </summary>
    public enum ItemChangeType
    {
        /// <summary>
        /// An item was added to the table.
        /// </summary>
        ItemAdded,

		/// <summary>
		/// An item was inserted to the table.
		/// </summary>
		ItemInserted,
		
        /// <summary>
        /// An item in the table was changed.
        /// </summary>
        ItemChanged,

        /// <summary>
        /// An item was removed from the table.
        /// </summary>
        ItemRemoved,

        /// <summary>
        /// All items in the table may have changed.
        /// </summary>
        Reset
    }

    /// <summary>
    /// Provides data for the <see cref="IItemCollection.ItemsChanged"/> event.
    /// </summary>
    public class ItemChangedEventArgs : EventArgs
    {
        private readonly object _item;
        private readonly int _itemIndex;
        private readonly ItemChangeType _changeType;

        internal ItemChangedEventArgs(ItemChangeType changeType, int itemIndex, object item)
        {
            _changeType = changeType;
            _itemIndex = itemIndex;
            _item = item;
        }

        /// <summary>
        /// Gets the type of change that occured.
        /// </summary>
        public ItemChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// Gets the index of the item that changed.
        /// </summary>
        public int ItemIndex
        {
            get { return _itemIndex; }
        }

        /// <summary>
        /// Gets the item that has changed.
        /// </summary>
        public object Item
        {
            get { return _item; }
        }
    }


	/// <summary>
    /// Defines the interface to the collection of items.
    /// </summary>
    /// <remarks>
    /// Do not implement this interface.  It is 
    /// not considered part of the public API and is subject to change.
    /// </remarks>
    public interface IItemCollection : IList
    {
		/// <summary>
		/// Occurs when a transaction has started.
		/// </summary>
		event EventHandler TransactionStarted;
		
		/// <summary>
        /// Occurs when an item in the collection has changed.
        /// </summary>
        event EventHandler<ItemChangedEventArgs> ItemsChanged;

		/// <summary>
		/// Occurs when a transaction has completed.
		/// </summary>
		event EventHandler TransactionCompleted;
    }

    /// <summary>
    /// Defines the interface to the collection of items.
    /// </summary>
	/// <remarks>
	/// Do not implement this interface.  It is likely to be removed
	/// in subsequent versions of the framework and is 
	/// not considered part of the public API.
	/// </remarks>
	/// <typeparam name="TItem">The item type.</typeparam>
    public interface IItemCollection<TItem> : IItemCollection, IList<TItem>
    {
    }
}
