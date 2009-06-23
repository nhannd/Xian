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

        public bool ExecuteAtLeastOnce
        {
            get { return _executeAtLeastOnce; }
            set { _executeAtLeastOnce = value; }
        }
    }
}