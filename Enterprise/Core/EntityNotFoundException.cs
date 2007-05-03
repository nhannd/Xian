using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Thrown when an <see cref="EntityRef"/> cannot be resovled.
    /// </summary>
    public class EntityNotFoundException : PersistenceException
    {
        public EntityNotFoundException(Exception inner)
            : base(SR.ExceptionEntityNotFound, inner)
        {
        }

        public EntityNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
