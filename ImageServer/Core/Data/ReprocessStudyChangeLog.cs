using System;

namespace ClearCanvas.ImageServer.Core.Data
{
    public class ReprocessStudyChangeLog
    {
        #region Private Members
        private string _studyInstanceUid;
        private DateTime _timeStamp;
        private string _user;
        private string _reason; 
        #endregion

        #region Public Properties
        public DateTime TimeStamp
        {
            get { return _timeStamp; }
            set { _timeStamp = value; }
        }

        public string Reason
        {
            get { return _reason; }
            set { _reason = value; }
        }

        public string User
        {
            get { return _user; }
            set { _user = value; }
        }

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        #endregion
    }
}