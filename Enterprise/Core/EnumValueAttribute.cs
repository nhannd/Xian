using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    /// <summary>
    /// Allows meta-data to be specified for each member of a C# enum.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple=false, Inherited=false)]
    public class EnumValueAttribute : Attribute
    {
        private string _value;
        private string _description;

        public EnumValueAttribute(string value)
        {
            _value = value;
        }

        public string Value
        {
            get { return _value; }
        }

        public string Description
        {
            get { return _description; }
            set { _description = value; }
        }
    }
}
