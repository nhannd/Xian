using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Desktop.Trees
{
    /// <summary>
    /// Generic implemenation of <see cref="ITreeItemCollection"/> used by the <see cref="Tree"/> class.
    /// </summary>
    /// <typeparam name="TItem"></typeparam>
    public class TreeItemCollection<TItem> : IList<TItem>, ITreeItemCollection
    {
        private List<TItem> _items;
        private Tree<TItem> _owner;

        internal TreeItemCollection(Tree<TItem> owner, IEnumerable elements)
        {
            _owner = owner;
            _items = new List<TItem>();
            if (elements != null)
            {
                _items.AddRange(new TypeSafeEnumerableWrapper<TItem>(elements));
            }
        }

        internal TreeItemCollection(Tree<TItem> owner)
            :this(owner, null)
        {
        }

        #region IList<TItem> Members

        public int IndexOf(TItem item)
        {
            return _items.IndexOf(item);
        }

        public void Insert(int index, TItem item)
        {
            _items.Insert(index, item);
            _owner.NotifyItemsChanged(TreeItemChangeType.ItemAdded, index);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
            _owner.NotifyItemsChanged(TreeItemChangeType.ItemRemoved, index);
        }

        public TItem this[int index]
        {
            get
            {
                return _items[index];
            }
            set
            {
                _items[index] = value;
                _owner.NotifyItemsChanged(TreeItemChangeType.ItemChanged, index);
            }
        }

        #endregion

        #region ICollection<TItem> Members

        public void Add(TItem item)
        {
            _items.Add(item);
            _owner.NotifyItemsChanged(TreeItemChangeType.ItemAdded, this.Count - 1);
        }

        public void Clear()
        {
            _items.Clear();
            _owner.NotifyItemsChanged(TreeItemChangeType.Reset, -1);
        }

        public bool Contains(TItem item)
        {
            return _items.Contains(item);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _items.Count; }
        }

        public bool Remove(TItem item)
        {
            int index = IndexOf(item);
            bool removed = _items.Remove(item);
            if (removed)
            {
                _owner.NotifyItemsChanged(TreeItemChangeType.ItemRemoved, index);
            }
            return removed;
        }

        bool ICollection<TItem>.IsReadOnly
        {
            get { return (_items as ICollection<TItem>).IsReadOnly; }
        }

        #endregion

        #region IEnumerable<TItem> Members

        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        object ITreeItemCollection.this[int index]
        {
            get
            {
                return _items[index];
            }
        }
    }
}
