#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

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