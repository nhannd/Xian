using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class MimeTypeProcessorOutput
    {
        private string _contentType;
        private byte[] _output;
        private bool _isCompressed;
        private bool _isLast;

        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public byte[] Output
        {
            get { return _output; }
            set { _output = value; }
        }

        public bool IsLast
        {
            get { return _isLast; }
            set { _isLast = value; }
        }

        public bool IsCompressed
        {
            get { return _isCompressed; }
            set { _isCompressed = value; }
        }
    }
}