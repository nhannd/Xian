using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Indicates that an entity's version has changed, and does not match the requested version
    /// </summary>
    public class EntityVersionException : PersistenceException
    {
        public EntityVersionException(object oid, Exception inner)
            : base(string.Format(SR.ExceptionEntityVersion, oid), inner)
        {
        }
    }
}
