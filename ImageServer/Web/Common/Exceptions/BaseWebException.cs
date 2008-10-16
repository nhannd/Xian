using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class BaseWebException : Exception
    {
        private string _logMessage;
        private string _errorMessage;
        private string _errorDescription;

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set { _errorMessage = value; }
        }

        public string LogMessage
        {
            get { return _logMessage; }
            set { _logMessage = value; }
        }

        public string ErrorDescription
        {
            get { return _errorDescription; }
            set { _errorDescription = value; }
        }

        public BaseWebException(string errorMessage, string logMessage, string errorDescription)
        {
            _errorMessage = errorMessage;
            _errorDescription = errorDescription;
            _logMessage = logMessage;
        }

        public BaseWebException() {}

    }
}
