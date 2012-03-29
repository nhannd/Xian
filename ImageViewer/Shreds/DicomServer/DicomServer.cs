#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using System.Net;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network.Scp;

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

			#endregion
		}

		#endregion

		private readonly IDicomServerContext _context;
		private readonly DicomScp<IDicomServerContext> _scp;

		private readonly string _aeTitle;
		private readonly string _host;
		private readonly int _port;
		private readonly string _interimStorageDirectory;

		public DicomServer(string aeTitle, string host, int port, string interimStorageDirectory)
		{
			_aeTitle = aeTitle;
			_host = host;
			_port = port;
			_interimStorageDirectory = interimStorageDirectory;

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
				if (!Directory.Exists(_interimStorageDirectory))
					Directory.CreateDirectory(_interimStorageDirectory);

				return _interimStorageDirectory;
			}	
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
