using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using ClearCanvas.Common;

namespace ClearCanvas.Enterprise.Core
{
    /// <summary>
    /// Base class for <see cref="Entity"/>, <see cref="ValueObject"/> and <see cref="EnumValue"/>.
    /// </summary>
    /// 
    [Serializable] // TH (Oct 5, 2007): All entity objects should be serializable to use in ASP.NET app
    public abstract class DomainObject
    {
        /// <summary>
        /// In the case where this object is a proxy, returns the raw instance underlying the proxy.  This
        /// method must be virtual for correct behaviour, however, it is not intended to be overridden by
        /// subclasses and is not intended for use by application code.
        /// </summary>
        /// <returns></returns>
        protected virtual DomainObject GetRawInstance()
        {
            return this;
        }

        /// <summary>
        /// Gets the domain class of this object.  Note that the domain class is not necessarily the same as the
        /// type of this object, because this object may be a proxy.  Therefore, use this method rather
        /// than <see cref="object.GetType"/>.
        /// </summary>
        /// <returns></returns>
        public virtual Type GetClass()
        {
            return GetRawInstance().GetType();
        }
    }
}
