using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    /// <summary>
    /// When applied to a property of a domain object class, indicates that the property is
    /// required to have a value (e.g. must not be null).
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute
    {
        public RequiredAttribute()
        {
        }
    }
}
