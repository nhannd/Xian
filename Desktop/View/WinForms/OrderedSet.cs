using System.Collections.Generic;

namespace ClearCanvas.Desktop.View.WinForms
{
    /// <summary>
    /// Implements an ordered set, where the order of enumeration reflects the order in which items
    /// were added to the set.  If the same item is added to the set again, it is moved to the end of the ordering.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class OrderedSet<T> : IEnumerable<T>
    {
        private LinkedList<T> _items;

        public OrderedSet()
        {
            _items = new LinkedList<T>();
        }

        public void Add(T item)
        {
            _items.Remove(item);
            _items.AddLast(item);
        }

        public void Remove(T item)
        {
            _items.Remove(item);
        }

        public T FirstElement
        {
            get { return _items.Count > 0 ? _items.First.Value : default(T); }
        }

        public T LastElement
        {
            get { return _items.Count > 0 ? _items.Last.Value : default(T); }
        }

        public int Count
        {
            get { return _items.Count; }
        }

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        #endregion
    }
}
