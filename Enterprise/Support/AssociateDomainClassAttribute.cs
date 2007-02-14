using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Support
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class AssociateDomainClassAttribute : Attribute
    {
        private Type _domainClass;
        public AssociateDomainClassAttribute(Type domainClass)
        {
            _domainClass = domainClass;
        }

        public Type DomainClass
        {
            get { return _domainClass; }
        }
    }
}
