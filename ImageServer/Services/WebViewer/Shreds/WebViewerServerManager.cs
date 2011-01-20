#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using ClearCanvas.Server.ShredHost;
using ClearCanvas.Web.Services;

namespace ClearCanvas.ImageServer.Services.WebViewer.Shreds
{
    class WebViewerServerManager
    {
        private readonly List<ServiceHost> _serviceHosts = new List<ServiceHost>();

        private static readonly WebViewerServerManager _instance = new WebViewerServerManager();

        public static WebViewerServerManager Instance
        {
            get { return _instance; }
        }

        private WebViewerServerManager(){}

        public void Start()
        {
			var endpoints = new[] { new Uri(string.Format("net.tcp://localhost:{0}", ShredHostServiceSettings.Instance.SharedTcpPort))};
			ApplicationServiceHost host = new ApplicationServiceHost(endpoints);
#if DEBUG
			host.MexHttpUrl = new Uri(String.Format("http://localhost:{0}", ShredHostServiceSettings.Instance.SharedHttpPort));
#endif
			_serviceHosts.Add(host);
            
            foreach (ServiceHost theHost in _serviceHosts)
            {
                theHost.Open();
            }
        }

        public void Stop()
        {
			Application.StopAll("The server has been shut down.");

            foreach (ServiceHost theHost in _serviceHosts)
            {
                theHost.Close();
            }
        }
    }
}