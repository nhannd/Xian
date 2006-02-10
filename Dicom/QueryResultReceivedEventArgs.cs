
namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class QueryResultReceivedEventArgs : EventArgs
    {
        public QueryResultReceivedEventArgs(QueryResultDictionary result)
        {
            _result = result;
        }

        public QueryResultDictionary GetResult()
        {
            return _result;
        }

        private QueryResultDictionary _result;
    }
}
