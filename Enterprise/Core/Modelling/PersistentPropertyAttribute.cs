using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class PersistentPropertyAttribute : Attribute
    {
        public PersistentPropertyAttribute()
        {

        }
    }
}
