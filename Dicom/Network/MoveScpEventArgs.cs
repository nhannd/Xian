using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Dicom.OffisWrapper;

namespace ClearCanvas.Dicom.Network
{
    public class MoveScpProgressEventArgs : EventArgs
    {
        private int _messageID;
        private T_DIMSE_C_MoveRSP _response;

        public MoveScpProgressEventArgs(int messageID, T_DIMSE_C_MoveRSP response)
        {
            _messageID = messageID;
            _response = response;
        }

        public int MessageID
        {
            get { return _messageID; }
            set { _messageID = value; }
        }

        public T_DIMSE_C_MoveRSP Response
        {
            get { return _response; }
            set { _response = value; }
        }
    }
        public class MoveScpEventArgs : EventArgs
    {
        private int _messageID;
        private string _moveDestination;
        private string _studyInstanceUID;
        private string _description;
        private T_DIMSE_C_MoveRSP _response;

        public MoveScpEventArgs(int messageID, string moveDestination, string studyInstanceUID, string description, T_DIMSE_C_MoveRSP response)
        {
            _messageID = messageID;
            _moveDestination = moveDestination;
            _studyInstanceUID = studyInstanceUID;
            _description = description;
            _response = response;
        }

        public int MessageID
        {
            get { return _messageID; }
            set { _messageID = value; }
        }

        public string MoveDestination
        {
            get { return _moveDestination; }
            set { _moveDestination = value; }
        }

        public string StudyInstanceUID
        {
            get { return _studyInstanceUID; }
            set { _studyInstanceUID = value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }

        public T_DIMSE_C_MoveRSP Response
        {
            get { return _response; }
            set { _response = value; }
        }
    }
}
