namespace ClearCanvas.ImageServer.Core.Data
{
    public class ReprocessStudyQueueData
    {
        private ReprocessStudyState _state;
        private ReprocessStudyChangeLog _changeLog;

        public ReprocessStudyState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public ReprocessStudyChangeLog ChangeLog
        {
            get { return _changeLog; }
            set { _changeLog = value; }
        }

    }

    public class ReprocessStudyState
    {
        private bool _executeAtLeastOnce;
        private bool _completed;
        private int _completeAttemptCount;

        public bool ExecuteAtLeastOnce
        {
            get { return _executeAtLeastOnce; }
            set { _executeAtLeastOnce = value; }
        }

        public bool Completed
        {
            get { return _completed; }
            set { _completed = value; }
        }

        public int CompleteAttemptCount
        {
            get { return _completeAttemptCount; }
            set { _completeAttemptCount = value; }
        }
    }
}