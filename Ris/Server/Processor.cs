using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Runtime.Remoting;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Application.Services;

namespace ClearCanvas.Ris.Server
{
	class Processor
	{
		private List<ServiceHost> _serviceHosts = new List<ServiceHost>();

		public void Start()
		{
			Platform.Log(LogLevel.Info, "Starting application root {0}", this.GetType().FullName);

			string baseAddress = WebServicesSettings.Default.BaseUrl;

			_serviceHosts = new List<ServiceHost>();

			MountServices(new CoreServiceExtensionPoint(), baseAddress);
			MountServices(new ApplicationServiceExtensionPoint(), baseAddress);

			Platform.Log(LogLevel.Info, "Starting WCF services on {0}...", baseAddress);
			foreach (ServiceHost host in _serviceHosts)
			{
				host.Open();
			}
			Platform.Log(LogLevel.Info, "WCF Services started on {0}", baseAddress);

			// kick NHibernate, rather than waiting for it to load on demand
			PersistentStoreRegistry.GetDefaultStore();

			// configure the alternate remoting channel
			RemotingConfiguration.Configure("ClearCanvas.Ris.Server.Executable.exe.config", false);
		}

		public void RequestStop()
		{
			Platform.Log(LogLevel.Info, "Stopping WCF services...");
			foreach (ServiceHost host in _serviceHosts)
			{
				host.Close();
			}
			Platform.Log(LogLevel.Info, "WCF services stopped.");
		}

		#region Private Helpers

		private void MountServices(IExtensionPoint serviceLayer, string baseAddress)
		{
			IServiceFactory serviceFactory = new ServiceFactory(serviceLayer);
			ApplyInterceptors(serviceFactory);

			ICollection<Type> serviceClasses = serviceFactory.ListServiceClasses();
			foreach (Type serviceClass in serviceClasses)
			{
				MountService(serviceClass, serviceFactory, baseAddress);
			}
		}

		private void MountService(Type serviceClass, IServiceFactory serviceFactory, string baseAddress)
		{
			ServiceImplementsContractAttribute contractAttribute = AttributeUtils.GetAttribute<ServiceImplementsContractAttribute>(serviceClass, false);
			if (contractAttribute == null)
			{
				Platform.Log(LogLevel.Error, "Unknown contract for service {0}", serviceClass.Name);
				return;
			}

			Platform.Log(LogLevel.Info, "Mounting service {0}", serviceClass.Name);

			// determine if service requires authentication
			AuthenticationAttribute authenticationAttribute = AttributeUtils.GetAttribute<AuthenticationAttribute>(contractAttribute.ServiceContract);
			bool authenticated = authenticationAttribute == null ? true : authenticationAttribute.AuthenticationRequired;

			// create service URI
			Uri uri = new Uri(new Uri(baseAddress), contractAttribute.ServiceContract.FullName);

			// create service host
			ServiceHost host = new ServiceHost(serviceClass, uri);

			// build service according to binding
			Type configClass = Type.GetType(WebServicesSettings.Default.ConfigurationClass);
			IServiceHostConfiguration configuration = (IServiceHostConfiguration)Activator.CreateInstance(configClass);
			configuration.ConfigureServiceHost(host,
				new ServiceHostConfigurationArgs(
					contractAttribute.ServiceContract,
					uri, authenticated,
					WebServicesSettings.Default.MaxReceivedMessageSize,
					authenticated ? new CustomUserValidator() : null,
					authenticated ? new CustomAuthorizationPolicy() : null));

			// add behaviour to grab AOP proxied service instance
			host.Description.Behaviors.Add(new InstanceManagementServiceBehavior(contractAttribute.ServiceContract, serviceFactory));

			// adjust some service behaviours
			ServiceBehaviorAttribute serviceBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
			if (serviceBehavior != null)
			{
				// set instance mode to "per call"
				serviceBehavior.InstanceContextMode = InstanceContextMode.PerCall;

				// determine whether to send exception detail back to client
				if (WebServicesSettings.Default.SendExceptionDetailToClient)
				{
					serviceBehavior.IncludeExceptionDetailInFaults = true;
				}
			}

			_serviceHosts.Add(host);
		}

		private static void ApplyInterceptors(IServiceFactory serviceFactory)
		{
			// the order in which the interceptors are added is extremely important
			// the first interceptor in the list will be the outermost, and
			// the last interceptor in the list will be the innermost

			// add exception promotion advice at the beginning of the interception chain (outside of the service transaction)
			serviceFactory.Interceptors.Add(new ExceptionPromotionAdvice());

			if (WebServicesSettings.Default.EnablePerformanceLogging)
			{
				// add performance logging advice
				serviceFactory.Interceptors.Add(new PerformanceLoggingAdvice());
			}

			// exception logging occurs outside of the main persistence context
			serviceFactory.Interceptors.Add(new ExceptionLoggingAdvice());

			// add persistence context advice, that controls the persistence context for the main transaction
			serviceFactory.Interceptors.Add(new PersistenceContextAdvice());

			// add audit advice inside of main persistence context advice,
			// so that the audit record will be inserted as part of the main transaction (this applies only to update contexts)
			serviceFactory.Interceptors.Add(new AuditAdvice());

		}

		#endregion
	}
}
