using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ClearCanvas.Server.ShredHost
{
    internal class WcfHelper
    {
        static public Uri GetServiceHostBaseAddress(int port)
        {
            StringBuilder addressBuilder = new StringBuilder();
            addressBuilder.AppendFormat("http://localhost:{0}/", port);
            UriBuilder uriBuilder = new UriBuilder(addressBuilder.ToString());
            return uriBuilder.Uri;
        }

        static public ServiceEndpointDescription StartHost<TServiceType, TServiceInterfaceType>(int port, string name, string description)
        {
            ServiceEndpointDescription sed = new ServiceEndpointDescription(port, name, description);
            sed.Binding = new WSHttpBinding();
            sed.ServiceHost = new ServiceHost(typeof(TServiceType), GetServiceHostBaseAddress(port));

            ServiceMetadataBehavior metadataBehavior = sed.ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>();
            if (null == metadataBehavior)
            {
                metadataBehavior = new ServiceMetadataBehavior();
                metadataBehavior.HttpGetEnabled = true;
                sed.ServiceHost.Description.Behaviors.Add(metadataBehavior);
            }

            ServiceDebugBehavior debugBehavior = sed.ServiceHost.Description.Behaviors.Find<ServiceDebugBehavior>();
            if (null == debugBehavior)
            {
                debugBehavior = new ServiceDebugBehavior();
                debugBehavior.IncludeExceptionDetailInFaults = true;
                sed.ServiceHost.Description.Behaviors.Add(debugBehavior);
            }

            sed.ServiceHost.AddServiceEndpoint(typeof(TServiceInterfaceType), sed.Binding, name);
            sed.ServiceHost.Open();

            return sed;
        }

        static public void StopHost(ServiceEndpointDescription sed)
        {
            sed.ServiceHost.Close();
        }
    }
}