#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Diagnostics;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Common
{
    public static class ServiceTools
    {
        #region Static Private Members
        private static string _processorId;
        private static ServerInformation _serverInformation;
        private static readonly object _serverInformationLock = new object();
        #endregion

        #region Static Properties
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

        

        /// <summary>
        /// Gets the <see cref="ServerInformation"/> entry in the database that represents the server on the local machine.
        /// </summary>
        /// <remarks>
        /// </remarks>
        public static ServerInformation localServerInformation
        {
            get
            {
                lock (_serverInformationLock)
                {
                    if (_serverInformation==null)
                    {
                        // Add or load the server information entry
                        
                        IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

                        using (IUpdateContext context = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
                        {
                            ServerAddress localserver = ServerAddress.Local;

                            // serialize it
                            XmlDocument xmlAddress = new XmlDocument();
                            XmlWriterSettings writerSettings = new XmlWriterSettings();
                            writerSettings.OmitXmlDeclaration = true;
                            writerSettings.Indent = true;
                            StringBuilder sb = new StringBuilder();
                            XmlWriter writer = XmlWriter.Create(sb, writerSettings);
                            XmlSerializerNamespaces xsn = new XmlSerializerNamespaces();
                            xsn.Add(string.Empty, string.Empty); // don't use namespace
                            XmlSerializer serializer = new XmlSerializer(typeof(ServerAddress));
                            serializer.Serialize(writer, localserver, xsn);
                            xmlAddress.LoadXml(sb.ToString());


                            _serverInformation = FindLocalServerInDB(context);

                            if (_serverInformation == null)
                            {
                                // Add the entry

                                ServerInformationUpdateColumns columns = new ServerInformationUpdateColumns();
                                // use the hostname as the server name.
                                columns.ServerName = localserver.HostName;
                                columns.Hostname = localserver.HostName;
                                columns.ExtInformation = xmlAddress;
                                columns.LastKnownTime = Platform.Time;

                                IServerInformationEntityBroker broker = context.GetBroker<IServerInformationEntityBroker>();
                                _serverInformation = broker.Insert(columns);

                            }
                            else
                            {
                                // Update it

                                ServerInformationUpdateColumns columns = new ServerInformationUpdateColumns();
                                columns.Hostname = localserver.HostName;
                                columns.ExtInformation = xmlAddress;
                                columns.LastKnownTime = Platform.Time;

                                IServerInformationEntityBroker broker = context.GetBroker<IServerInformationEntityBroker>();
                                broker.Update(_serverInformation.GetKey(), columns);
                            }

                            context.Commit();
                        }
                    }
                    
                }
                
                Debug.Assert(_serverInformation != null);
                return _serverInformation;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Returns the <see cref="ServerInformation"/> entry in the database corresponding to the server on the local machine
        /// </summary>
        /// <param name="context">The update context used for database connection</param>
        /// <returns>null if the entry doesn't exist.</returns>
        private static ServerInformation FindLocalServerInDB(IPersistenceContext context)
        {
            IServerInformationEntityBroker broker = context.GetBroker<IServerInformationEntityBroker>();
            ServerInformationSelectCriteria criteria = new ServerInformationSelectCriteria();

            // TODO consider caching the database key locally and use it for lookup instead.
            // This way if users modify the entries in the DB through the GUI, the service won't
            // insert another entry.
            criteria.Hostname.EqualTo(Dns.GetHostName());

            IList<ServerInformation> existingEntries = broker.Find(criteria);
            if (existingEntries == null || existingEntries.Count == 0)
            {
                return null;
            }
            else
            {
                return existingEntries[0];
            }
        }
        #endregion
    }
}
