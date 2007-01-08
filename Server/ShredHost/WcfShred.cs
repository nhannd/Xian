using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ClearCanvas.Server.ShredHost
{
    public abstract class WcfShred : ShredBase
    {
        public WcfShred()
        {
            _serviceEndpointDescriptions = new Dictionary<string,ServiceEndpointDescription>();

        }

        public override object InitializeLifetimeService()
        {
            // I can't find any documentation yet, that says that returning null 
            // means that the lifetime of the object should not expire after a timeout
            // but the initial solution comes from this page: http://www.dotnet247.com/247reference/msgs/13/66416.aspx
            return null;
        }

        static protected Uri GetServiceHostBaseAddress(int port)
        {
            StringBuilder addressBuilder = new StringBuilder();
            addressBuilder.AppendFormat("http://localhost:{0}/", port);
            UriBuilder uriBuilder = new UriBuilder(addressBuilder.ToString());
            return uriBuilder.Uri;
        }

        protected void StartHost<TServiceType, TServiceInterfaceType>(int port, string name, string description)
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

            sed.ServiceHost.AddServiceEndpoint(typeof(TServiceInterfaceType), sed.Binding, name);
            _serviceEndpointDescriptions.Add(name, sed);

            sed.ServiceHost.Open();
        }

        protected void StopHost(string name)
        {
            if (_serviceEndpointDescriptions.ContainsKey(name))
            {
                _serviceEndpointDescriptions[name].ServiceHost.Close();
            }
            else
            {
                // TODO: throw an exception, since a name of a service endpoint that is
                // passed in here that doesn't exist should be considered a programming error
            }
        }

        #region Private Members
        class ServiceEndpointDescription
        {
            public ServiceEndpointDescription(int port, string name, string description)
            {
                _hostPort = port;
                _serviceName = name;
                _serviceDescription = description;
            }

            private string _serviceName;
            private string _serviceDescription;
            private int _hostPort;
            private Binding _binding;
            private ServiceHost _serviceHost;       

            public ServiceHost ServiceHost
            {
                get { return _serviceHost; }
                set { _serviceHost = value; }
            }
	
            public Binding Binding
            {
                get { return _binding; }
                set { _binding = value; }
            }
	
            public int HostPort
            {
                get { return _hostPort; }
                set { _hostPort = value; }
            }
	
            public string ServiceName
            {
                get { return _serviceName; }
            }

            public string ServiceDescription
            {
                get { return _serviceDescription; }
            }
        }

        private Dictionary<string, ServiceEndpointDescription> _serviceEndpointDescriptions;
        #endregion
    }
}
