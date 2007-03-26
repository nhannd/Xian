using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Desktop;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
    internal class DicomSessionKey
    {
        private string _aeTitle;
        private string _hostName;
        private int _messageID;

        public DicomSessionKey(string aeTitle, string hostName, int messageID)
        {
            _aeTitle = aeTitle;
            _hostName = hostName;
            _messageID = messageID;
        }

        #region Properties

        public string AETitle
        {
            get { return _aeTitle; }
        }

        public string HostName
        {
            get { return _hostName; }
        }

        public int MessageID
        {
            get { return _messageID; }
        }

        #endregion

        #region Override functions

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;

            DicomSessionKey key = obj as DicomSessionKey;
            if (key == obj)
                return true;

            if (_aeTitle != key._aeTitle)
                return false;

            if (_hostName != key._hostName)
                return false;

            if (_messageID != key._messageID)
                return false;

            return true;
        }

        public override int GetHashCode()
        {
            return _aeTitle.GetHashCode() + _hostName.GetHashCode() + _messageID.GetHashCode();
        }

        #endregion
    }

	internal class DicomQuerySession
    {
        ReadOnlyQueryResultCollection _queryResults;
        int _currentIndex;

        public DicomQuerySession(ReadOnlyQueryResultCollection queryResults)
        {
            _queryResults = queryResults;
            _currentIndex = 0;
        }

        #region Properties

        public ReadOnlyQueryResultCollection QueryResults
        {
            get { return _queryResults; }
            set { _queryResults = value; }
        }

        public int CurrentIndex
        {
            get { return _currentIndex; }
            set { _currentIndex = value; }
        }

        #endregion
    }

    internal class DicomMoveSession
    {
        private SendParcel _parcel;
        private BackgroundTask _task;
        private int _progress = 0;

        public DicomMoveSession(SendParcel parcel, BackgroundTask task)
        {
            _parcel = parcel;
            _task = task;
            _progress = 0;
        }

        #region Properties

        public SendParcel Parcel
        {
            get { return _parcel; }
            set { _parcel = value; }
        }

        public BackgroundTask Task
        {
            get { return _task; }
            set { _task = value; }
        }

        public int Progress
        {
            get { return _progress; }
            set { _progress = value; }
        }

        #endregion
    }
}
