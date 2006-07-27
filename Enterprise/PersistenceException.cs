using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Abstract base class for all exceptions related to persistence
    /// </summary>
    public abstract class PersistenceException : Exception
    {
        public PersistenceException(string message, Exception inner)
            :base(message, inner)
        {
        }
    }
}
