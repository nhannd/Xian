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
        /// <summary>
        /// Constructor
        /// </summary>
        public DicomServerEditComponent(ServerTree dicomServerTree)
        {
            _serverTree = dicomServerTree;
            if (_serverTree.CurrentNode.IsServer)
            {
				Server server = _serverTree.CurrentNode as Server;
				_serverName = server.Name;
				_serverLocation = server.Location;
				_serverAE = server.AETitle;
				_serverHost = server.Host;
				_serverPort = server.Port.ToString();
            }
            else
            {
                _serverName = "";
                _serverLocation = "";
                _serverAE = "";
                _serverHost = "";
                _serverPort = "";
            }
        }

        public void Accept()
        {
			if (!IsServerPropertyValid() || !this.Modified)
				return;

			Server newServer = new Server(_serverName, _serverLocation, _serverHost, _serverAE, int.Parse(_serverPort));

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

        private bool IsServerPropertyEmpty()
        {
            if (_serverName == null || _serverName.Equals("") || _serverAE == null || _serverAE.Equals("")
                || _serverHost == null || _serverHost.Equals("") || _serverPort == null || _serverPort.Equals(""))
            {
                return false;
            }

            return true;
        }

        private bool IsServerPropertyValid()
        {
            int port = -1;
            try
            {
                port = int.Parse(_serverPort);
                if (_serverTree.CurrentNode.IsServer
                        && _serverName.Equals(_serverTree.CurrentNode.Name)
                        && _serverAE.Equals(((Server)_serverTree.CurrentNode).AETitle)
						&& _serverHost.Equals(((Server)_serverTree.CurrentNode).Host)
						&& port == ((Server)_serverTree.CurrentNode).Port)
                    return true;
            }
            catch
            {
                this.Modified = false;
                StringBuilder msgText = new StringBuilder();
				msgText.AppendFormat(SR.MessageServerPortMustBePositiveInteger);
                throw new DicomServerException(msgText.ToString());
            }

            if (port <= 0)
            {
                this.Modified = false;
				throw new DicomServerException(SR.MessageServerPortMustBePositiveInteger);
            }

        	bool isConflicted;
            string conflictingServerPath;
        	if (_serverTree.CurrentNode.IsServer)
				isConflicted = _serverTree.CanEditCurrentServer(_serverName, _serverAE, _serverHost, int.Parse(_serverPort), out conflictingServerPath);
			else
				isConflicted = _serverTree.CanAddServerToCurrentGroup(_serverName, _serverAE, _serverHost, int.Parse(_serverPort), out conflictingServerPath);

        	if (isConflicted)
            {
                this.Modified = false;
                StringBuilder msgText = new StringBuilder();
				msgText.AppendFormat(SR.FormatServerConflict, conflictingServerPath);
                throw new DicomServerException(msgText.ToString());
            }

            return true;
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public bool FieldReadonly
        {
            get 
            {
				return _serverTree.CurrentNode.IsLocalDataStore;
            }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        #region Fields

        private ServerTree _serverTree;
        private string _serverName = "";
        private string _serverLocation = "";
        private string _serverAE = "";
        private string _serverHost = "";
        private string _serverPort = "";

		public string ServerName
		{
			get { return _serverName; }
			set
			{
				_serverName = value;
				this.Modified = IsServerPropertyEmpty();
			}
		}
		
		public string ServerLocation
        {
            get { return _serverLocation; }
            set { 
                _serverLocation = value;
                this.Modified = true;
            }
        }

        public string ServerAE
        {
            get { return _serverAE; }
            set { 
                _serverAE = value;
                this.Modified = IsServerPropertyEmpty();
            }
        }

        public string ServerHost
        {
            get { return _serverHost; }
            set { 
                _serverHost = value;
                this.Modified = IsServerPropertyEmpty();
            }
        }

        public string ServerPort
        {
            get { return _serverPort; }
            set { 
                _serverPort = value;
                this.Modified = IsServerPropertyEmpty();
            }
        }

        #endregion

    }

    public class DicomServerException : Exception
    {
        public DicomServerException(string message) : base(message) { }
        public DicomServerException(string message, Exception inner) : base(message, inner) { }
    }

}
