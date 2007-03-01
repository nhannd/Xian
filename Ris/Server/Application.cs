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
            Binding binding = new BasicHttpBinding();

            IServiceManager serviceManager = new ServiceManager();

            ICollection<Type> serviceClasses = serviceManager.ListServices();
            foreach (Type serviceClass in serviceClasses)
            {
                ServiceImplementationAttribute a = CollectionUtils.FirstElement<ServiceImplementationAttribute>(
                    serviceClass.GetCustomAttributes(typeof(ServiceImplementationAttribute), false));
                if (a != null)
                {
                    Console.WriteLine("Starting service " + serviceClass.FullName);

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


                    host.Open();

                    _serviceHosts.Add(host);
                }
                else
                {
                    Console.WriteLine("Unknown contract for service " + serviceClass.FullName);
                }
            }

            //Can do blocking calls: Application.Run(new MyForm());
            Console.Read();

            Console.WriteLine("Stopping services...");

            foreach (ServiceHost host in _serviceHosts)
            {
                host.Close();
            }
        }

        #endregion
    }
}
