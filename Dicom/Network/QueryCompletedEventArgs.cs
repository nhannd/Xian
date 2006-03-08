
namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Argument type that encapsulates data pertinent to a
    /// query having completed.
    /// </summary>
    public class QueryCompletedEventArgs : EventArgs
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="results">The object containing the results from the query.</param>
        public QueryCompletedEventArgs(ReadOnlyQueryResultCollection results)
        {
            _results = results;
        }

        /// <summary>
        /// Retrieves the results from the query.
        /// </summary>
        /// <returns>A read-only collection containing every result from the C-FIND query.</returns>
        public ReadOnlyQueryResultCollection Results
        {
            get { return _results; }
        }

        private ReadOnlyQueryResultCollection _results;
    }
}
