#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;

using System.ServiceModel;
using System.ServiceModel.Description;
using System.ServiceModel.Channels;
using System.Collections.ObjectModel;
using System.ServiceModel.Dispatcher;
using ClearCanvas.Enterprise.Core;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    /// <summary>
    /// Implementation of <see cref="IServiceBehaviour"/> that is used to customize a WCF
    /// service host such that it obtains instances of service implementations from
    /// a <see cref="IServiceFactory"/>.
    /// </summary>
    /// <remarks>
    /// Based on code found here: http://orand.blogspot.com/2006/10/wcf-service-dependency-injection.html
    /// </remarks>
    class ServiceFactoryInjectionServiceBehavior : IServiceBehavior
    {
        #region IInstanceProvider implementation

        class InstanceProvider : IInstanceProvider
        {
            private Type _serviceContract;
            private IServiceFactory _serviceFactory;

            internal InstanceProvider(Type serviceContract, IServiceFactory serviceFactory)
            {
                _serviceContract = serviceContract;
                _serviceFactory = serviceFactory;
            }

            #region IInstanceProvider Members

            public object GetInstance(InstanceContext instanceContext, Message message)
            {
                return _serviceFactory.GetService(_serviceContract);
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

        #endregion

        private Type _serviceContract;
        private IServiceFactory _serviceManager;

        /// <summary>
        /// Constructs an instance of the service behaviour that obtains instances of the
        /// specified service contract from the specified service factory.
        /// </summary>
        /// <param name="serviceContract"></param>
        /// <param name="serviceManager"></param>
        public ServiceFactoryInjectionServiceBehavior(Type serviceContract, IServiceFactory serviceManager)
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
                            new InstanceProvider(_serviceContract, _serviceManager);
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