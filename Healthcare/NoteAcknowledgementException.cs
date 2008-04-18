using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Healthcare
{
    /// <summary>
    /// Exception thrown when an error occurs with <see cref="Note"/> acknowledgement.
    /// </summary>
    public class NoteAcknowledgementException : Exception
    {
        internal NoteAcknowledgementException(string message)
            :base(message)
        {
        }
    }
}
