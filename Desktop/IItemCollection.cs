using System;
using System.Collections;
using System.Collections.Generic;

namespace ClearCanvas.Desktop
{
    /// <summary>
    /// Enumerates the types of item changes
    /// </summary>
    public enum ItemChangeType
    {
        /// <summary>
        /// An item was added to the table
        /// </summary>
        ItemAdded,

        /// <summary>
        /// An item in the table was changed
        /// </summary>
        ItemChanged,

        /// <summary>
        /// An item was removed from the table
        /// </summary>
        ItemRemoved,

        /// <summary>
        /// All items in the table may have changed
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
        /// Gets the type of change that occured
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
    public interface IItemCollection : IEnumerable, IList
    {
        /// <summary>
        /// Occurs when an item in the collection has changed.
        /// </summary>
        event EventHandler<ItemChangedEventArgs> ItemsChanged;

        /// <summary>
        /// Notifies the table that the item at the specified index has changed in some way.  Use this method
        /// to cause the view to update itself to reflect the changed item.
        /// </summary>
        /// <param name="index"></param>
        void NotifyItemUpdated(int index);

        /// <summary>
        /// Adds all items in the specified enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        void AddRange(IEnumerable enumerable);
    }

    /// <summary>
    /// Defines the interface to the collection of items
    /// </summary>
    /// <typeparam name="TItem">Item type</typeparam>
    public interface IItemCollection<TItem> : IItemCollection, IEnumerable<TItem>, IList<TItem>
    {
        /// <summary>
        /// Notifies the table that the specified item has changed in some way.  Use this method
        /// to cause the view to update itself to reflect the changed item.
        /// </summary>
        /// <param name="item"></param>
        void NotifyItemUpdated(TItem item);

        /// <summary>
        /// Adds all items in the specified enumerable
        /// </summary>
        /// <param name="enumerable"></param>
        void AddRange(IEnumerable<TItem> enumerable);

        /// <summary>
        /// Sorts items in the collection using the specified <see cref="IComparer{TItem}"/>
        /// </summary>
        /// <param name="comparer"></param>
        void Sort(IComparer<TItem> comparer);

        /// <summary>
        /// Sets any items in the collection matching the specified constraint to the specified new value. 
        /// </summary>
        /// <param name="constraint"></param>
        /// <param name="newValue"></param>
        void Replace(Predicate<TItem> constraint, TItem newValue);

        /// <summary>
        /// Searches the collection for an item that satisfies the specified constraint and returns
        /// the index of the first such item.
        /// </summary>
        /// <returns>The index of the first matching item, or -1 if no matching items are found</returns>
        int FindIndex(Predicate<TItem> constraint);
    }
}
