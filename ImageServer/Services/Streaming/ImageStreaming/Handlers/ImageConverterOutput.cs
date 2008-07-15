using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming.Handlers
{
    public class ImageConverterOutput
    {
        private string _contentType;
        private Stream _stream;
        private bool _hasMore;

        public string ContentType
        {
            get { return _contentType; }
            set { _contentType = value; }
        }

        public Stream Stream
        {
            get { return _stream; }
            set { _stream = value; }
        }

        public bool HasMoreFrame
        {
            get { return _hasMore; }
            set { _hasMore = value; }
        }
    }
}