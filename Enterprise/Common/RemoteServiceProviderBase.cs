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
using Castle.Core.Interceptor;
using System.Collections.Generic;

namespace ClearCanvas.Enterprise.Common
{
	#region RemoteServiceProviderArgs class

	/// <summary>
	/// Encapsulates options that confgiure a <see cref="RemoteServiceProviderBase{T}"/>.
	/// </summary>
	public class RemoteServiceProviderArgs
	{
		private string _baseUrl;
		private string _failoverBaseUrl;
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
		[Obsolete("Use another constructor overload")]
		public RemoteServiceProviderArgs(
			string baseUrl,
			string configurationClassName,
			int maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode,
			X509RevocationMode revocationMode)
			: this(baseUrl, null, configurationClassName, maxReceivedMessageSize, certificateValidationMode,
				revocationMode, null)
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="baseUrl"></param>
		/// <param name="failoverBaseUrl"></param>
		/// <param name="configurationClassName"></param>
		/// <param name="maxReceivedMessageSize"></param>
		/// <param name="certificateValidationMode"></param>
		/// <param name="revocationMode"></param>
		public RemoteServiceProviderArgs(
			string baseUrl,
			string failoverBaseUrl,
			string configurationClassName,
			int maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode,
			X509RevocationMode revocationMode)
			: this(baseUrl, failoverBaseUrl, configurationClassName, maxReceivedMessageSize, certificateValidationMode,
				revocationMode, null)
		{
		}



		/// <summary>
		/// Constructor
		/// </summary>
		public RemoteServiceProviderArgs(
			string baseUrl,
			string failoverBaseUrl,
			string configurationClassName,
			int maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode,
			X509RevocationMode revocationMode,
			string credentialsProviderClassName)
		{
			_baseUrl = baseUrl;
			_failoverBaseUrl = failoverBaseUrl;
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
		/// Failover base URL shared by all services in the service layer.
		/// </summary>
		public string FailoverBaseUrl
		{
			get { return _failoverBaseUrl; }
			set { _failoverBaseUrl = value; }
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


	/// <summary>
	/// Abstract base class for remote service provider extensions.
	/// </summary>
	public abstract class RemoteServiceProviderBase : IServiceProvider
	{
		#region RemoteServiceProxyMixin class

		/// <summary>
		/// Remote service proxy mix-in class.
		/// </summary>
		internal class RemoteServiceProxyMixin : IRemoteServiceProxy
		{
			private readonly object _channel;

			internal RemoteServiceProxyMixin(object channel)
			{
				_channel = channel;
			}

			/// <summary>
			/// Gets the channel object.
			/// </summary>
			/// <returns></returns>
			object IRemoteServiceProxy.GetChannel()
			{
				return _channel;
			}
		}

		#endregion

		#region DisposableInterceptor

		/// <summary>
		/// Interceptor that ensure <see cref="IDisposable"/> is honoured.
		/// </summary>
		internal class DisposableInterceptor : IInterceptor
		{
			public void Intercept(IInvocation invocation)
			{
				if (InvocationMethodIsDispose(invocation))
				{
					//TODO before calling dispose, we should check if the target implements
					// IClientChannel, and if so, call Close()
					//if(invocation.InvocationTarget is IClientChannel)
					//{
					//    IClientChannel channel = (IClientChannel)invocation.InvocationTarget;
					//    channel.Close();
					//}

					// Dispose and then do not proceed along the interceptor chain
					GetDisposableInvocationTarget(invocation).Dispose();
				}
				else
				{
					// proceed normally
					invocation.Proceed();
				}
			}

			private bool InvocationMethodIsDispose(IInvocation invocation)
			{
				return invocation.Method.DeclaringType == typeof(IDisposable)
					&& invocation.Method.Name == "Dispose";
			}

			private IDisposable GetDisposableInvocationTarget(IInvocation invocation)
			{
				return (IDisposable)ResolveProxiedInvocationTarget(invocation);
			}

			private object ResolveProxiedInvocationTarget(IInvocation invocation)
			{
				var invocationTarget = invocation.InvocationTarget;

				if ((invocationTarget is IProxyTargetAccessor))
				{
					// this is odd - it is not clear whether InvocationTarget is the proxy or the target
					// DP seems to do different things under different circumstances, probably due to bugs
					// we do the safe thing and check if we need to access the inner object
					return ((IProxyTargetAccessor)invocationTarget).DynProxyGetTarget();
				}
				else
				{
					// invoke the method directly on the target
					return invocationTarget;
				}
			}
		}

		#endregion

		private readonly ProxyGenerator _proxyGenerator;
		private readonly IChannelFactoryProvider _channelFactoryProvider;
		private readonly IUserCredentialsProvider _userCredentialsProvider;
		private List<IInterceptor> _interceptors;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		protected RemoteServiceProviderBase(RemoteServiceProviderArgs args)
			: this(new StaticChannelFactoryProvider(args), args.UserCredentialsProvider)
		{
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="channelFactoryProvider"></param>
		/// <param name="userCredentialsProvider"></param>
		protected RemoteServiceProviderBase(IChannelFactoryProvider channelFactoryProvider, IUserCredentialsProvider userCredentialsProvider)
		{
			_channelFactoryProvider = channelFactoryProvider;
			_userCredentialsProvider = userCredentialsProvider;
			_proxyGenerator = new ProxyGenerator();
		}

		#region IServiceProvider

		public object GetService(Type serviceContract)
		{
			// check if the service is provided by this provider
			if (CanProvideService(serviceContract))
				return null;

			// create the channel
			// TODO: defer channel creation until an interceptor
			// actually Proceed()s to it (DP2 supports this)
			ChannelFactory factory = _channelFactoryProvider.GetPrimary(serviceContract);
			object channel = CreateChannel(serviceContract, factory);

			// create an AOP proxy around the channel, and return that
			return CreateChannelProxy(serviceContract, channel);
		}

		#endregion

		#region Protected API

		/// <summary>
		/// Gets a value indicating whether this service provider provides the specified service.
		/// </summary>
		protected abstract bool CanProvideService(Type serviceType);

		/// <summary>
		/// Applies AOP interceptors to the proxy.
		/// </summary>
		/// <remarks>
		/// Override this method to customize which interceptors are applied to the
		/// proxy by adding/removing or inserting into the specified list.
		/// The order of interceptors is significant.  The first entry
		/// in the list is the outermost, and the last entry in the list is the 
		/// innermost.
		/// </remarks>
		/// <param name="interceptors"></param>
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

			// add fail-over advice at the end of the list, closest the target call
			interceptors.Add(new FailoverClientAdvice(this));
		}

		/// <summary>
		/// Gets the user name to pass as a credential to the service.
		/// </summary>
		protected virtual string UserName
		{
			get { return _userCredentialsProvider == null ? "" : _userCredentialsProvider.UserName; }
		}

		/// <summary>
		/// Gets the password to pass as a credential to the service.
		/// </summary>
		protected virtual string Password
		{
			get { return _userCredentialsProvider == null ? "" : _userCredentialsProvider.SessionTokenId; }
		}

		/// <summary>
		/// Attempts to get a failover channel for the specified service contract.
		/// </summary>
		/// <param name="serviceContract"></param>
		/// <param name="failedEndpoint"></param>
		/// <returns></returns>
		protected internal object GetFailoverChannel(Type serviceContract, EndpointAddress failedEndpoint)
		{
			ChannelFactory alternate = _channelFactoryProvider.GetFailover(serviceContract, failedEndpoint);
			return alternate != null ? CreateChannel(serviceContract, alternate) : null;
		}

		#endregion

		#region Helpers

		/// <summary>
		/// Creates a channel for the specified contract, using the specified factory.
		/// Note that this method modifies the factory, therefore it cannot be re-used!
		/// </summary>
		/// <param name="serviceContract"></param>
		/// <param name="factory"></param>
		/// <returns></returns>
		private object CreateChannel(Type serviceContract, ChannelFactory factory)
		{
			bool authenticationRequired = AuthenticationAttribute.IsAuthenticationRequired(serviceContract);
			if (authenticationRequired)
			{
				factory.Credentials.UserName.UserName = this.UserName;
				factory.Credentials.UserName.Password = this.Password;
			}

			// invoke the CreateChannel method on the factory
			MethodInfo createChannelMethod = factory.GetType().GetMethod("CreateChannel", Type.EmptyTypes);
			object channel = createChannelMethod.Invoke(factory, null);
			Platform.Log(LogLevel.Debug, "Created service channel instance for service {0}, authenticationRequired={1}.",
						 serviceContract.FullName, authenticationRequired);
			return channel;
		}

		private object CreateChannelProxy(Type serviceContract, object channel)
		{
			// get list of interceptors if not yet created
			if (_interceptors == null)
			{
				_interceptors = new List<IInterceptor>();
				ApplyInterceptors(_interceptors);
			}

			var options = new ProxyGenerationOptions();
			options.AddMixinInstance(new RemoteServiceProxyMixin(channel));

			// create and return proxy
			// note: _proxyGenerator does internal caching based on service contract
			// so subsequent calls based on the same contract will be fast
			// note: important to proxy IDisposable too, otherwise channels can't get disposed!!!
			return _proxyGenerator.CreateInterfaceProxyWithTarget(
				serviceContract,
				new[] { serviceContract, typeof(IDisposable) },
				channel,
				options,
				_interceptors.ToArray());
		}

		#endregion
	}

	/// <summary>
	/// Abstract base class for remote service provider extensions.
	/// </summary>
	/// <typeparam name="TServiceLayerAttribute">Attribute that identifiers the service layer to which a service belongs.</typeparam>
	public abstract class RemoteServiceProviderBase<TServiceLayerAttribute> : RemoteServiceProviderBase
		where TServiceLayerAttribute : Attribute
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		protected RemoteServiceProviderBase(RemoteServiceProviderArgs args)
			: base(args)
		{
		}

		#region Protected API

		/// <summary>
		/// Gets a value indicating whether this service provider provides the specified service.
		/// </summary>
		/// <remarks>
		/// The default implementation is based on the service contract being marked with the <see cref="TServiceLayerAttribute"/>
		/// attribute.  Override this method to customize which services are provided by this provider.
		/// </remarks>
		/// <param name="serviceType"></param>
		/// <returns></returns>
		protected override bool CanProvideService(Type serviceType)
		{
			return !AttributeUtils.HasAttribute<TServiceLayerAttribute>(serviceType);
		}

		#endregion
	}
}
