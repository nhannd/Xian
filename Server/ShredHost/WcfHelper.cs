using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ClearCanvas.Server.ShredHost
{
	internal sealed class WcfHelper
    {
		private enum HostBindingType
		{
			WSHttp = 0,
			WSDualHttp,
			NetTcp,
			NamedPipes
		}

		static public ServiceEndpointDescription StartHttpHost<TServiceType, TServiceInterfaceType>(string name, string description, int port)
		{ 
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.WSHttp, port, 0);
		}

		static public ServiceEndpointDescription StartHttpDualHost<TServiceType, TServiceInterfaceType>(string name, string description, int port)
		{
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.WSDualHttp, port, 0);
		}

		static public ServiceEndpointDescription StartNetTcpHost<TServiceType, TServiceInterfaceType>(string name, string description, int port, int metaDataHttpPort)
		{
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.NetTcp, metaDataHttpPort, port);
		}

		static public ServiceEndpointDescription StartNetPipeHost<TServiceType, TServiceInterfaceType>(string name, string description, int metaDataHttpPort)
		{
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.NamedPipes, metaDataHttpPort, 0);
		}

		static private ServiceEndpointDescription StartHost<TServiceType, TServiceInterfaceType>
			(
				string name, 
				string description, 
				HostBindingType bindingType,
				int httpPort,
				int tcpPort)
        {

			ServiceEndpointDescription sed = new ServiceEndpointDescription(name, description);

			if (bindingType == HostBindingType.NetTcp)
			{
				sed.Binding = new NetTcpBinding();
				//sed.Binding.PortSharingEnabled = true;
			}
			else if (bindingType == HostBindingType.NamedPipes)
			{
				sed.Binding = new NetNamedPipeBinding();
			}
			else if (bindingType == HostBindingType.WSDualHttp)
			{
				sed.Binding = new WSDualHttpBinding();
			}
			else
				sed.Binding = new WSHttpBinding();

			sed.ServiceHost = new ServiceHost(typeof(TServiceType), GetServiceHostBaseAddresses(name, bindingType, tcpPort, httpPort));

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

		private static Uri[] GetServiceHostBaseAddresses(string endpointName, HostBindingType bindingType, int tcpPort, int httpPort)
		{
			List<Uri> endpoints = new List<Uri>();

			if (bindingType == HostBindingType.NetTcp)
			{
				endpoints.Add(new UriBuilder(String.Format("net.tcp://localhost:{0}/{1}", tcpPort, endpointName)).Uri);
			}
			else if (bindingType == HostBindingType.NamedPipes)
			{
				//the servicehost will automatically append the endpointname.
				endpoints.Add(new UriBuilder("net.pipe://localhost/Shreds/").Uri);
			}

			endpoints.Add(new UriBuilder(String.Format("http://localhost:{0}/{1}", httpPort, endpointName)).Uri);

			return endpoints.ToArray();
		}

	}
}