namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Text;

    public class QueryResultList : IList<QueryResult>
    {

        #region IList<QueryResult> Members

        public int IndexOf(QueryResult item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void Insert(int index, QueryResult item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public void RemoveAt(int index)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public QueryResult this[int index]
        {
            get
            {
                throw new Exception("The method or operation is not implemented.");
            }
            set
            {
                throw new Exception("The method or operation is not implemented.");
            }
        }

        #endregion
 
        #region ICollection<QueryResult> Members

        void ICollection<QueryResult>.Add(QueryResult item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ICollection<QueryResult>.Clear()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        bool ICollection<QueryResult>.Contains(QueryResult item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        void ICollection<QueryResult>.CopyTo(QueryResult[] array, int arrayIndex)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        int ICollection<QueryResult>.Count
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool ICollection<QueryResult>.IsReadOnly
        {
            get { throw new Exception("The method or operation is not implemented."); }
        }

        bool ICollection<QueryResult>.Remove(QueryResult item)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable<QueryResult> Members

        IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

    }

    public class ReadOnlyQueryResultCollection : ReadOnlyCollection<QueryResult>
    {
        public ReadOnlyQueryResultCollection(QueryResultList queryResults) : base(queryResults)
        {
        }
    }

}
