using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to a property of a domain class, indicates that the property value is unique
    /// within the set of all of persistent instances of the class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UniqueAttribute : Attribute
    {
        public UniqueAttribute()
        {
        }
    }
}
