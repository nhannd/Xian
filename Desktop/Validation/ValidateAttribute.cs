using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Desktop.Validation
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class ValidateAttribute : Attribute
    {
        private string _displayName;
        private bool _mandatory;

        public ValidateAttribute(string displayName, bool mandatory)
        {
            _displayName = displayName;
            _mandatory = mandatory;
        }

        public bool Mandatory
        {
            get { return _mandatory; }
        }

        public string DisplayName
        {
            get { return _displayName; }
        }
    }
}
