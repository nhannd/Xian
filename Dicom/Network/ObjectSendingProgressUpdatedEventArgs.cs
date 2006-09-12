using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Argument type that encapsulates data pertinent to a
    /// the progress of sending objects (using C-STORE)
    /// </summary>
    class SendProgressUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="results">The object containing the results from the query.</param>
        public SendProgressUpdatedEventArgs(ReadOnlyQueryResultCollection results)
        {
        }

    }
}
