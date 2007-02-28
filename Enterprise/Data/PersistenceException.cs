using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Base class for all exceptions related to persistence
    /// </summary>
    public class PersistenceException : Exception
    {
        public PersistenceException(string message, Exception inner)
            :base(message, inner)
        {
        }
    }
}
