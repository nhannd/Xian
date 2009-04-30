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
using System.Diagnostics;
using System.Net;
using System.Threading;
using ClearCanvas.Common;
namespace ClearCanvas.ImageServer.Common
{
    public static class ServiceTools
    {
        #region Static Private Members

        private static string _hostId;
        private static string _serverInstanceId;
        private static string _processorId;
        #endregion

        #region Static Properties

        /// <summary>
        /// Returns a string that can be used to identify the host machine where the server is running
        /// </summary>
        public static string HostId
        {
            get
            {
                if (String.IsNullOrEmpty(_hostId))
                {
                    String strHostName = Dns.GetHostName();
                    if (String.IsNullOrEmpty(strHostName) == false)
                        _hostId = strHostName;
                    else
                    {
                        // Find host by name
                        IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

                        // Enumerate IP addresses, pick an IPv4 address first
                        foreach (IPAddress ipaddress in iphostentry.AddressList)
                        {
                            if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                _hostId = ipaddress.ToString();
                                break;
                            }
                        }
                    }
                }

                return _hostId;
            }
            
        }


        public static string ServerInstanceId
        {
            get
            {
                if (String.IsNullOrEmpty(_serverInstanceId))
                {
                    _serverInstanceId = String.Format("Host={0}/Pid={1}", HostId, Process.GetCurrentProcess().Id);
                }

                return _serverInstanceId;
            }
        }

        /// <summary>
        /// A string representing the ID of the work queue processor.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This ID is used to reset the work queue items.
        /// </para>
        /// <para>
        /// For the time being, the machine ID is tied to the IP address. Assumimg the server
        /// will be installed on a machine with DHCP disabled or if the DNS server always assign
        /// the same IP for the machine, this will work fine.
        /// </para>
        /// <para>
        /// Because of this implemenation, all instances of WorkQueueProcessor will have the same ID.
        /// </para>
        /// </remarks>
        public static string ProcessorId
        {
            get
            {

                if (_processorId == null)
                {
                    try
                    {
                        String strHostName = Dns.GetHostName();

                        // Find host by name
                        IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

                        // Enumerate IP addresses, pick an IPv4 address first
                        foreach (IPAddress ipaddress in iphostentry.AddressList)
                        {
                            if (ipaddress.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            {
                                _processorId = ipaddress.ToString();
                                break;
                            }
                        }
                        if (_processorId == null)
                        {
                            foreach (IPAddress ipaddress in iphostentry.AddressList)
                            {
                                _processorId = ipaddress.ToString();
                                break;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Cannot resolve hostname into IP address");
                    }
                }

                if (_processorId == null)
                {
                    Platform.Log(LogLevel.Warn, "Could not determine hostname or IP address of the local machine. Work Queue Processor ID is set to Unknown");
                    _processorId = "Unknown";

                }

                return _processorId;
            }
        }
        #endregion

    }
}
