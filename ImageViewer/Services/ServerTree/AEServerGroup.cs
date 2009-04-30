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

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Services.ServerTree
{
	public class AEServerGroup
	{
        private List<IServerTreeNode> _servers;
        private string _name;
		private string _groupID;

		public AEServerGroup()
		{
        }

        public List<IServerTreeNode> Servers
        {
            get {
                if (_servers == null)
                    _servers = new List<IServerTreeNode>();
                return _servers; 
            }
            set { _servers = value; }
        }

        public string Name
		{
			get { return _name; }
			set { _name = value; }
		}
	
		public string GroupID
		{
			get { return _groupID; }
			set { _groupID = value; }
		}

		public bool IsLocalDatastore
		{
			get
			{
                if (_servers.Count != 1)
					return false;

                if (_servers[0].IsLocalDataStore)
					return true;
				else
					return false;
			}
		}

		public bool IsOnlyStreamingServers()
		{
			if (_servers.Count == 0)
				return false;

			foreach (IServerTreeNode node in _servers)
			{
				if (node.IsServer)
				{
					Server server = node as Server;

					if (!server.IsStreaming)
						return false;
				}
			}

			return true;
		}
    }
}
