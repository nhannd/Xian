using System;
using System.Collections.Generic;
using System.Text;

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
    /// Event args used when an item changes
    /// </summary>
    public class ItemEventArgs : EventArgs
    {
        private object _item;
        private int _itemIndex;
        private ItemChangeType _changeType;

        internal ItemEventArgs(ItemChangeType changeType, int itemIndex, object item)
        {
            _changeType = changeType;
            _itemIndex = itemIndex;
            _item = item;
        }

        /// <summary>
        /// The type of change that occured
        /// </summary>
        public ItemChangeType ChangeType
        {
            get { return _changeType; }
        }

        /// <summary>
        /// The index of the item that changed
        /// </summary>
        public int ItemIndex
        {
            get { return _itemIndex; }
        }

        public object Item
        {
            get { return _item; }
        }
    }

    /// <summary>
    /// Defines the interface to the collection of items.
    /// </summary>
    public interface IItemCollection : System.Collections.IEnumerable
    {
        event EventHandler<ItemEventArgs> ItemsChanged;

        /// <summary>
        /// Gets the number of items in the collection
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Gets the item at the specified index
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        object this[int index] { get; }
    }
}
