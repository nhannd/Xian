using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Base class for domain objects that behave as value types (NHiberate: components, or collection of values)
    /// </summary>
    public abstract class ValueObject : DomainObject, ICloneable
    {
        #region ICloneable Members

        public abstract object Clone();

        #endregion
    }
}
