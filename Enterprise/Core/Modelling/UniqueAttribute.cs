using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UniqueAttribute : Attribute
    {
        private bool _unique;

        public UniqueAttribute()
        {
            _unique = true;
        }

        public UniqueAttribute(bool unique)
        {
            _unique = unique;
        }
    }
}
