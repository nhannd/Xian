using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ClearCanvas.Server.ShredHost
{
	public enum HostBindingType
	{ 
		WSHttp = 0,
		WSDualHttp,
		NetTcp,
		NetTcpDual,
	}

	internal class HostBindingInformation
	{
		private HostBindingType _hostBindingType;
		private int _httpPort;
		private int _tcpPort;

		public HostBindingInformation(int httpPort, int tcpPort)
			: this(HostBindingType.WSHttp, httpPort, tcpPort)
		{ 
		}

		public HostBindingInformation(HostBindingType hostBindingType, int httpPort, int tcpPort)
		{
			_hostBindingType = hostBindingType;
			_tcpPort = tcpPort;
			_httpPort = httpPort;
		}

		public HostBindingType HostBindingType
		{
			get { return _hostBindingType; }
			set { _hostBindingType = value; }
		}

		public int HttpPort
		{
			get { return _httpPort; }
			set { _httpPort = value; }
		}

		public int TcpPort
		{
			get { return _tcpPort; }
			set { _tcpPort = value; }
		}

		public Binding NewBinding()
		{
			if (_hostBindingType == HostBindingType.NetTcp || _hostBindingType == HostBindingType.NetTcpDual)
			{
				return new NetTcpBinding();
			}
			else if (_hostBindingType == HostBindingType.WSDualHttp)
			{
				return new WSDualHttpBinding();
			}

			return new WSHttpBinding();
		}
		
		public int ServicePort
		{
			get
			{
				if (_hostBindingType == HostBindingType.NetTcp || _hostBindingType == HostBindingType.NetTcpDual)
					return this.TcpPort;

				return this.HttpPort;
			}
		}

		public Uri[] GetServiceHostBaseAddresses(string endpointName)
		{
			List<Uri> endpoints = new List<Uri>();

			if (_hostBindingType == HostBindingType.NetTcp || _hostBindingType == HostBindingType.NetTcpDual)
			{
				endpoints.Add(new UriBuilder(String.Format("net.tcp://localhost:{0}/{1}", this.TcpPort, endpointName)).Uri);
			}

			endpoints.Add(new UriBuilder(String.Format("http://localhost:{0}/{1}", this.HttpPort, endpointName)).Uri);
			return endpoints.ToArray();
		}
	}

	internal class WcfHelper
    {
		static public ServiceEndpointDescription StartHost<TServiceType, TServiceInterfaceType>(int port, string name, string description)
		{ 
			return StartHost<TServiceType, TServiceInterfaceType>(name, description, new HostBindingInformation(port, 0));
		}

		static public ServiceEndpointDescription StartHost<TServiceType, TServiceInterfaceType>(string name, string description, HostBindingInformation bindingInformation)
        {
			ServiceEndpointDescription sed = new ServiceEndpointDescription(bindingInformation.ServicePort, name, description);

			sed.Binding = bindingInformation.NewBinding();

			sed.ServiceHost = new ServiceHost(typeof(TServiceType), bindingInformation.GetServiceHostBaseAddresses(name));

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