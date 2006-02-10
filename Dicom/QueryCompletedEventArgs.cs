
namespace ClearCanvas.Dicom
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ClearCanvas.Dicom.Network;

    public class QueryCompletedEventArgs : EventArgs
    {
        public QueryCompletedEventArgs(ReadOnlyQueryResultCollection results)
        {
            _results = results;
        }

        public ReadOnlyQueryResultCollection GetResults()
        {
            return _results;
        }

        private ReadOnlyQueryResultCollection _results;
    }
}
