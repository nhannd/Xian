using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.OffisNetwork
{
    /// <summary>
    /// Argument type that encapsulates data pertinent to a
    /// the progress of sending objects (using C-STORE)
    /// </summary>
    public class SendProgressUpdatedEventArgs : EventArgs
    {
		private string _sentSopInstanceUid;
		private int _currentCount;
		private int _totalCount;
				
		/// <summary>
		/// Mandatory Constructor.
		/// </summary>
		/// <param name="sentSopInstanceUid">The sopInstanceUid that has been sent</param>
		/// <param name="currentCount">The running count of instances sent</param>
		/// <param name="totalCount">The total number of instances to be sent</param>
		public SendProgressUpdatedEventArgs(string sentSopInstanceUid, int currentCount, int totalCount)
        {
			_sentSopInstanceUid = sentSopInstanceUid;
			_currentCount = currentCount;
            _totalCount = totalCount;
        }

		public string SentSopInstanceUid
		{
			get { return _sentSopInstanceUid; }
		}

        public int TotalCount
        {
            get { return _totalCount; }
        }
	
        public int CurrentCount
        {
            get { return _currentCount; }
        }
    }
}
