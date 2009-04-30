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

			public string InterimStorageDirectory
			{
				get { return _server.InterimStorageDirectory; }	
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
