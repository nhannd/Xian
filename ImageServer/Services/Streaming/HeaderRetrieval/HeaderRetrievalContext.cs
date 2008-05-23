namespace ClearCanvas.ImageServer.Services.Streaming.HeaderRetrieval
{
    internal class HeaderRetrievalContext
    {
        private string _callerAE;
        private HeaderRetrievalParameters _parameters;
        private string _serviceInstanceID;

        public string ServiceInstanceID
        {
            get { return _serviceInstanceID; }
            set { _serviceInstanceID = value; }
        }

        public HeaderRetrievalParameters Parameters
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