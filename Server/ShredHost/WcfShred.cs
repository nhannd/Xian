using System;
using System.Collections.Generic;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;

namespace ClearCanvas.Server.ShredHost
{
    public abstract class WcfShred : ShredBase, IWcfShred
    {
		public WcfShred()
        {
            _serviceEndpointDescriptions = new Dictionary<string,ServiceEndpointDescription>();
        }

        public override object InitializeLifetimeService()
        {
            // I can't find any documentation yet, that says that returning null 
            // means that the lifetime of the object should not expire after a timeout
            // but the initial solution comes from this page: http://www.dotnet247.com/247reference/msgs/13/66416.aspx
            return null;
        }

		public void StartHttpHost<TServiceType, TServiceInterfaceType>(string name, string description)
		{
			if (_serviceEndpointDescriptions.ContainsKey(name))
				throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

			ServiceEndpointDescription sed = WcfHelper.StartHttpHost<TServiceType, TServiceInterfaceType>(name, description, this.HttpPort);
			_serviceEndpointDescriptions[name] = sed;
		}

		public void StartHttpDualHost<TServiceType, TServiceInterfaceType>(string name, string description)
		{
			if (_serviceEndpointDescriptions.ContainsKey(name))
				throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

			ServiceEndpointDescription sed = WcfHelper.StartHttpDualHost<TServiceType, TServiceInterfaceType>(name, description, this.HttpPort);
			_serviceEndpointDescriptions[name] = sed;
		}

		public void StartNetTcpHost<TServiceType, TServiceInterfaceType>(string name, string description)
		{
			if (_serviceEndpointDescriptions.ContainsKey(name))
				throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

			ServiceEndpointDescription sed = WcfHelper.StartNetTcpHost<TServiceType, TServiceInterfaceType>(name, description, this.TcpPort, this.HttpPort);
			_serviceEndpointDescriptions[name] = sed;
		}

		public void StartNetPipeHost<TServiceType, TServiceInterfaceType>(string name, string description)
		{
			if (_serviceEndpointDescriptions.ContainsKey(name))
				throw new Exception(String.Format("The service endpoint '{0}' already exists.", name));

			ServiceEndpointDescription sed = WcfHelper.StartNetPipeHost<TServiceType, TServiceInterfaceType>(name, description, this.HttpPort);
			_serviceEndpointDescriptions[name] = sed;
		}
		
		protected void StopHost(string name)
        {
            if (_serviceEndpointDescriptions.ContainsKey(name))
            {
                _serviceEndpointDescriptions[name].ServiceHost.Close();
                _serviceEndpointDescriptions.Remove(name);
            }
            else
            {
                // TODO: throw an exception, since a name of a service endpoint that is
                // passed in here that doesn't exist should be considered a programming error
            }
        }

		#region Private Members
        private Dictionary<string, ServiceEndpointDescription> _serviceEndpointDescriptions;
        #endregion

		#region IWcfShred Members
		
		private int _httpPort;
		private int _tcpPort;

		public int HttpPort
		{
			get { return _httpPort; }
			set	{ _httpPort = value; }
		}

		public int TcpPort
		{
			get { return _tcpPort; }
			set	{ _tcpPort = value; }
		}

		#endregion
	}
}
