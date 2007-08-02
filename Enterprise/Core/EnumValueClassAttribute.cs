using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    [AttributeUsage(AttributeTargets.Enum, AllowMultiple = false, Inherited = false)]
    public class EnumValueClassAttribute : Attribute
    {
        private Type _enumValueClass;

        public EnumValueClassAttribute(Type enumValueClass)
        {
            _enumValueClass = enumValueClass;
        }

        public Type EnumValueClass
        {
            get { return _enumValueClass; }
        }
    }
}
