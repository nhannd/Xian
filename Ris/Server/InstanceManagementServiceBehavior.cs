using System;
using System.Collections.Generic;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;
using System.ServiceModel.Dispatcher;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Ris.Server
{
    /// <summary>
    /// Based on code found here: http://orand.blogspot.com/2006/10/wcf-service-dependency-injection.html
    /// </summary>
    class InstanceManagementServiceBehavior : IServiceBehavior
    {
        private Type _serviceContract;
        private IServiceFactory _serviceManager;

        public InstanceManagementServiceBehavior(Type serviceContract, IServiceFactory serviceManager)
        {
            _serviceContract = serviceContract;
            _serviceManager = serviceManager;
        }


        #region IServiceBehavior Members

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcherBase cdb in serviceHostBase.ChannelDispatchers)
            {
                ChannelDispatcher cd = cdb as ChannelDispatcher;
                if (cd != null)
                {
                    foreach (EndpointDispatcher ed in cd.Endpoints)
                    {
                        ed.DispatchRuntime.InstanceProvider =
                            new ProxiedServiceInstanceProvider(_serviceContract, _serviceManager);
                    }
                }
            }
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
        }

        #endregion
    }
}
