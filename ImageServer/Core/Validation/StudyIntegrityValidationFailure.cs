using System;

namespace ClearCanvas.ImageServer.Core.Exceptions
{
    /// <summary>
    /// Represents exception thrown when study integrity validation fails.
    /// </summary>
    public class StudyIntegrityValidationFailure : Exception
    {
        public StudyIntegrityValidationFailure(string error):base(error)
        {
            
        }
    }
}