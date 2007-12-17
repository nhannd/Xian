#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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
        public ReadOnlyQueryResultCollection(IList<QueryResult> queryResults) : base(queryResults)
        {
        }
    }

}
