using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom.Network
{
    public class StoreScpProgressUpdateEventArgs : EventArgs
    {
        private int _messageID;
        private string _sopInstanceUID;
        private long _progressBytes;
        private long _totalBytes;

        public StoreScpProgressUpdateEventArgs(InteropStoreScpCallbackInfo callbackInfo)
        {
            if (callbackInfo != null)
            {
                _messageID = callbackInfo.Request.MessageID;
                _sopInstanceUID = callbackInfo.Request.AffectedSOPInstanceUID;
                _progressBytes = callbackInfo.Progress.progressBytes;
                _totalBytes = callbackInfo.Progress.totalBytes;
            }
        }

        public int MessageID
        {
            get { return _messageID; }
            set { _messageID = value; }
        }

        public string SOPInstanceUID
        {
            get { return _sopInstanceUID; }
            set { _sopInstanceUID = value; }
        }

        public long ProgressBytes
        {
            get { return _progressBytes; }
            set { _progressBytes = value; }
        }

        public long TotalBytes
        {
            get { return _totalBytes; }
            set { _totalBytes = value; }
        }
    }

    public class StoreScpImageReceivedEventArgs : EventArgs
    {
        private string _fileName;

        public StoreScpImageReceivedEventArgs(InteropStoreScpCallbackInfo callbackInfo)
        {
            if (callbackInfo != null)
                _fileName = callbackInfo.FileName;
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }
    }
}
