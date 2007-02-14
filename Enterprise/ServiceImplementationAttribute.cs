using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ServiceImplementationAttribute : Attribute
    {
        private Type _serviceContract;

        public ServiceImplementationAttribute(Type serviceContract)
        {
            _serviceContract = serviceContract;
        }

        public Type ServiceContract
        {
            get { return _serviceContract; }
        }
    }
}
