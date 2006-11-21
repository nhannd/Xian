using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IList"/> as a type-safe one.
    /// </summary>
    /// <typeparam name="T">The type of the items in the list</typeparam>
    public class TypeSafeListWrapper<T> : IList<T>, IList
    {
        private IList _inner;

        public TypeSafeListWrapper(IList innerList)
        {
            _inner = innerList;
        }

        #region IList<T> Members

        public int IndexOf(T item)
        {
            return _inner.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _inner.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _inner.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return (T)_inner[index];
            }
            set
            {
                _inner[index] = value;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(T item)
        {
            _inner.Add(item);
        }

        public void Clear()
        {
            _inner.Clear();
        }

        public bool Contains(T item)
        {
            return _inner.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _inner.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _inner.Count; }
        }

        public bool IsReadOnly
        {
            get { return _inner.IsReadOnly; }
        }

        public bool Remove(T item)
        {
            if (_inner.Contains(item))
            {
                _inner.Remove(item);
                return !_inner.Contains(item);
            }
            return false;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new TypeSafeEnumeratorWrapper<T>(_inner.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        #endregion

        #region IList Members

        public int Add(object value)
        {
            return _inner.Add(value);
        }

        public bool Contains(object value)
        {
            return _inner.Contains(value);
        }

        public int IndexOf(object value)
        {
            return _inner.IndexOf(value);
        }

        public void Insert(int index, object value)
        {
            _inner.Insert(index, value);
        }

        public bool IsFixedSize
        {
            get { return _inner.IsFixedSize; }
        }

        public void Remove(object value)
        {
            _inner.Remove(value);
        }

        object IList.this[int index]
        {
            get
            {
                return _inner[index];
            }
            set
            {
                _inner[index] = value;
            }
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            _inner.CopyTo(array, index);
        }

        public bool IsSynchronized
        {
            get { return _inner.IsSynchronized; }
        }

        public object SyncRoot
        {
            get { return _inner.SyncRoot; }
        }

        #endregion
    }
}
