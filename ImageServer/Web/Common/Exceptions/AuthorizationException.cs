using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class AuthorizationException : BaseWebException
    {
        public AuthorizationException(string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.AuthorizationRequired);
            ErrorDescription = ExceptionMessages.AuthorizationRequiredDescription;
            LogMessage = logMessage;
        }

        public AuthorizationException()
        {
            ErrorMessage = string.Format(ExceptionMessages.AuthorizationRequired);
            ErrorDescription = ExceptionMessages.AuthorizationRequiredDescription;
            LogMessage = string.Empty;
        }
    }
}
