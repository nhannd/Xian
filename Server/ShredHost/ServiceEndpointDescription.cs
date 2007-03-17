using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ClearCanvas.Server.ShredHost
{
    internal class ServiceEndpointDescription
    {
        public ServiceEndpointDescription(string name, string description)
        {
            _serviceName = name;
            _serviceDescription = description;
        }

        private string _serviceName;
        private string _serviceDescription;
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

        public string ServiceName
        {
            get { return _serviceName; }
        }

        public string ServiceDescription
        {
            get { return _serviceDescription; }
        }
    }
}
