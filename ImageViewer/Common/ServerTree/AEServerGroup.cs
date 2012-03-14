#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Common.ServerTree
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
			get
			{
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

		public bool HasAnyNonStreamingServers()
		{
			if (_servers.Count == 0)
				return false;

			foreach (IServerTreeNode node in _servers)
			{
				if (node.IsServer)
				{
					Server server = node as Server;
					if (server == null || !server.IsStreaming)
						return true;
				}
			}

			return false;
		}

		public bool HasAnyStreamingServers()
		{
			if (_servers.Count == 0)
				return false;

			foreach (IServerTreeNode node in _servers)
			{
				if (node.IsServer)
				{
					Server server = node as Server;
					if (server != null && server.IsStreaming)
						return true;
				}
			}

			return false;
		}

		public bool IsOnlyNonStreamingServers()
		{
			if (_servers.Count == 0)
				return false;

			foreach (IServerTreeNode node in _servers)
			{
				if (node.IsServer)
				{
					Server server = node as Server;
					if (server != null && server.IsStreaming)
						return false;
				}
			}

			return true;
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
					if (server == null || !server.IsStreaming)
						return false;
				}
			}

			return true;
		}
	}
}