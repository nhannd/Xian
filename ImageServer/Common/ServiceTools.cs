using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Common
{
    public static class ServiceTools
    {
        #region Static Private Members
        private static string _processorId;
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
        #endregion
    }
}
