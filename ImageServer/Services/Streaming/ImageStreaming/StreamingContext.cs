#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

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