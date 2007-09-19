using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class UniqueKeyAttribute : Attribute
    {
        private string _compositeKey;

        /// <summary>
        /// Optional name of composite key.  If supplied, the property is made part of that
        /// composite key.  If omitted, the property itself is designated as a unique key.
        /// </summary>
        public string CompositeKey
        {
            get { return _compositeKey; }
            set { _compositeKey = value; }
        }

    }
}
