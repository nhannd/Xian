using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Application.Services
{
    public class ValidationResult
    {
        private bool _success;
        private  int _errorCode;
        private  String _errorText;

        public bool Success
        {
            get { return _success; }
            set { _success = value; }
        }

        public int ErrorCode
        {
            get { return _errorCode; }
            set { _errorCode = value; }
        }

        public string ErrorText
        {
            get { return _errorText; }
            set { _errorText = value; }
        }
    }
}
