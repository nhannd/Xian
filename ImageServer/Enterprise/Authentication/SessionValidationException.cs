using System;

namespace ClearCanvas.ImageServer.Enterprise.Authentication
{
    /// <summary>
    /// Represents an exception thrown when a <see cref="SessionInfo"/> cannot be 
    /// validated.
    /// </summary>
    public class SessionValidationException : Exception
    {
        public SessionValidationException()
        {
        }

        public SessionValidationException(Exception baseException)
            : base(baseException.Message, baseException)
        {
            
        }

    }
}