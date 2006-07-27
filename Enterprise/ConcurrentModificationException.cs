using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Indicates that an attempt was made to modify an entity that was already modified by another user
    /// </summary>
    public class ConcurrentModificationException : PersistenceException
    {
        public ConcurrentModificationException(Exception inner)
            : base("An entity was modified by another user", inner)
        {
        }
    }
}
