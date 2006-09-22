using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.Network
{
    /// <summary>
    /// Argument type that encapsulates data pertinent to a
    /// the progress of sending objects (using C-STORE)
    /// </summary>
    public class SendProgressUpdatedEventArgs : EventArgs
    {
        /// <summary>
        /// Mandatory constructor.
        /// </summary>
        /// <param name="results">The object containing the results from the query.</param>
        public SendProgressUpdatedEventArgs(int currentCount, int totalCount)
        {
            _currentCount = currentCount;
            _totalCount = totalCount;
        }

        private int _currentCount;
        private int _totalCount;

        public int TotalCount
        {
            get { return _totalCount; }
            set { _totalCount = value; }
        }
	
        public int CurrentCount
        {
            get { return _currentCount; }
            set { _currentCount = value; }
        }
    }
}
