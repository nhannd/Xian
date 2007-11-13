#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.ImageViewer.Services.ServerTree;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
	/// <summary>
	/// Extension point for views onto <see cref="DicomServerEditComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DicomServerEditComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DicomServerEditComponent class
	/// </summary>
	[AssociateView(typeof(DicomServerEditComponentViewExtensionPoint))]
	public class DicomServerEditComponent : ApplicationComponent
	{
		public static readonly int MinimumPort = 1;
		public static readonly int DefaultPort = 104;
		public static readonly int MaximumPort = 65535;

		#region Private Fields

		private readonly ServerTree _serverTree;
		private string _serverName;
		private string _serverLocation;
		private string _serverAE;
		private string _serverHost;
		private int _serverPort;

		#endregion

		public DicomServerEditComponent(ServerTree dicomServerTree)
		{
			_serverTree = dicomServerTree;
			if (_serverTree.CurrentNode.IsServer)
			{
				Server server = (Server)_serverTree.CurrentNode;
				_serverName = server.Name;
				_serverLocation = server.Location;
				_serverAE = server.AETitle;
				_serverHost = server.Host;
				_serverPort = server.Port;
			}
			else
			{
				_serverName = "";
				_serverLocation = "";
				_serverAE = "";
				_serverHost = "";
				_serverPort = DefaultPort;
			}
		}

		#region Public Properties

		public string ServerName
		{
			get { return _serverName; }
			set
			{
				if (_serverName == value)
					return;

				_serverName = value;
				UpdateAcceptEnabled();
				NotifyPropertyChanged("ServerName");
			}
		}

		public string ServerLocation
		{
			get { return _serverLocation; }
			set
			{
				if (_serverLocation == value)
					return;
				
				_serverLocation = value;
				UpdateAcceptEnabled();
				NotifyPropertyChanged("ServerLocation");
			}
		}

		public string ServerAE
		{
			get { return _serverAE; }
			set
			{
				if (_serverAE == value)
					return;

				_serverAE = value;
				UpdateAcceptEnabled();
				NotifyPropertyChanged("ServerAE");
			}
		}

		public string ServerHost
		{
			get { return _serverHost; }
			set
			{
				if (_serverHost == value)
					return;
				
				_serverHost = value;
				UpdateAcceptEnabled();
				NotifyPropertyChanged("ServerHost");
			}
		}

		public int ServerPort
		{
			get { return _serverPort; }
			set
			{
				if (_serverPort == value)
					return;
				
				_serverPort = value;
				UpdateAcceptEnabled();
				NotifyPropertyChanged("ServerPort");
			}
		}

		#endregion

		private void UpdateAcceptEnabled()
		{
			if (String.IsNullOrEmpty(_serverHost) || String.IsNullOrEmpty(_serverAE) || String.IsNullOrEmpty(_serverName))
				this.AcceptEnabled = false;
			else 
				this.AcceptEnabled = true;
		}

		public void Accept()
		{
			if (!IsServerPropertyValid())
			{
				this.AcceptEnabled = false;
				return;
			}

			Server newServer = new Server(_serverName, _serverLocation, _serverHost, _serverAE, _serverPort);

			// edit current server
			if (_serverTree.CurrentNode.IsServer)
			{
				_serverTree.ReplaceDicomServerInCurrentGroup(newServer);
			}
			// add new server
			else if (_serverTree.CurrentNode.IsServerGroup)
			{
				((ServerGroup)_serverTree.CurrentNode).AddChild(newServer);
				_serverTree.CurrentNode = newServer;
				_serverTree.Save();
				_serverTree.FireServerTreeUpdatedEvent();
			}

			this.ExitCode = ApplicationComponentExitCode.Accepted;
			Host.Exit();
		}

		public void Cancel()
		{
			Host.Exit();
		}

		private string VerifyServerProperties()
		{
			if (String.IsNullOrEmpty(_serverName))
				return SR.MessageServerNameCannotBeEmpty;
			if (String.IsNullOrEmpty(_serverHost))
				return SR.MessageHostNameCannotBeEmpty;
			if (String.IsNullOrEmpty(_serverAE) || _serverAE.Length > 16 || _serverAE.Contains(" "))
				return SR.MessageServerAEInvalid;
			if (_serverPort < MinimumPort || _serverPort > MaximumPort)
				return SR.MessagePortInvalid;

			return null;
		}

		private bool IsServerPropertyValid()
		{
			string verifyMessage = VerifyServerProperties();
			if (verifyMessage != null)
			{
				this.Host.DesktopWindow.ShowMessageBox(verifyMessage, MessageBoxActions.Ok);
				return false;
			}

			bool isConflicted;
			string conflictingServerPath;
			if (_serverTree.CurrentNode.IsServer)
				isConflicted = _serverTree.CanEditCurrentServer(_serverName, _serverAE, _serverHost, _serverPort, out conflictingServerPath);
			else
				isConflicted = _serverTree.CanAddServerToCurrentGroup(_serverName, _serverAE, _serverHost, _serverPort, out conflictingServerPath);

			if (isConflicted)
			{
				this.Host.DesktopWindow.ShowMessageBox(String.Format(SR.FormatServerConflict, conflictingServerPath), MessageBoxActions.Ok);
				return false;
			}

			return true;
		}

		public bool AcceptEnabled
		{
			get { return this.Modified; }
			set
			{
				if (value == this.Modified)
					return;

				this.Modified = value;
				NotifyPropertyChanged("AcceptEnabled");
			}
		}

		public bool FieldReadonly
		{
			get { return _serverTree.CurrentNode.IsLocalDataStore; }
		}
	}
}
