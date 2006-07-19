using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public class SearchResultPage
    {
        private int _firstRow;
        private int _maxRows;

        public SearchResultPage()
        {
            _firstRow = 0;  // default
            _maxRows = 100; // default
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
        /// The maximum number of rows to return.
        /// </summary>
        public int MaxRows
        {
            get { return _maxRows; }
            set { _maxRows = value; }
        }
    }
}
