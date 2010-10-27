#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Net;
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
        private string _contentType;
        private StudyStorageLocation _studylocation;

        private string[] _acceptTypes;

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

        public string ContentType
        {
            get
            {
                return _contentType;
            }
            set { _contentType = value; }
        }

        public string[] AcceptTypes
        {
            get { return _acceptTypes; }
            set { _acceptTypes = value; }
        }

        public HttpListenerResponse Response
        {
            get { return _response; }
            set { _response = value; }
        }
    }
}