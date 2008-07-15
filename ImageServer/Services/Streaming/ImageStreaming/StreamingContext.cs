using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Model;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    public class StreamingContext
    {
        private HttpListenerRequest _request;
        private HttpListenerResponse _response;
        private string _studyInstanceUid;
        private string _seriesInstanceUid;
        private string _objectUid;
        private StudyStorageLocation _studylocation;

        public string StudyInstanceUid
        {
            get { return _studyInstanceUid; }
            set { _studyInstanceUid = value; }
        }

        public string SeriesInstanceUid
        {
            get { return _seriesInstanceUid; }
            set { _seriesInstanceUid = value; }
        }

        public string ObjectUid
        {
            get { return _objectUid; }
            set { _objectUid = value; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _studylocation; }
            set { _studylocation = value; }
        }

        public HttpListenerRequest Request
        {
            get { return _request; }
            set { _request = value; }
        }

        public HttpListenerResponse Response
        {
            get { return _response; }
            set { _response = value; }
        }
    }
}