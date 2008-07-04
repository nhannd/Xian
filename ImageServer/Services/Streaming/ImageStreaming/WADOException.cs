using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Services.Streaming.ImageStreaming
{
    public class WADOException : Exception
    {
        private int _HttpErrorCode;
        public WADOException(int code, string message)
            : base(message)
        {
            HttpErrorCode = code;
        }

        public int HttpErrorCode
        {
            get { return _HttpErrorCode; }
            set { _HttpErrorCode = value; }
        }
    }
}
