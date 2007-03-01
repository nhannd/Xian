using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.Enterprise.Core
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false, Inherited=false)]
    public class ServiceImplementsContractAttribute : Attribute
    {
        private Type _serviceContract;

        public ServiceImplementsContractAttribute(Type serviceContract)
        {
            _serviceContract = serviceContract;
        }

        public Type ServiceContract
        {
            get { return _serviceContract; }
        }
    }
}
