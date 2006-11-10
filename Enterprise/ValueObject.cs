using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Base class for domain objects that behave as value types (NHiberate: components, or collection of values)
    /// </summary>
    public abstract class ValueObject : ICloneable
    {
        #region ICloneable Members

        public abstract object Clone();

        #endregion
    }
}
