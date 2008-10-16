using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class PartitionNotFoundException : BaseWebException
    {
        public PartitionNotFoundException(string serverAE, string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.PartitionNotFound, serverAE);
            ErrorDescription = ExceptionMessages.PartitonNotFoundDescription;
            LogMessage = logMessage;
        }

        public PartitionNotFoundException(string serverAE)
        {
            ErrorMessage = string.Format(ExceptionMessages.PartitionNotFound, serverAE);
            ErrorDescription = ExceptionMessages.PartitonNotFoundDescription;
            LogMessage = ExceptionMessages.EmptyLogMessage;
        }
    }
}
