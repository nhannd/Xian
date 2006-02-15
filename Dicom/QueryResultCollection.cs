namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    /// <summary>
    /// List of C-FIND query results. Each result represents the data
    /// from a particular C-FIND Response.
    /// </summary>
    public class QueryResultList : IList<QueryResultDictionary>
    {
        /// <summary>
        /// Add a new result item into the list.
        /// </summary>
        /// <param name="item">A result item.</param>
        public void Add(QueryResultDictionary item)
        {
            (this as ICollection<QueryResultDictionary>).Add(item);
        }

        /// <summary>
        /// Remove all results.
        /// </summary>
        public void Clear()
        {
            (this as ICollection<QueryResultDictionary>).Clear();
        }

        private List<QueryResultDictionary> _internalList = new List<QueryResultDictionary>();

        #region IList<QueryResult> Members

        public int IndexOf(QueryResultDictionary item)
        {
            return _internalList.IndexOf(item);
        }

        public void Insert(int index, QueryResultDictionary item)
        {
            _internalList.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            _internalList.RemoveAt(index);
        }

        public QueryResultDictionary this[int index]
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

        void ICollection<QueryResultDictionary>.Add(QueryResultDictionary item)
        {
            (_internalList as ICollection<QueryResultDictionary>).Add(item);
        }

        void ICollection<QueryResultDictionary>.Clear()
        {
            (_internalList as ICollection<QueryResultDictionary>).Clear();
        }

        bool ICollection<QueryResultDictionary>.Contains(QueryResultDictionary item)
        {
            return (_internalList as ICollection<QueryResultDictionary>).Contains(item);
        }

        void ICollection<QueryResultDictionary>.CopyTo(QueryResultDictionary[] array, int arrayIndex)
        {
            (_internalList as ICollection<QueryResultDictionary>).CopyTo(array, arrayIndex);
        }

        int ICollection<QueryResultDictionary>.Count
        {
            get { return (_internalList as ICollection<QueryResultDictionary>).Count; }
        }

        bool ICollection<QueryResultDictionary>.IsReadOnly
        {
            get { return (_internalList as ICollection<QueryResultDictionary>).IsReadOnly; }
        }

        bool ICollection<QueryResultDictionary>.Remove(QueryResultDictionary item)
        {
            return (_internalList as ICollection<QueryResultDictionary>).Remove(item);
        }

        #endregion

        #region IEnumerable<QueryResult> Members

        IEnumerator<QueryResultDictionary> IEnumerable<QueryResultDictionary>.GetEnumerator()
        {
            return _internalList.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_internalList as ICollection<QueryResultDictionary>).GetEnumerator();
        }

        #endregion

    }

    /// <summary>
    /// A read-only encapsulation of a QueryResultList.
    /// </summary>
    public class ReadOnlyQueryResultCollection : ReadOnlyCollection<QueryResultDictionary>
    {
        public ReadOnlyQueryResultCollection(QueryResultList queryResults) : base(queryResults)
        {
        }
    }

}
