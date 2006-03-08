namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// List of C-FIND query results. Each result represents the data
    /// from a particular C-FIND Response.
    /// </summary>
    public class QueryResultList : IList<QueryResult>
    {
        /// <summary>
        /// Add a new result item into the list.
        /// </summary>
        /// <param name="item">A result item.</param>
        public void Add(QueryResult item)
        {
            (this as ICollection<QueryResult>).Add(item);
        }

        /// <summary>
        /// Remove all results.
        /// </summary>
        public void Clear()
        {
            (this as ICollection<QueryResult>).Clear();
        }

        private List<QueryResult> _internalList = new List<QueryResult>();

        #region IList<QueryResult> Members

        public int IndexOf(QueryResult item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, QueryResult item)
        {
            _internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _internalList.RemoveAt(index);
        }

        public QueryResult this[int index]
        {
            get
            {
                return _internalList[index];
            }
            set
            {
                _internalList[index] = value;
            }
        }

        #endregion
 
        #region ICollection<QueryResult> Members

        void ICollection<QueryResult>.Add(QueryResult item)
        {
            (_internalList as ICollection<QueryResult>).Add(item);
        }

        void ICollection<QueryResult>.Clear()
        {
            (_internalList as ICollection<QueryResult>).Clear();
        }

        bool ICollection<QueryResult>.Contains(QueryResult item)
        {
            return (_internalList as ICollection<QueryResult>).Contains(item);
        }

        void ICollection<QueryResult>.CopyTo(QueryResult[] array, int arrayIndex)
        {
            (_internalList as ICollection<QueryResult>).CopyTo(array, arrayIndex);
        }

        int ICollection<QueryResult>.Count
        {
            get { return (_internalList as ICollection<QueryResult>).Count; }
        }

        bool ICollection<QueryResult>.IsReadOnly
        {
            get { return (_internalList as ICollection<QueryResult>).IsReadOnly; }
        }

        bool ICollection<QueryResult>.Remove(QueryResult item)
        {
            return (_internalList as ICollection<QueryResult>).Remove(item);
        }

        #endregion

        #region IEnumerable<QueryResult> Members

        IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_internalList as ICollection<QueryResult>).GetEnumerator();
        }

        #endregion

    }

    /// <summary>
    /// A read-only encapsulation of a QueryResultList.
    /// </summary>
    public class ReadOnlyQueryResultCollection : ReadOnlyCollection<QueryResult>
    {
        public ReadOnlyQueryResultCollection(QueryResultList queryResults) : base(queryResults)
        {
        }
    }

}
