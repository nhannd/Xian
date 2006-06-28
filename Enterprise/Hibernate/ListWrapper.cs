using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Hibernate
{
    /// <summary>
    /// Utility class used to wrap an untyped <see cref="IList"/> as a type-safe <see cref="IList<T>"/>.
    /// This is an interim measure because NHibernate does not yet support generics.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ListWrapper<T> : IList<T>
    {
        private IList _inner;

        internal ListWrapper(IList innerList)
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
            // implement this if necessary
            throw new Exception("The method or operation is not implemented.");
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
            return new EnumeratorWrapper<T>(_inner.GetEnumerator());
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _inner.GetEnumerator();
        }

        #endregion
    }
}
