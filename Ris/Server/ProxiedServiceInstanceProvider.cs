using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;

namespace ClearCanvas.Ris.Server
{
    class ProxiedServiceInstanceProvider : IInstanceProvider
    {
        private Type _serviceContract;

        public ProxiedServiceInstanceProvider(Type serviceContract)
        {
            _serviceContract = serviceContract;
        }

        #region IInstanceProvider Members

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return ClearCanvas.Enterprise.Core.ServiceManager.GetService(_serviceContract);
        }

        public object GetInstance(InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(InstanceContext instanceContext, object instance)
        {
        }

        #endregion
    }
}
