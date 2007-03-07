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
using ClearCanvas.Ris.Test.Services;
using System.IdentityModel.Policy;
using System.Security.Cryptography.X509Certificates;

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
 
            string baseAddress = "http://localhost:8000/";

            _serviceHosts = new List<ServiceHost>();

            _serviceHosts.AddRange(MountServices(new CoreServiceExtensionPoint(), baseAddress));
            _serviceHosts.AddRange(MountServices(new TestServiceExtensionPoint(), baseAddress));

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

        private List<ServiceHost> MountServices(IExtensionPoint serviceLayer, string baseAddress)
        {
            WSHttpBinding binding = new WSHttpBinding();
            binding.Security.Mode = SecurityMode.Message;
            binding.Security.Message.ClientCredentialType = MessageCredentialType.UserName;


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
                    Uri uri = new Uri(string.Format("{0}/{1}", baseAddress, a.ServiceContract.FullName));
                    ServiceHost host = new ServiceHost(serviceClass, uri);

                    // add behaviour to grab AOP proxied service instance
                    host.Description.Behaviors.Add(new InstanceManagementServiceBehavior(a.ServiceContract, serviceManager));

                    // establish endpoint
                    host.AddServiceEndpoint(a.ServiceContract, binding, "");

                    // expose meta-data via HTTP GET
                    ServiceMetadataBehavior metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                    if (metadataBehavior == null)
                    {
                        metadataBehavior = new ServiceMetadataBehavior();
                        metadataBehavior.HttpGetEnabled = true;
                        host.Description.Behaviors.Add(metadataBehavior);
                    }
#if DEBUG
                    ServiceBehaviorAttribute debuggingBehavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
                    debuggingBehavior.IncludeExceptionDetailInFaults = true;
#endif
                    // set up authentication model
                    host.Credentials.UserNameAuthentication.UserNamePasswordValidationMode = UserNamePasswordValidationMode.Custom;
                    host.Credentials.UserNameAuthentication.CustomUserNamePasswordValidator = new CustomUserValidator();

                    // set up the certificate
                    host.Credentials.ServiceCertificate.SetCertificate(
                        StoreLocation.LocalMachine, StoreName.My, X509FindType.FindBySubjectName, "localhost");

                    // set up authorization
                    List<IAuthorizationPolicy> policies = new List<IAuthorizationPolicy>();
                    policies.Add(new CustomAuthorizationPolicy());
                    host.Authorization.ExternalAuthorizationPolicies = policies.AsReadOnly();
                    host.Authorization.PrincipalPermissionMode = PrincipalPermissionMode.Custom;

                        

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
