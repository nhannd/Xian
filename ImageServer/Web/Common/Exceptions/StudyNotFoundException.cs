using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageServer.Web.Common.Exceptions
{
    public class StudyNotFoundException : BaseWebException
    {
        public StudyNotFoundException(string studyInstanceUID, string logMessage)
        {
            ErrorMessage = string.Format(ExceptionMessages.StudyNotFound, studyInstanceUID);
            ErrorDescription = ExceptionMessages.StudyNotFoundDescription;
            LogMessage = logMessage;
        }

        public StudyNotFoundException(string studyInstanceUID)
        {
            ErrorMessage = string.Format(ExceptionMessages.StudyNotFound, studyInstanceUID);
            ErrorDescription = ExceptionMessages.StudyNotFoundDescription;
            LogMessage = string.Empty;
        }
    }
}
