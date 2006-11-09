using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    public abstract class ValueObject : ICloneable
    {
        #region ICloneable Members

        public abstract object Clone();

        #endregion
    }
}
