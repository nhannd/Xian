using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class RequiredAttribute : Attribute
    {
        private bool _required;

        public RequiredAttribute()
        {
            _required = true;
        }

        public RequiredAttribute(bool required)
        {
            _required = required;
        }
    }
}
