using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Dicom.OffisNetwork
{
    public class RetrieveProgressUpdatedEventArgs : EventArgs
    {

        public RetrieveProgressUpdatedEventArgs(int completedSuboperations, int failedSuboperations, int remainingSuboperations)
        {
            _completedSuboperations = completedSuboperations;
            _failedSuboperations = failedSuboperations;
            _remainingSuboperations = remainingSuboperations;
        }

        private int _completedSuboperations;
        private int _failedSuboperations;
        private int _remainingSuboperations;

        public int RemainingSuboperations
        {
            get { return _remainingSuboperations; }
            set { _remainingSuboperations = value; }
        }
	
        public int FailedSuboperations
        {
            get { return _failedSuboperations; }
            set { _failedSuboperations = value; }
        }
	
        public int CompletedSuboperations
        {
            get { return _completedSuboperations; }
            set { _completedSuboperations = value; }
        }
	
    }
}
