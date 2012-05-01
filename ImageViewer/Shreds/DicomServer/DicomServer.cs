#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.IO;
using System.Net;
using ClearCanvas.Dicom.Network.Scp;
using ClearCanvas.ImageViewer.Common.DicomServer;
using ClearCanvas.ImageViewer.Common.StudyManagement;

namespace ClearCanvas.ImageViewer.Shreds.DicomServer
{
	internal class DicomServer
	{
		#region Context

		public class DicomServerContext : IDicomServerContext
		{
			private readonly DicomServer _server;
		    
			internal DicomServerContext(DicomServer server)
			{
				_server = server;
			}

			#region IDicomServerContext Members

			public string AETitle
			{
				get { return _server.AETitle; }
			}

			public string Host
			{
				get { return _server.Host; }	
			}

			public int Port
			{
				get { return _server.Port; }	
			}

		    public StorageConfiguration StorageConfiguration
		    {
                get { return _server.StorageConfiguration; }
		    }

			#endregion
		}

		#endregion

		private readonly IDicomServerContext _context;
		private readonly DicomScp<IDicomServerContext> _scp;

		private readonly string _aeTitle;
		private readonly string _host;
		private readonly int _port;
	    private readonly StorageConfiguration _storageConfiguration;

		public DicomServer(DicomServerConfiguration serverConfiguration, StorageConfiguration storageConfiguration)
		{
			_aeTitle = serverConfiguration.AETitle;
            _host = serverConfiguration.HostName;
			_port = serverConfiguration.Port;
			_storageConfiguration = storageConfiguration;

			_context = new DicomServerContext(this);
			_scp = new DicomScp<IDicomServerContext>(_context, AssociationVerifier.VerifyAssociation);
		}

		#region Public Properties

		public string AETitle
		{
			get { return _aeTitle; }
		}

		public string Host
		{
			get { return _host; }	
		}

		public int Port
		{
			get { return _port; }
		}

		public string InterimStorageDirectory
		{
			get
			{
                if (!Directory.Exists(_storageConfiguration.FileStoreDirectory))
                    Directory.CreateDirectory(_storageConfiguration.FileStoreDirectory);

                return _storageConfiguration.FileStoreDirectory;
			}	
		}

	    public StorageConfiguration StorageConfiguration
	    {
	        get { return _storageConfiguration; }
	    }

		#endregion

		#region Server Startup/Shutdown

		public void Start()
		{
			IPHostEntry entry = Dns.GetHostEntry(_host);
			IPAddress address = entry.AddressList[0];
			IPAddress localhost = Dns.GetHostEntry("localhost").AddressList[0];
			if (localhost.Equals(address))
				address = IPAddress.Any;

			_scp.AeTitle = _aeTitle;
			_scp.ListenPort = _port;
			_scp.Start(address);
		}

		public void Stop()
		{
			_scp.Stop();
		}

		#endregion
	}
}
