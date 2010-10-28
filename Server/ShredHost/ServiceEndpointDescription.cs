#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.ServiceModel;
using System.ServiceModel.Channels;

namespace ClearCanvas.Server.ShredHost
{
    public class ServiceEndpointDescription
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
