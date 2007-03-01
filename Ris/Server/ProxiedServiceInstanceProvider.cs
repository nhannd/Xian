using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using System.ServiceModel;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Server
{
    class ProxiedServiceInstanceProvider : IInstanceProvider
    {
        private Type _serviceContract;
        private IServiceManager _serviceManager;

        public ProxiedServiceInstanceProvider(Type serviceContract, IServiceManager serviceManager)
        {
            _serviceContract = serviceContract;
            _serviceManager = serviceManager;
        }

        #region IInstanceProvider Members

        public object GetInstance(InstanceContext instanceContext, Message message)
        {
            return _serviceManager.GetService(_serviceContract);
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
