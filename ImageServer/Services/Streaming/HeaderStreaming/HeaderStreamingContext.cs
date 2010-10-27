#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.ServiceModel.Streaming;

namespace ClearCanvas.ImageServer.Services.Streaming.HeaderStreaming
{
    internal class HeaderStreamingContext
    {
        private string _callerAE;
        private HeaderStreamingParameters _parameters;
        private string _serviceInstanceID;

        public string ServiceInstanceID
        {
            get { return _serviceInstanceID; }
            set { _serviceInstanceID = value; }
        }

        public HeaderStreamingParameters Parameters
        {
            get { return _parameters; }
            set { _parameters = value; }
        }

        public string CallerAE
        {
            get { return _callerAE; }
            set { _callerAE = value; }
        }
    }
}