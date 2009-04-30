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
using System.Net;
using System.Runtime.Serialization;
using System.Text;

namespace ClearCanvas.ImageServer.Common
{
    /// <summary>
    /// Encapsulates the address of the server on the network.
    /// </summary>
    [Serializable]
    public class ServerAddress
    {
        #region Private Members
        private string _hostName;
        private List<string> _ipAddresses = new List<string>();
        #endregion

        #region Static Properties
        /// <summary>
        /// Gets a instance of <see cref="ServerAddress"/> that represents the server on local machine.
        /// </summary>
        public static  ServerAddress Local
        {
            get
            {
                ServerAddress local = new ServerAddress();
                local.HostName = Dns.GetHostName();
                IPAddress[] ipAddresses = Dns.GetHostAddresses(local.HostName);
                foreach (IPAddress ip in ipAddresses)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        local.IPAddresses.Add(ip.ToString());
                    }
                    else if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetworkV6)
                    {
                        local.IPAddresses.Add(ip.ToString());
                    }
                    
                }
                return local;
            }
        }

        #endregion 

        #region Public Properties
        /// <summary>
        /// The host name of the machine where the server is running
        /// </summary>
        public string HostName
        {
            get { return _hostName; }
            set { _hostName = value; }
        }
        /// <summary>
        /// IP addresses of the machine where the server is running
        /// </summary>
        public List<string> IPAddresses
        {
            get { return _ipAddresses; }
            set { _ipAddresses = value; }
        }
        #endregion
    }
}
