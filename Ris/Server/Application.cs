using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Security;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Ris.Services;

namespace ClearCanvas.Ris.Server
{
    [ExtensionOf(typeof(ApplicationRootExtensionPoint))]
    public class Application : IApplicationRoot
    {
        private List<ServiceHost> _serviceHosts = new List<ServiceHost>();

        #region IApplicationRoot Members

        public void RunApplication(string[] args)
        {
            Console.WriteLine("Starting application root " + this.GetType().FullName);
 
            Uri baseAddress = new Uri("http://localhost:8000/");

            _serviceHosts = new List<ServiceHost>();

            _serviceHosts.AddRange(MountServices(new CoreServiceExtensionPoint(), baseAddress));
            _serviceHosts.AddRange(MountServices(new ApplicationServiceExtensionPoint(), baseAddress));

            Console.WriteLine("Starting services...");
            foreach (ServiceHost host in _serviceHosts)
            {
                host.Open();
            }
            Console.WriteLine("Services started.");
            Console.WriteLine("PRESS ANY KEY TO EXIT");

            Console.Read();

            Console.WriteLine("Stopping services...");
            foreach (ServiceHost host in _serviceHosts)
            {
                host.Close();
            }
        }

        #endregion

        private List<ServiceHost> MountServices(ExtensionPoint<IServiceLayer> serviceLayer, Uri baseAddress)
        {
            Binding binding = new BasicHttpBinding();
            List<ServiceHost> hostedServices = new List<ServiceHost>();

            IServiceFactory serviceManager = new ServiceFactory(serviceLayer);
            ICollection<Type> serviceClasses = serviceManager.ListServices();
            foreach (Type serviceClass in serviceClasses)
            {
                ServiceImplementsContractAttribute a = CollectionUtils.FirstElement<ServiceImplementsContractAttribute>(
                    serviceClass.GetCustomAttributes(typeof(ServiceImplementsContractAttribute), false));
                if (a != null)
                {
                    Console.WriteLine("Mounting service " + serviceClass.FullName);

                    // create service host
                    ServiceHost host = new ServiceHost(serviceClass, baseAddress);

                    // add behaviour to grab AOP proxied service instance
                    host.Description.Behaviors.Add(new InstanceManagementServiceBehavior(a.ServiceContract, serviceManager));

                    // establish endpoint
                    host.AddServiceEndpoint(a.ServiceContract, binding, a.ServiceContract.FullName);

                    // expose meta-data via HTTP GET
                    ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (metadataBehavior == null)
                    {
                        metadataBehavior = new ServiceMetadataBehavior();
                        metadataBehavior.HttpGetEnabled = true;
                        host.Description.Behaviors.Add(metadataBehavior);
                    }

                    // set up authentication model
                    host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                    host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new UserValidator();

                    hostedServices.Add(host);
                }
                else
                {
                    Console.WriteLine("Unknown contract for service " + serviceClass.FullName);
                }
            }
            return hostedServices;
        }
    }
}
