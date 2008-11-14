using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class StudyInstanceUIDNotFoundException : BaseWebException
    {
        public StudyInstanceUIDNotFoundException(string studyInstanceUID, string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.StudyNotFound, studyInstanceUID);
            ErrorDescription = ExceptionMessages.StudyNotFoundDescription;
            LogMessage = logMessage;
        }

        public StudyInstanceUIDNotFoundException(string studyInstanceUID)
        {
            ErrorMessage = string.Format(ExceptionMessages.StudyNotFound, studyInstanceUID);
            ErrorDescription = ExceptionMessages.StudyNotFoundDescription;
            LogMessage = ExceptionMessages.EmptyLogMessage;
        }
    }
}
