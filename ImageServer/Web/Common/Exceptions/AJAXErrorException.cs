using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class AJAXErrorException : BaseWebException
    {
        public AJAXErrorException(string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.AJAXError);
            ErrorDescription = ExceptionMessages.AJAXErrorDescription;
            LogMessage = logMessage;
        }

        public AJAXErrorException()
        {
            ErrorMessage = string.Format(ExceptionMessages.AJAXError);
            ErrorDescription = ExceptionMessages.AJAXErrorDescription;
            LogMessage = ExceptionMessages.EmptyLogMessage;
        }
    }
}
