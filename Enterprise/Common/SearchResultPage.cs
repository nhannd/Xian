using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Common
{
    /// <summary>
    /// Provides a mechanism for requesting a "page" of search results from a persistent store.
    /// </summary>
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
        public int FirstRow
        {
            get { return _firstRow; }
            set { _firstRow = value; }
        }

        /// <summary>
        /// The maximum number of rows to return.  A value of -1 can be used to indicate that all rows should
        /// be returned.  This feature should be used with caution however.
        /// </summary>
        public int MaxRows
        {
            get { return _maxRows; }
            set { _maxRows = value; }
        }
    }
}
