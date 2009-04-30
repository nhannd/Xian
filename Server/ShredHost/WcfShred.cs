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
using ClearCanvas.Common;
using ClearCanvas.Common.Shreds;

namespace ClearCanvas.Server.ShredHost
{
    public abstract class WcfShred : Shred, IWcfShred
    {
        public WcfShred()
        {
            _serviceEndpointDescriptions = new Dictionary<string, ServiceEndpointDescription>();
        }


        public override object InitializeLifetimeService()
        {
            // I can't find any documentation yet, that says that returning null 
            // means that the lifetime of the object should not expire after a timeout
            // but the initial solution comes from this page: http://www.dotnet247.com/247reference/msgs/13/66416.aspx
            return null;
        }


        public ServiceEndpointDescription StartHttpHost<TServiceType, TServiceInterfaceType>(string name, string description)
        {
            Platform.Log(LogLevel.Info, "Starting WCF Shred {0}...", name);

            if (_serviceEndpointDescriptions.ContainsKey(name))
                throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

            ServiceEndpointDescription sed =
                WcfHelper.StartHttpHost<TServiceType, TServiceInterfaceType>(name, description, SharedHttpPort);
            _serviceEndpointDescriptions[name] = sed;

            Platform.Log(LogLevel.Info, "WCF Shred {0} is listening at {1}.", name, sed.ServiceHost.BaseAddresses[0]);

            return sed;
        }

        public ServiceEndpointDescription StartBasicHttpHost<TServiceType, TServiceInterfaceType>(string name, string description)
        {
            Platform.Log(LogLevel.Info, "Starting WCF Shred {0}...", name);

            if (_serviceEndpointDescriptions.ContainsKey(name))
                throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

            ServiceEndpointDescription sed =
                WcfHelper.StartBasicHttpHost<TServiceType, TServiceInterfaceType>(name, description, SharedHttpPort);
            _serviceEndpointDescriptions[name] = sed;

            Platform.Log(LogLevel.Info, "WCF Shred {0} is listening at {1}.", name, sed.ServiceHost.BaseAddresses[0]);

            return sed;
        }

        public ServiceEndpointDescription StartHttpDualHost<TServiceType, TServiceInterfaceType>(string name,
                                                                                                 string description)
        {
            Platform.Log(LogLevel.Info, "Starting WCF Shred {0}...", name);

            if (_serviceEndpointDescriptions.ContainsKey(name))
                throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

            ServiceEndpointDescription sed =
                WcfHelper.StartHttpDualHost<TServiceType, TServiceInterfaceType>(name, description, SharedHttpPort);
            _serviceEndpointDescriptions[name] = sed;

            Platform.Log(LogLevel.Info, "WCF Shred {0} is listening at {1}.", name, sed.ServiceHost.BaseAddresses[0]);

            return sed;
        }

        public ServiceEndpointDescription StartNetTcpHost<TServiceType, TServiceInterfaceType>(string name,
                                                                                               string description)
        {
            Platform.Log(LogLevel.Info, "Starting WCF Shred {0}...", name);

            if (_serviceEndpointDescriptions.ContainsKey(name))
                throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

            ServiceEndpointDescription sed =
                WcfHelper.StartNetTcpHost<TServiceType, TServiceInterfaceType>(name, description, SharedTcpPort, SharedHttpPort);
            _serviceEndpointDescriptions[name] = sed;

            Platform.Log(LogLevel.Info, "WCF Shred {0}is listening at {1}.", name, sed.ServiceHost.BaseAddresses[0]);


            return sed;
        }

        public ServiceEndpointDescription StartNetPipeHost<TServiceType, TServiceInterfaceType>(string name,
                                                                                                string description)
        {
            Platform.Log(LogLevel.Info, "Starting WCF Shred {0}...", name);

            if (_serviceEndpointDescriptions.ContainsKey(name))
                throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

            ServiceEndpointDescription sed =
                WcfHelper.StartNetPipeHost<TServiceType, TServiceInterfaceType>(name, description, SharedHttpPort);
            _serviceEndpointDescriptions[name] = sed;


            Platform.Log(LogLevel.Info, "WCF Shred {0} is listening at {1}.", name, sed.ServiceHost.BaseAddresses[0]);

            return sed;
        }

        protected void StopHost(string name)
        {
            Platform.Log(LogLevel.Info, "Stopping WCF Shred {0}...", name);

            if (_serviceEndpointDescriptions.ContainsKey(name))
            {
                _serviceEndpointDescriptions[name].ServiceHost.Close();
                _serviceEndpointDescriptions.Remove(name);

                Platform.Log(LogLevel.Info, "WCF Shred {0} Stopped", name);
            }
            else
            {
                // TODO: throw an exception, since a name of a service endpoint that is
                // passed in here that doesn't exist should be considered a programming error
                Platform.Log(LogLevel.Debug, "Attempt to stop WCF Shred {0} failed: shred doesn't exist.", name);
            }
        }

        #region Private Members

        private Dictionary<string, ServiceEndpointDescription> _serviceEndpointDescriptions;

        #endregion

        #region IWcfShred Members

        private int _httpPort;
        private int _tcpPort;

        public int SharedHttpPort
        {
            get { return _httpPort; }
            set { _httpPort = value; }
        }

        public int SharedTcpPort
        {
            get { return _tcpPort; }
            set { _tcpPort = value; }
        }

        #endregion
    }
}
