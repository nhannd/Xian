#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using System.IdentityModel.Selectors;
using System.IdentityModel.Policy;
using Castle.DynamicProxy;

namespace ClearCanvas.Enterprise.Core.ServiceModel
{
    /// <summary>
    /// Mounts a set of related services on a common base URI using a common binding configuration.
    /// </summary>
    /// <remarks>
    /// Simplifies the process of hosting a number of related WCF services by limiting the amount
    /// of configuration required and applying the configuration across the entire set of services.
    /// </remarks>
    public class ServiceMount
    {
        private Uri _baseAddress;
        private IServiceHostConfiguration _configuration;
        private UserNamePasswordValidator _userValidator = new DefaultUserValidator();
        private IAuthorizationPolicy _authorizationPolicy = new DefaultAuthorizationPolicy();
        private bool _sendExceptionDetailToClient = false;
        private bool _enablePerformanceLogging = false;
        private int _maxReceivedMessageSize = 1000000;
        private InstanceContextMode _instanceMode = InstanceContextMode.PerCall;



        private List<ServiceHost> _serviceHosts = new List<ServiceHost>();

        /// <summary>
        /// Constructs a service mount that hosts services on the specified base URI
        /// using the specified service host configuration.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="configuration"></param>
        public ServiceMount(Uri baseAddress, IServiceHostConfiguration configuration)
        {
            _baseAddress = baseAddress;
            _configuration = configuration;
        }

        /// <summary>
        /// Constructs a service mount that hosts services on the specified base URI
        /// using the specified service host configuration.
        /// </summary>
        /// <param name="baseAddress"></param>
        /// <param name="serviceHostConfigurationClass"></param>
        public ServiceMount(Uri baseAddress, string serviceHostConfigurationClass)
            : this(baseAddress, (IServiceHostConfiguration)InstantiateClass(serviceHostConfigurationClass))
        {
        }

        #region Public API

        /// <summary>
        /// Gets or sets the base address on which services will be hosted.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public Uri BaseAddress
        {
            get { return _baseAddress; }
            set { _baseAddress = value; }
        }

        /// <summary>
        /// Gets or sets the configuration that will be applied to hosted services.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public IServiceHostConfiguration ServiceHostConfiguration
        {
            get { return _configuration; }
            set { _configuration = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether exception detail should be returned to the client.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public bool SendExceptionDetailToClient
        {
            get { return _sendExceptionDetailToClient; }
            set { _sendExceptionDetailToClient = value; }
        }

        /// <summary>
        /// Gets or set the maximum size of received messages in bytes.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public int MaxReceivedMessageSize
        {
            get { return _maxReceivedMessageSize; }
            set { _maxReceivedMessageSize = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether performance logging is enabled.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public bool EnablePerformanceLogging
        {
            get { return _enablePerformanceLogging; }
            set { _enablePerformanceLogging = value; }
        }

        /// <summary>
        /// Gets or sets the service instance mode.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public InstanceContextMode InstanceMode
        {
            get { return _instanceMode; }
            set { _instanceMode = value; }
        }

        /// <summary>
        /// Gets or sets the authorization policy that is used to authorize authenticated services.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public IAuthorizationPolicy AuthorizationPolicy
        {
            get { return _authorizationPolicy; }
            set { _authorizationPolicy = value; }
        }

        /// <summary>
        /// Gets or sets the user validator that is used to authenticate services that require authentication.
        /// </summary>
        /// <remarks>
        /// Must be set prior to calling <see cref="AddServices"/>.
        /// </remarks>
        public UserNamePasswordValidator UserValidator
        {
            get { return _userValidator; }
            set { _userValidator = value; }
        }

        /// <summary>
        /// Adds all services defined by the specified service layer extension point.
        /// </summary>
        /// <remarks>
        /// This method internally calls the <see cref="ApplyInterceptors"/> method to decorate
        /// the services with a set of AOP interceptors.
        /// </remarks>
        /// <param name="serviceLayer"></param>
        public void AddServices(IExtensionPoint serviceLayer)
        {
            IServiceFactory serviceFactory = new ServiceFactory(serviceLayer);
            ApplyInterceptors(serviceFactory.Interceptors);
            AddServices(serviceFactory);
        }

        /// <summary>
        /// Gets the list of <see cref="ServiceHost"/> objects created by calls to <see cref="AddServices"/>.
        /// </summary>
        public IList<ServiceHost> ServiceHosts
        {
            get { return _serviceHosts.AsReadOnly(); }
        }

        /// <summary>
        /// Open all mounted services.
        /// </summary>
        public void OpenServices()
        {
            foreach (ServiceHost host in _serviceHosts)
            {
                host.Open();
            }
        }

        /// <summary>
        /// Close all mounted services.
        /// </summary>
        public void CloseServices()
        {
            foreach (ServiceHost host in _serviceHosts)
            {
                host.Close();
            }
        }

        #endregion

        #region Protected API

        /// <summary>
        /// Applies interceptors to the mounted services.
        /// </summary>
        /// <remarks>
        /// The default implementation of this method applies a default set of interceptors
        /// that are suitable for many common scenarios.  Override this method to customize the 
        /// interceptors by adding or removing items from the specified list.
        /// The order in which the interceptors are added is significant
        /// the first interceptor in the list will be the outermost, and
        /// the last interceptor in the list will be the innermost.
        /// </remarks>
        /// <param name="interceptors"></param>
        protected virtual void ApplyInterceptors(IList<IInterceptor> interceptors)
        {

            // add exception promotion advice at the beginning of the interception chain (outside of the service transaction)
            interceptors.Add(new ExceptionPromotionAdvice());

            // add performance logging advice conditionally
            if (_enablePerformanceLogging)
            {
                interceptors.Add(new PerformanceLoggingAdvice());
            }

            // exception logging occurs outside of the main persistence context
            interceptors.Add(new ExceptionLoggingAdvice());

            // add persistence context advice, that controls the persistence context for the main transaction
            interceptors.Add(new PersistenceContextAdvice());

            // add audit advice inside of main persistence context advice,
            // so that the audit record will be inserted as part of the main transaction (this applies only to update contexts)
            interceptors.Add(new AuditAdvice());
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Adds all services provided by the specified service factory.
        /// </summary>
        /// <param name="serviceFactory"></param>
        private void AddServices(IServiceFactory serviceFactory)
        {
            ICollection<Type> serviceClasses = serviceFactory.ListServiceClasses();
            foreach (Type serviceClass in serviceClasses)
            {
                AddService(serviceClass, serviceFactory);
            }
        }

        private void AddService(Type serviceClass, IServiceFactory serviceFactory)
        {
            ServiceImplementsContractAttribute contractAttribute = AttributeUtils.GetAttribute<ServiceImplementsContractAttribute>(serviceClass, false);
            if (contractAttribute == null)
                throw new ServiceMountException(string.Format("Unknown contract for service {0}", serviceClass.Name));

            Platform.Log(LogLevel.Info, "Mounting service {0}", serviceClass.Name);

            // determine if service requires authentication
            AuthenticationAttribute authenticationAttribute = AttributeUtils.GetAttribute<AuthenticationAttribute>(contractAttribute.ServiceContract);
            bool authenticated = authenticationAttribute == null ? true : authenticationAttribute.AuthenticationRequired;

            // create service URI
            Uri uri = new Uri(_baseAddress, contractAttribute.ServiceContract.FullName);

            Platform.Log(LogLevel.Info, "on URI {0}", uri);

            // create service host
            ServiceHost host = new ServiceHost(serviceClass, uri);

            // build service according to binding
            _configuration.ConfigureServiceHost(host,
                new ServiceHostConfigurationArgs(
                    contractAttribute.ServiceContract,
                    uri, authenticated,
                    _maxReceivedMessageSize,
                    authenticated ? _userValidator : null,
                    authenticated ? _authorizationPolicy : null));

            // add behaviour to inject AOP proxy service factory
            host.Description.Behaviors.Add(new ServiceFactoryInjectionServiceBehavior(contractAttribute.ServiceContract, serviceFactory));

            // adjust some service behaviours
            ServiceBehaviorAttribute serviceBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            if (serviceBehavior != null)
            {
                // set instance mode to "per call"
                serviceBehavior.InstanceContextMode = _instanceMode;

                // determine whether to send exception detail back to clients
                serviceBehavior.IncludeExceptionDetailInFaults = _sendExceptionDetailToClient;
            }

            _serviceHosts.Add(host);
        }

        private static object InstantiateClass(string className)
        {
            Type type = Type.GetType(className);
            return Activator.CreateInstance(type);
        }

        #endregion
    }
}
