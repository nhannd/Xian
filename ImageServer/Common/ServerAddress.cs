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
