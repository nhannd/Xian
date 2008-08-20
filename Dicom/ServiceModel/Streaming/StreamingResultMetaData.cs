using System;
using System.Net;
using ClearCanvas.Common.Statistics;

namespace ClearCanvas.Dicom.ServiceModel.Streaming
{
    /// <summary>
    /// Represents the result of an image streaming operation
    /// </summary>
    public class StreamingResultMetaData
    {
        private HttpStatusCode _status;
        private string _statusDescription;
        private string _mimeType;
        private long _contentLength;
        private Uri _uri;

        private RateStatistics _speed = new RateStatistics("Speed", RateType.BYTES);
        
        public HttpStatusCode Status
        {
            get { return _status; }
            set { _status = value; }
        }

        public string StatusDescription
        {
            get { return _statusDescription; }
            set { _statusDescription = value; }
        }

        public string ResponseMimeType
        {
            get { return _mimeType; }
            set { _mimeType = value; }
        }


        public Uri Uri
        {
            get { return _uri; }
            set { _uri = value; }
        }

        public RateStatistics Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public long ContentLength
        {
            get { return _contentLength; }
            set { _contentLength = value; }
        }

    }

    /// <summary>
    /// Represents the result of a frame streaming operation
    /// </summary>
    public class FrameStreamingResultMetaData : StreamingResultMetaData
    {
        private bool _isLast = false;

        /// <summary>
        /// Indicates whether the current frame is the last frame in the image.
        /// </summary>
        public bool IsLast
        {
            get { return _isLast; }
            set { _isLast = value; }
        }
    }
}
