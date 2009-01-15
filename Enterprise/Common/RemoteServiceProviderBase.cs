using System;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Security;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using System.Security.Cryptography.X509Certificates;

namespace ClearCanvas.Enterprise.Common
{
	#region RemoteServiceProviderArgs class

	/// <summary>
	/// Arguments that are passed to a <see cref="RemoteServiceProviderBase{T}"/>.
	/// </summary>
	public class RemoteServiceProviderArgs
	{
		private string _baseUrl;
		private string _configurationClassName;
		private int _maxReceivedMessageSize;
		private X509CertificateValidationMode _certificateValidationMode;
		private X509RevocationMode _revocationMode;

		/// <summary>
		/// Constructor
		/// </summary>
		public RemoteServiceProviderArgs(string baseUrl, string configurationClassName, int maxReceivedMessageSize,
			X509CertificateValidationMode certificateValidationMode, X509RevocationMode revocationMode)
		{
			_baseUrl = baseUrl;
			_configurationClassName = configurationClassName;
			_maxReceivedMessageSize = maxReceivedMessageSize;
			_certificateValidationMode = certificateValidationMode;
			_revocationMode = revocationMode;
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
		/// Assembly-qualified name of the class that is responsible for configuring the service binding/endpoint.
		/// This class must implement <see cref="IServiceChannelConfiguration"/>.
		/// </summary>
		public string ConfigurationClassName
		{
			get { return _configurationClassName; }
			set { _configurationClassName = value; }
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
	}

	#endregion

	/// <summary>
	/// Abstract base class for remote service provider extensions.
	/// </summary>
	/// <typeparam name="TServiceLayerAttribute">Attribute that identifiers the service layer to which a service belongs.</typeparam>
	public abstract class RemoteServiceProviderBase<TServiceLayerAttribute> : IServiceProvider
		where TServiceLayerAttribute : Attribute
	{
		private readonly RemoteServiceProviderArgs _args;
		private readonly IServiceChannelConfiguration _channelConfiguration;

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="args"></param>
		public RemoteServiceProviderBase(RemoteServiceProviderArgs args)
		{
			_args = args;

			Type configClass = Type.GetType(_args.ConfigurationClassName);
			_channelConfiguration = (IServiceChannelConfiguration)Activator.CreateInstance(configClass);
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
            object channel = CreateChannel(serviceContract, authenticationRequired);

            Platform.Log(LogLevel.Debug, "Created WCF channel instance for service {0}, authenticationRequired={1}.",
                         serviceContract.FullName, authenticationRequired);

            return channel;
        }

		#endregion

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
			ChannelFactory factory = _channelConfiguration.ConfigureChannelFactory(
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
			get { return ""; }
		}

		/// <summary>
		/// Gets the password to pass as a credential to the service.
		/// </summary>
		protected virtual string Password
		{
			get { return ""; }
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

	}
}
