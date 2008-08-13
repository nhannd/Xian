using ClearCanvas.DicomServices.ServiceModel.Streaming;

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