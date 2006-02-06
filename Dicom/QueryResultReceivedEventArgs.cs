
namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class QueryResultReceivedEventArgs : EventArgs
    {
        public QueryResultReceivedEventArgs(QueryResult result)
        {
            _result = result;
        }

        public QueryResult GetResult()
        {
            return _result;
        }

        private QueryResult _result = null;
    }
}
