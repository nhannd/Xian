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
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Security;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Security.Cryptography.X509Certificates;
using Castle.DynamicProxy;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Common
{
	#region RemoteServiceProviderArgs class

	/// <summary>
	/// Arguments that are passed to a <see cref="RemoteServiceProviderBase{T}"/>.
	/// </summary>
	public class RemoteServiceProviderArgs
	{
		private string _baseUrl;
        private IServiceChannelConfiguration _configuration;
		private int _maxReceivedMessageSize;
		private X509CertificateValidationMode _certificateValidationMode;
		private X509RevocationMode _revocationMode;
        private IUserCredentialsProvider _userCredentialsProvider;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="baseUrl"></param>
        /// <param name="configurationClassName"></param>
        /// <param name="maxReceivedMessageSize"></param>
        /// <param name="certificateValidationMode"></param>
        /// <param name="revocationMode"></param>
        public RemoteServiceProviderArgs(string baseUrl, string configurationClassName, int maxReceivedMessageSize,
            X509CertificateValidationMode certificateValidationMode, X509RevocationMode revocationMode)
            :this(baseUrl, configurationClassName, maxReceivedMessageSize, certificateValidationMode,
                revocationMode, null)
        {
        }



		/// <summary>
		/// Constructor
		/// </summary>
		public RemoteServiceProviderArgs(string baseUrl, string configurationClassName, int maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode, X509RevocationMode revocationMode,
            string credentialsProviderClassName)
		{
			_baseUrl = baseUrl;
            _configuration = InstantiateClass<IServiceChannelConfiguration>(configurationClassName);
			_maxReceivedMessageSize = maxReceivedMessageSize;
			_certificateValidationMode = certificateValidationMode;
			_revocationMode = revocationMode;
            _userCredentialsProvider = string.IsNullOrEmpty(credentialsProviderClassName) ? null :
                InstantiateClass<IUserCredentialsProvider>(credentialsProviderClassName);
		}

		/// <summary>
		/// Base URL shared by all services in the service layer.
		/// </summary>
		public string BaseUrl
		{
			get { return _baseUrl; }
			set { _baseUrl = value; }
		}

		/// <summary>
		/// Configuration that is responsible for configuring the service binding/endpoint.
		/// </summary>
        public IServiceChannelConfiguration Configuration
		{
			get { return _configuration; }
			set { _configuration = value; }
		}

		/// <summary>
		/// Maximum size in bytes of message received by the service client.
		/// </summary>
		public int MaxReceivedMessageSize
		{
			get { return _maxReceivedMessageSize; }
			set { _maxReceivedMessageSize = value; }
		}

		/// <summary>
		/// Certificate validation mode.
		/// </summary>
		public X509CertificateValidationMode CertificateValidationMode
		{
			get { return _certificateValidationMode; }
			set { _certificateValidationMode = value; }
		}

		/// <summary>
		/// Certificate revocation mode.
		/// </summary>
		public X509RevocationMode RevocationMode
		{
			get { return _revocationMode; }
			set { _revocationMode = value; }
		}

        /// <summary>
        /// Gets or sets an <see cref="IUserCredentialsProvider"/>.
        /// May be null if user credentials are not relevant.
        /// </summary>
        public IUserCredentialsProvider UserCredentialsProvider
        {
            get { return _userCredentialsProvider; }
            set { _userCredentialsProvider = value; }
        }

        private static T InstantiateClass<T>(string className)
        {
			try
			{
				Type type = Type.GetType(className, true);
				return (T)Activator.CreateInstance(type);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e, "Cannot instantiate class {0}", className);
				throw;
			}
		}
    }

	#endregion

    public interface IRemoteServiceProxy
    {
        object GetChannel();
    }

    internal class RemoteServiceProxyMixin : IRemoteServiceProxy
    {
        private object _channel;

        internal RemoteServiceProxyMixin(object channel)
        {
            _channel = channel;
        }

        object IRemoteServiceProxy.GetChannel()
        {
            return _channel;
        }
    }

    internal class DisposableInterceptor : IInterceptor
    {
        public object Intercept(IInvocation invocation, params object[] args)
        {
            // if the method being called is IDisposable.Dispose()
            if (invocation.Method.DeclaringType == typeof(IDisposable))
            {
                // invoke the method directly on the target - do not proceed along interceptor chain
                IDisposable disposable = (IDisposable)invocation.InvocationTarget;
                disposable.Dispose();
                return null;
            }
            else
            {
                // proceed normally
                return invocation.Proceed(args);
            }
        }
    }

	/// <summary>
	/// Abstract base class for remote service provider extensions.
	/// </summary>
	/// <typeparam name="TServiceLayerAttribute">Attribute that identifiers the service layer to which a service belongs.</typeparam>
	public abstract class RemoteServiceProviderBase<TServiceLayerAttribute> : IServiceProvider
		where TServiceLayerAttribute : Attribute
	{
		private readonly RemoteServiceProviderArgs _args;
        private readonly ProxyGenerator _proxyGenerator;
        private List<IInterceptor> _interceptors;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		public RemoteServiceProviderBase(RemoteServiceProviderArgs args)
		{
			_args = args;
            _proxyGenerator = new ProxyGenerator();
        }

        #region IServiceProvider Members

        public object GetService(Type serviceContract)
        {
            // check if the service is provided by this provider
            if (CanProvideService(serviceContract))
                return null;

            AuthenticationAttribute authAttr = AttributeUtils.GetAttribute<AuthenticationAttribute>(serviceContract);
            bool authenticationRequired = authAttr == null ? true : authAttr.AuthenticationRequired;

            // create the channel
            // it is unfortunate that we cannot defer channel creation until an interceptor
            // actually Proceed()s to it, but DynamicProxy1 does not support this (DP2 does!)
            object channel = CreateChannel(serviceContract, authenticationRequired);

            Platform.Log(LogLevel.Debug, "Created WCF channel instance for service {0}, authenticationRequired={1}.",
                         serviceContract.FullName, authenticationRequired);
            //return channel;
            return CreateChannelProxy(serviceContract, channel);
        }

		#endregion

		#region Protected API

        protected virtual void ApplyInterceptors(IList<IInterceptor> interceptors)
        {
            // this must be added as the outer-most interceptor
            // it is basically a hack to prevent the interception chain from acting on a call to Dispose(),
            // because Dispose() is not a service operation
            interceptors.Add(new DisposableInterceptor());

            if (Caching.Cache.IsSupported())
            {
                // add response-caching client-side advice
                interceptors.Add(new ResponseCachingClientAdvice());
            }
        }

        /// <summary>
		/// Gets a value indicating whether this service provider provides the specified service.
		/// </summary>
		/// <remarks>
		/// The default implementation is based on the service contract being marked with the <see cref="TServiceLayerAttribute"/>
		/// attribute.  Override this method to customize which services are provided by this provider.
		/// </remarks>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		protected virtual bool CanProvideService(Type serviceType)
		{
			return !AttributeUtils.HasAttribute<TServiceLayerAttribute>(serviceType);
		}

		/// <summary>
		/// Gets a channel factory instance of the specified class.
		/// </summary>
		/// <remarks>
		/// Override this method to do custom configuration of the channel factory.
		/// </remarks>
		/// <param name="channelFactoryClass"></param>
		/// <param name="uri"></param>
		/// <param name="authenticationRequired"></param>
		/// <returns></returns>
		protected virtual ChannelFactory GetChannelFactory(Type channelFactoryClass, Uri uri, bool authenticationRequired)
		{
			ChannelFactory factory = _args.Configuration.ConfigureChannelFactory(
				new ServiceChannelConfigurationArgs(channelFactoryClass, uri, authenticationRequired,
													_args.MaxReceivedMessageSize,
													_args.CertificateValidationMode,
													_args.RevocationMode));

			if(authenticationRequired)
			{
				factory.Credentials.UserName.UserName = this.UserName;
				factory.Credentials.UserName.Password = this.Password;
			}

			return factory;
		}

		/// <summary>
		/// Gets the user name to pass as a credential to the service.
		/// </summary>
		protected virtual string UserName
		{
			get { return _args.UserCredentialsProvider == null ? "" : _args.UserCredentialsProvider.UserName; }
		}

		/// <summary>
		/// Gets the password to pass as a credential to the service.
		/// </summary>
		protected virtual string Password
		{
            get { return _args.UserCredentialsProvider == null ? "" : _args.UserCredentialsProvider.SessionTokenId; }
        }

		#endregion

		private object CreateChannel(Type serviceType, bool authenticationRequired)
		{
			Uri uri = new Uri(new Uri(_args.BaseUrl), serviceType.FullName);
			Type channelFactoryClass = typeof(ChannelFactory<>).MakeGenericType(new Type[] { serviceType });

			ChannelFactory channelFactory = GetChannelFactory(channelFactoryClass, uri, authenticationRequired);

			// invoke the CreateChannel method on the factory
			MethodInfo createChannelMethod = channelFactory.GetType().GetMethod("CreateChannel", Type.EmptyTypes);
			return createChannelMethod.Invoke(channelFactory, null);
		}

        private object CreateChannelProxy(Type serviceContract, object channel)
        {
            // get list of interceptors if not yet created
            if (_interceptors == null)
            {
                _interceptors = new List<IInterceptor>();
                ApplyInterceptors(_interceptors);
            }

            // build AOP intercept chain
            AopInterceptorChain aopIntercept = new AopInterceptorChain(_interceptors);

            GeneratorContext genContext = new GeneratorContext();
            genContext.AddMixinInstance(new RemoteServiceProxyMixin(channel));

            // create and return proxy
            // note: _proxyGenerator does internal caching based on service contract
            // so subsequent calls based on the same contract will be fast
            // note: important to proxy IDisposable too, otherwise channels can't get disposed!!!
            object proxy = _proxyGenerator.CreateCustomProxy(
                new Type[] { serviceContract, typeof(IDisposable) },
                aopIntercept,
                channel,
                genContext);
            return proxy;
        }
	}
}
