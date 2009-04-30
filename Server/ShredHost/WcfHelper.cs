#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using ClearCanvas.Common;

namespace ClearCanvas.Server.ShredHost
{
	internal sealed class WcfHelper
    {
		private enum HostBindingType
		{
            BasicHttp,
			WSHttp,
			WSDualHttp,
			NetTcp,
			NamedPipes
		}

        static public ServiceEndpointDescription StartBasicHttpHost<TServiceType, TServiceInterfaceType>(string name, string description, int port)
        {
            return StartHost<TServiceType, TServiceInterfaceType>(name, description, HostBindingType.BasicHttp, port, 0);
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

			sed.Binding = GetBinding<TServiceInterfaceType>(bindingType);
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

		static private Binding GetBinding<TServiceInterfaceType>(HostBindingType bindingType)
		{
			object[] contractObjects = typeof(TServiceInterfaceType).GetCustomAttributes(typeof(ServiceContractAttribute), false);
			string serviceConfigurationName = null;
			if (contractObjects.Length > 0)
				serviceConfigurationName = ((ServiceContractAttribute)contractObjects[0]).ConfigurationName;

			if (String.IsNullOrEmpty(serviceConfigurationName))
				serviceConfigurationName = typeof(TServiceInterfaceType).Name;
							
			Binding binding;

			if (bindingType == HostBindingType.NetTcp)
			{
				string configurationName = String.Format("{0}_{1}", typeof(NetTcpBinding).Name, serviceConfigurationName);
				try
				{
					binding = new NetTcpBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, String.Format("unable to load binding configuration {0}; using default binding configuration", configurationName));
					binding = new NetTcpBinding();
				}

				((NetTcpBinding)binding).PortSharingEnabled = true;
			}
			else if (bindingType == HostBindingType.NamedPipes)
			{
				string configurationName = String.Format("{0}_{1}", typeof(NetNamedPipeBinding).Name, serviceConfigurationName);
				try
				{
					binding = new NetNamedPipeBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new NetNamedPipeBinding();
				}
			}
			else if (bindingType == HostBindingType.WSDualHttp)
			{
				string configurationName = String.Format("{0}_{1}", typeof(WSDualHttpBinding).Name, serviceConfigurationName);
				try
				{
					binding = new WSDualHttpBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new WSDualHttpBinding();
				}
			}
            else if (bindingType == HostBindingType.WSHttp)
			{
				string configurationName = String.Format("{0}_{1}", typeof(WSHttpBinding).Name, serviceConfigurationName);
				try
				{
					binding = new WSHttpBinding(configurationName);
				}
				catch
				{
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new WSHttpBinding();
				}
			}
            else
            {	
                string configurationName = String.Format("{0}_{1}", typeof(BasicHttpBinding).Name, serviceConfigurationName);
                try
                {
                    binding = new BasicHttpBinding(configurationName);
                }
                catch
                {
                    Platform.Log(LogLevel.Info, "unable to load binding configuration {0}; using default binding configuration", configurationName);
					binding = new BasicHttpBinding();
                }
            }

			return binding;
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
				endpoints.Add(new UriBuilder(String.Format("net.pipe://localhost/{0}", endpointName)).Uri);
			}

			endpoints.Add(new UriBuilder(String.Format("http://localhost:{0}/{1}", httpPort, endpointName)).Uri);

			return endpoints.ToArray();
		}

	}
}