using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Common.Utilities
{
    /// <summary>
    /// Utility implementation of <see cref="IFunctionalList"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FunctionalList<T> : IFunctionalList<T>
    {
        private IList<T> _inner;

        /// <summary>
        /// Constructs an empty list
        /// </summary>
        public FunctionalList()
        {
            _inner = new List<T>();
        }

        /// <summary>
        /// Constructs a list that wraps the specified inner list.  This constructor does not copy
        /// the argument, so all methods called on this list operate on the inner list.
        /// </summary>
        /// <param name="inner"></param>
        public FunctionalList(IList<T> inner)
        {
            _inner = inner;
        }

        /// <summary>
        /// Constructs a list that wraps the specified inner list.  This constructor does not copy
        /// the argument, so all methods called on this list operate on the inner list.
        /// </summary>
        /// <param name="inner"></param>
        public FunctionalList(System.Collections.IList inner)
        {
            _inner = new TypeSafeListWrapper<T>(inner);
        }

        /// <summary>
        /// Constructs a list that initially contains the specified source items.  The source items
        /// are copied to this list.
        /// </summary>
        /// <param name="source"></param>
        public FunctionalList(IEnumerable<T> source)
        {
            _inner = new List<T>(source);
        }

        /// <summary>
        /// Constructs a list that initially contains the specified source items.  The source items
        /// are copied to this list.
        /// </summary>
        /// <param name="source"></param>
        public FunctionalList(System.Collections.IEnumerable source)
        {
            _inner = new List<T>(new TypeSafeEnumerableWrapper<T>(source));
        }

        #region IFunctionalList<T> members

        public IFunctionalList<T> Select(Predicate<T> predicate)
        {
            return CollectionUtils.Select<T, FunctionalList<T>>(_inner, predicate);
        }

        public IFunctionalList<T> Reject(Predicate<T> predicate)
        {
            return CollectionUtils.Reject<T, FunctionalList<T>>(_inner, predicate);
        }

        public T SelectFirst(Predicate<T> predicate)
        {
            return CollectionUtils.SelectFirst<T>(_inner, predicate);
        }

        public bool Contains(Predicate<T> predicate)
        {
            return CollectionUtils.Contains<T>(_inner, predicate);
        }

        public bool TrueForAll(Predicate<T> predicate)
        {
            return CollectionUtils.TrueForAll<T>(_inner, predicate);
        }

        public IFunctionalList<TResult> Map<TResult>(Converter<T, TResult> mapFunction)
        {
            return CollectionUtils.Map<T, TResult, FunctionalList<TResult>>(_inner, mapFunction);
        }

        public void ForEach(Action<T> doFunction)
        {
            CollectionUtils.ForEach<T>(_inner, doFunction);
        }

        #endregion

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
            get { return _inner[index]; }
            set { _inner[index] = value; }
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
            return _inner.Remove(item);
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return _inner.GetEnumerator();
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
