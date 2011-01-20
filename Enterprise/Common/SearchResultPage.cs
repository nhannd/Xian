#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Provides a mechanism for requesting a "page" of search results from a persistent store.
    /// </summary>
    [DataContract]
    public class SearchResultPage
    {
        private int _firstRow;
        private int _maxRows;

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchResultPage()
        {
            _firstRow = -1;  // ignore
            _maxRows = -1; // ignore
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="firstRow">The first row to return</param>
        /// <param name="maxRows">The maximum number of rows to return</param>
        public SearchResultPage(int firstRow, int maxRows)
        {
            _firstRow = firstRow;
            _maxRows = maxRows;
        }

        /// <summary>
        /// The first row to return.
        /// </summary>
        [DataMember]
        public int FirstRow
        {
            get { return _firstRow; }
            set { _firstRow = value; }
        }

        /// <summary>
        /// The maximum number of rows to return.  A value of -1 can be used to indicate that all rows should
        /// be returned.  This feature should be used with caution however.
        /// </summary>
        [DataMember]
        public int MaxRows
        {
            get { return _maxRows; }
            set { _maxRows = value; }
        }
    }
}
