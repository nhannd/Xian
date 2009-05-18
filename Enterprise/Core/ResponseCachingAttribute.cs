using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple=false)]
    public class ResponseCachingAttribute : Attribute
    {
        private string _directiveMethod;

        public ResponseCachingAttribute(string directiveMethod)
        {
            _directiveMethod = directiveMethod;
        }

        public string DirectiveMethod
        {
            get { return _directiveMethod; }
        }
    }
}
