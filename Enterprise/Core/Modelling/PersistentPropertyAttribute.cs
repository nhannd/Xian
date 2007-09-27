using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to a property of a domain object class, indicates that the property is persistent.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PersistentPropertyAttribute : Attribute
    {
        public PersistentPropertyAttribute()
        {

        }
    }
}
