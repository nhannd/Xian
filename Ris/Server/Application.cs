using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using System.ServiceModel;
using ClearCanvas.Common.Utilities;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

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

            ICollection<Type> serviceClasses = Core.ServiceManager.ListServices();
            foreach (Type serviceClass in serviceClasses)
            {
                ServiceImplementationAttribute a = CollectionUtils.FirstElement<ServiceImplementationAttribute>(
                    serviceClass.GetCustomAttributes(typeof(ServiceImplementationAttribute), false));
                if (a != null)
                {
                    Console.WriteLine("Starting service " + serviceClass.FullName);

                    ServiceHost host = new ServiceHost(serviceClass, baseAddress);
                    host.Description.Behaviors.Add(new InstanceManagementServiceBehavior(a.ServiceContract));

                    host.AddServiceEndpoint(a.ServiceContract, binding, a.ServiceContract.FullName);

                    ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (metadataBehavior == null)
                    {
                        metadataBehavior = new ServiceMetadataBehavior();
                        metadataBehavior.HttpGetEnabled = true;
                        host.Description.Behaviors.Add(metadataBehavior);
                    }


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
