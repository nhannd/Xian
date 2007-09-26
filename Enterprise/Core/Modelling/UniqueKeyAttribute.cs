using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core.Modelling
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class UniqueKeyAttribute : Attribute
    {
        private string[] _memberProperties;
        private string _logicalName;

        public UniqueKeyAttribute(string logicalName, string[] memberProperties)
        {
            _logicalName = logicalName;
            _memberProperties = memberProperties;
        }

        public string[] MemberProperties
        {
            get { return _memberProperties; }
        }

        public string LogicalName
        {
            get { return _logicalName; }
        }
    }
}
