using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class WorkQueueItemNotFoundException : BaseWebException
    {
        public WorkQueueItemNotFoundException(string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.WorkQueueItemNotFound);
            ErrorDescription = ExceptionMessages.WorkQueueItemNotFoundDescription;
            LogMessage = logMessage;
        }

        public WorkQueueItemNotFoundException()
        {
            ErrorMessage = string.Format(ExceptionMessages.WorkQueueItemNotFound);
            ErrorDescription = ExceptionMessages.WorkQueueItemNotFoundDescription;
            LogMessage = string.Empty;
        }
    }
}
