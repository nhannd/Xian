using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    public class FindScpEventArgs : EventArgs
    {
        private QueryKey _queryKey;
        private ReadOnlyQueryResultCollection _queryResults;

        public FindScpEventArgs(QueryKey queryKey)
        {
            _queryKey = queryKey;
            _queryResults = null;
        }

        public QueryKey QueryKey
        {
            get { return _queryKey; }
            set { _queryKey = value; }
        }

        public ReadOnlyQueryResultCollection QueryResults
        {
            get { return _queryResults; }
            set { _queryResults = value; }
        }
    }
}
