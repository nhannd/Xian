#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Services.DicomServer;

namespace ClearCanvas.ImageViewer.Services.ServerTree
{
    public class ServerTree
	{
		internal static readonly string MyServersXmlFile = "DicomAEServers.xml";
		internal static readonly string MyDataStoreTitle = SR.MyDataStoreTitle;
		internal static readonly string MyServersTitle = SR.MyServersTitle;
		internal static readonly string RootNodeName = "Root";
		
		#region Private fields

		private ServerTreeRoot _rootNode;
		private IServerTreeNode _currentNode;
		private event EventHandler _serverTreeUpdated;

    	private static readonly XmlSerializer _serializer;

		#endregion

		static ServerTree()
		{
			DicomServerConfigurationHelper.Changed += new EventHandler(OnServerConfigurationChanged);

			_serializer = new XmlSerializer(typeof(ServerTreeRoot), new Type[] { 
                    typeof(ServerGroup),
                    typeof(Server),
                    typeof(List<ServerGroup>),
                    typeof(List<Server>)
                });
		}

		private static void OnServerConfigurationChanged(object sender, EventArgs e)
		{
			try
			{
				ServerTree serverTree = new ServerTree();
				if (serverTree.RootNode.LocalDataStoreNode.OfflineAE != DicomServerConfigurationHelper.AETitle)
				{
					serverTree.RootNode.LocalDataStoreNode.OfflineAE = DicomServerConfigurationHelper.AETitle;
					serverTree.Save();
				}
			}
			catch(Exception ex)
			{
				Platform.Log(LogLevel.Error, ex, "Failed to save the server tree.");
			}
		}

    	public ServerTree()
    	{
    		LoadServers();
    		CurrentNode = _rootNode.LocalDataStoreNode;
    	}

    	#region Public Properties / Events

    	public IServerTreeNode CurrentNode
    	{
    		get { return _currentNode; }
    		set { _currentNode = value; }
    	}

    	public ServerTreeRoot RootNode
    	{
    		get { return _rootNode; }
    	}

    	public event EventHandler ServerTreeUpdated
    	{
    		add { _serverTreeUpdated += value; }
    		remove { _serverTreeUpdated -= value; }
    	}

    	#endregion

		#region Public Methods

		public static string GetClientAETitle()
		{
			ServerTree serverTree = new ServerTree();
			return serverTree.RootNode.LocalDataStoreNode.GetClientAETitle();
		}

    	public void FireServerTreeUpdatedEvent()
        {
            EventsHelper.Fire(_serverTreeUpdated, this, EventArgs.Empty);
        }

		public bool CanMoveOrAdd(IServerTreeNode destinationNode, IServerTreeNode addMoveNode)
		{
			if (destinationNode.IsServer)
				return false;

			if (destinationNode == addMoveNode)
				return false;

			if (addMoveNode == this.RootNode.ServerGroupNode || addMoveNode == this.RootNode.LocalDataStoreNode)
				return false;

			if (addMoveNode.IsServer)
			{
				Server server = (Server)addMoveNode;
				string conflictingPath;
				if (IsConflictingServerInGroup((ServerGroup)destinationNode, server.Name, false, server.AETitle, server.Host, server.Port, out conflictingPath))
					return false;
			}
			else if (addMoveNode is ServerGroup)
			{
				string conflictingPath;
				if (IsConflictingServerGroupInGroup((ServerGroup)destinationNode, addMoveNode.Name, false, out conflictingPath))
					return false;

				if (addMoveNode.ParentPath == destinationNode.Path)
					return false;

				// if the node that's being added is actually a direct parent of
				// the destination node, then it's not possible for it to be added as a child
				// direct parent:
				//      destination - ./a/b/c/destination
				//      direct parents - ./a/b/c; ./a/b; ./a
				//      non-d parents - ./a/b/d; ./a/b/c/e
				// thus, if the proposed node's path is NOT wholly contained in the destination node's
				// path, then it's okay to add the proposed node
				return (destinationNode.Path.IndexOf(addMoveNode.Path) == -1);
			}

			return true;
		}
		
		public bool CanAddServerToCurrentGroup(string serverName, string AETitle, string serverHost, int port, out string conflictingServerPath)
        {
			if (!CurrentNode.IsServerGroup)
			{
				conflictingServerPath = "";
				return false;
			}

			return IsConflictingServerInGroup((ServerGroup)this.CurrentNode, serverName, false, AETitle, serverHost, port, out conflictingServerPath);
        }

		public bool CanEditCurrentServer(string serverName, string AETitle, string serverHost, int port, out string conflictingServerPath)
		{
			if (!CurrentNode.IsServer)
			{
				conflictingServerPath = "";
				return false;
			}

			return IsConflictingServerInGroup(FindParentGroup(this.CurrentNode), serverName, true, AETitle, serverHost, port, out conflictingServerPath);
		}

		public bool CanAddGroupToCurrentGroup(string newGroupName, out string conflictingGroupPath)
        {
            if (!CurrentNode.IsServerGroup)
            {
                conflictingGroupPath = "";
                return false;
            }

			return IsConflictingServerGroupInGroup((ServerGroup)CurrentNode, newGroupName, false, out conflictingGroupPath);
		}

		public bool CanEditCurrentGroup(string newGroupName, out string conflictingGroupPath)
		{
			if (!CurrentNode.IsServerGroup)
			{
				conflictingGroupPath = "";
				return false;
			}

			return IsConflictingServerGroupInGroup(FindParentGroup(CurrentNode), newGroupName, true, out conflictingGroupPath);
		}
		
		public void DeleteServerGroup()
        {
			if (!CurrentNode.IsServerGroup)
				return;
			
			ServerGroup parentGroup = FindParentGroup(CurrentNode);
            if (null == parentGroup)
                return;	

            for (int i = 0; i < parentGroup.ChildGroups.Count; ++i)
            {
                if (parentGroup.ChildGroups[i].Name == CurrentNode.Name)
                {
                    parentGroup.ChildGroups.RemoveAt(i);
                    CurrentNode = parentGroup;
                    Save();
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

        public void DeleteDicomServer()
        {
			if (!CurrentNode.IsServer)
				return;

            ServerGroup parentGroup = FindParentGroup(CurrentNode);
            if (parentGroup == null)
                return;

            for (int i = 0; i < parentGroup.ChildServers.Count; i++)
            {
                if (parentGroup.ChildServers[i].Name == CurrentNode.Name)
                {
                    parentGroup.ChildServers.RemoveAt(i);
                    CurrentNode = parentGroup;
                    Save();
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

		public void ReplaceDicomServerInCurrentGroup(Server newServer)
		{
			if (!CurrentNode.IsServer)
				return;

			ServerGroup serverGroup = FindParentGroup(CurrentNode);
			if (serverGroup == null)
				return;

			for (int i = 0; i < serverGroup.ChildServers.Count; i++)
			{
				if (serverGroup.ChildServers[i].Name == CurrentNode.Name)
				{
					serverGroup.ChildServers[i] = newServer;
					newServer.ChangeParentPath(serverGroup.Path);
					CurrentNode = newServer;
					Save();
					FireServerTreeUpdatedEvent();
					break;
				}
			}
		}

		public void Save()
		{
			Stream fStream = new FileStream(MyServersXmlFile, FileMode.Create, FileAccess.Write, FileShare.Read);
			_serializer.Serialize(fStream, _rootNode);
            fStream.Close();
            return;
        }

        public List<IServerTreeNode> FindChildServers(ServerGroup serverGroup)
        {
            List<IServerTreeNode> listOfChildrenServers = new List<IServerTreeNode>();
            FindChildServers(serverGroup, listOfChildrenServers);
            return listOfChildrenServers;
		}

		public List<IServerTreeNode> FindDefaultServers(ServerGroup serverGroup)
		{
			return FindChildServers(serverGroup).FindAll(delegate(IServerTreeNode node) { return node.IsDefault; });
		}

    	#endregion

		#region Private methods

		private void LoadServers()
        {
            _rootNode = new ServerTreeRoot();

			if (File.Exists(MyServersXmlFile))
            {
				Stream fStream = File.OpenRead(MyServersXmlFile);

                using (fStream)
                {
					ServerTreeRoot serverTreeRoot = (ServerTreeRoot)_serializer.Deserialize(fStream);
					_rootNode = serverTreeRoot;
                }
            }
            else
            {
                // create default entries and save them to disk
                _rootNode.ServerGroupNode = new ServerGroup(MyServersTitle);
				_rootNode.ServerGroupNode.ChildGroups.Add(new ServerGroup(SR.ExampleGroup));
                _rootNode.ServerGroupNode.ChildServers.Add(
					new Server(SR.ExampleServer, "", "localhost", "SAMPLE", 104, false, 50221, 1000));
            	Save();
            }

			_rootNode.InitializeChildPaths();
        }

		private ServerGroup FindParentGroup(IServerTreeNode node)
        {
            return FindGroupNode(_rootNode.ServerGroupNode, node.ParentPath);
        }

		private ServerGroup FindGroupNode(ServerGroup startSearchNode, string path)
        {
            if (!startSearchNode.IsServerGroup)
                return null;

            if (startSearchNode.Path == path)
                return startSearchNode;

			foreach (ServerGroup childrenServerGroup in startSearchNode.ChildGroups)
            {
				ServerGroup foundNode = FindGroupNode(childrenServerGroup, path);
                if (null != foundNode)
                    return foundNode;
            }

            return null;
        }

		private bool IsConflictingServerInGroup(ServerGroup serverGroup, string toFindServerName, bool excludeCurrentNode, string toFindServerAE, string toFindServerHost, int toFindServerPort, out string conflictingServerPath)
        {
            foreach (Server server in serverGroup.ChildServers)
            {
				if (excludeCurrentNode && server == this.CurrentNode)
					continue;

				if (String.Compare(server.Name, toFindServerName, true) == 0 || 
						(server.AETitle == toFindServerAE &&
						String.Compare(server.Host, toFindServerHost, true) == 0 &&
						server.Port == toFindServerPort))
				{
					conflictingServerPath = server.Path;
                    return true;
                }
            }

            conflictingServerPath = "";
            return false;
        }
		private bool IsConflictingServerGroupInGroup(ServerGroup searchSite, string toFindServerGroupName, bool excludeCurrentNode, out string conflictingGroupPath)
        {
            foreach (ServerGroup serverGroup in searchSite.ChildGroups)
            {
				if (excludeCurrentNode && serverGroup == this.CurrentNode)
					continue;

				if (String.Compare(serverGroup.Name, toFindServerGroupName, true) == 0)
                {
                    conflictingGroupPath = serverGroup.Path;
                    return true;
                }
            }

            conflictingGroupPath = "";
            return false;
        }
        private void FindChildServers(ServerGroup serverGroup, List<IServerTreeNode> list)
        {
            foreach (ServerGroup group in serverGroup.ChildGroups)
            {
                FindChildServers(group, list);
            }

            foreach (Server server in serverGroup.ChildServers)
            {
                list.Add(server);
            }
        }

        #endregion
	}

    public interface IServerTreeNode
    {
		bool IsDefault { get; }
        bool IsLocalDataStore { get; }
        bool IsServer { get; }
        bool IsServerGroup { get; }
        bool IsRoot { get; }
        string ParentPath { get; }
        string Path { get; }
        string Name { get; }
    }

    [Serializable]
    public class ServerTreeRoot : IServerTreeNode
	{
		#region Private Fields
		private LocalDataStore _localDataStoreNode;
		#endregion

		public ServerTreeRoot()
        {
        }

		public LocalDataStore LocalDataStoreNode
    	{
			get
			{
				if (_localDataStoreNode == null)
					_localDataStoreNode = new LocalDataStore();

				return _localDataStoreNode;
			}
			set
			{
				_localDataStoreNode = value;
			}
    	}

		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public ServerGroup ServerGroupNode;

        #region IServerTreeNode Members

        public bool IsLocalDataStore
        {
            get { return false; }
        }

    	public bool IsDefault
    	{
			get { return false; }
    	}

        public bool IsServer
        {
            get { return false; }
        }

        public bool IsServerGroup
        {
            get { return false; }
        }

        public string Path
        {
            get { return "./"; }
        }

        public bool IsRoot
        {
            get { return true; }
        }

        public string Name
        {
            get { return ServerTree.RootNodeName; }
        }

        public string ParentPath
        {
            get { return ""; }
        }

        #endregion

		internal void InitializeChildPaths()
		{
			this.LocalDataStoreNode.ChangeParentPath(Path);
			this.ServerGroupNode.ChangeParentPath(Path);
		}
	}

    [Serializable]
    public class ServerGroup : IServerTreeNode
	{
		#region Private Fields
		private string _parentPath;
		private string _path;
    	private string _nameOfGroup;
		#endregion
		protected ServerGroup()
		{
		}

		public ServerGroup(string name)
        {
			_nameOfGroup = name;
            ChildGroups = new List<ServerGroup>();
            ChildServers = new List<Server>();
		}

		#region Public Properties/Fields

		public String NameOfGroup
    	{
			get { return _nameOfGroup; }
			set
			{
				_nameOfGroup = value;
				ChangeParentPath(_parentPath);
			}
    	}

		/// <summary>
		/// Public field for serialization only; use the <see cref="AddChild"/> method instead.
		/// </summary>
		public List<ServerGroup> ChildGroups;
		/// <summary>
		/// Public field for serialization only; use the <see cref="AddChild"/> method instead.
		/// </summary>
		public List<Server> ChildServers;
		
		#endregion

		#region IServerTreeNode Members

    	public bool IsDefault
    	{
			get { return false; }
    	}

		public bool IsLocalDataStore
        {
            get { return false; }
        }

        public bool IsServer
        {
            get { return false; }
        }

        public bool IsServerGroup
        {
            get { return true; }
        }

        public string Path
        {
			get { return _path; }
        }

        public bool IsRoot
        {
            get { return false; }
        }

        public string Name
        {
            get { return NameOfGroup; }
        }

        public string ParentPath
        {
			get { return _parentPath; }
        }

        #endregion

    	internal void ChangeParentPath(string newParentPath)
    	{
    		// change the parent path of myself and all my children
    		_parentPath = newParentPath ?? "";
    		_path = _parentPath + this.NameOfGroup + "/";

    		foreach (Server server in ChildServers)
    			server.ChangeParentPath(_path);

    		foreach (ServerGroup serverGroup in ChildGroups)
    			serverGroup.ChangeParentPath(_path);
    	}

    	public void AddChild(IServerTreeNode child)
    	{
    		if (child.IsServer)
    		{
    			Server childServer = (Server) child;
    			ChildServers.Add(childServer);
    			childServer.ChangeParentPath(_path);
    		}
    		else if (child.IsServerGroup)
    		{
    			ServerGroup childServerGroup = (ServerGroup)child;
    			ChildGroups.Add(childServerGroup);
    			childServerGroup.ChangeParentPath(_path);
    		}
    	}

    	public override string ToString()
    	{
			return _path;
    	}
    }

    [Serializable]
    public class Server : IServerTreeNode
	{
		#region Private Fields

    	private bool _isDefault;
		private string _parentPath;
    	private string _path;
		#endregion

		protected Server()
        {
        }

        public Server(
			string name, 
			string location, 
			string host, 
			string aeTitle, 
			int port, 
			bool isStreaming,
			int headerServicePort,
			int wadoServicePort)
        {
			NameOfServer = name;
            Location = location;
            Host = host;
            AETitle = aeTitle;
            Port = port;
        	IsStreaming = isStreaming;
			HeaderServicePort = headerServicePort;
        	WadoServicePort = wadoServicePort;
		}

		#region Public Fields

		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
    	public String NameOfServer;
		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public String Location;
		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public String Host;
		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public String AETitle;
		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public int Port;
		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public bool IsStreaming;
		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public int HeaderServicePort;
		/// <summary>
		/// Public field for serialization only; do not modify directly.
		/// </summary>
		public int WadoServicePort;

		#endregion

		#region IServerTreeNode Members

		public bool IsDefault
    	{
			get { return _isDefault; }
			set { _isDefault = value; }
    	}

		public bool IsLocalDataStore
        {
            get { return false; }
        }

        public bool IsServer
        {
            get { return true; }
        }

        public bool IsServerGroup
        {
            get { return false; }
        }

        public string Path
        {
			get { return _path; }
        }

        public bool IsRoot
        {
            get { return false; }
        }

        public string Name
        {
            get { return NameOfServer; }
        }

        public string ParentPath
        {
			get { return _parentPath; }
        }

        #endregion

		internal void ChangeParentPath(string newParentPath)
		{
			_parentPath = newParentPath ?? "";
			_path = _parentPath + this.Name;
		}

    	public override string ToString()
    	{
    		StringBuilder aeDescText = new StringBuilder();
			aeDescText.AppendFormat(SR.FormatServerDetails, this.Name, this.AETitle, this.Host, this.Port);
			if (!string.IsNullOrEmpty(this.Location))
			{
				aeDescText.AppendLine();
				aeDescText.AppendFormat(SR.Location, this.Location);
			}
			if(this.IsStreaming)
			{
				aeDescText.AppendLine();
				aeDescText.AppendFormat(SR.FormatStreamingDetails, this.HeaderServicePort, this.WadoServicePort);
			}
    		return aeDescText.ToString();
    	}
    }

	public interface IDicomServerConfigurationProvider
	{
		string Host { get; }
		string AETitle { get; }
		int Port { get; }
		string InterimStorageDirectory { get; }

		bool NeedsRefresh { get; }
		bool ConfigurationExists { get; }
		
		void Refresh();
		void RefreshAsync();

		event EventHandler Changed;
	}

	[Serializable]
	public class LocalDataStore : IServerTreeNode
	{
		public const string DefaultOfflineAE = "AETITLE";

		#region Private Fields

		private string _offlineAE;

		private string _path;
		private string _parentPath;
		private static readonly IDicomServerConfigurationProvider _dicomServerConfigurationProvider =
			DicomServerConfigurationHelper.GetDicomServerConfigurationProvider();
		
		#endregion

		internal LocalDataStore()
        {
        }

		public IDicomServerConfigurationProvider DicomServerConfigurationProvider
		{
			get { return _dicomServerConfigurationProvider; }
		}

		public override string ToString()
		{
			try
			{
				if (_dicomServerConfigurationProvider.NeedsRefresh)
					_dicomServerConfigurationProvider.RefreshAsync();

				if (_dicomServerConfigurationProvider.ConfigurationExists)
					return String.Format(SR.FormatLocalDataStoreDetails, Name, _dicomServerConfigurationProvider.AETitle, _dicomServerConfigurationProvider.Host, _dicomServerConfigurationProvider.Port, _dicomServerConfigurationProvider.InterimStorageDirectory);
			}
			catch (Exception e)
			{
				Platform.Log(LogLevel.Error, e);
			}

			return String.Format(SR.FormatLocalDataStoreConfigurationUnavailable, OfflineAE);
		}

		// NOTE: this is not good, but it's the only way to allow certain things to work without altering some of the services.
		// When the server tree is refactored, this must be dealt with.
		public string GetClientAETitle()
		{
			string ae = null;
			try
			{
				if (_dicomServerConfigurationProvider.NeedsRefresh)
					_dicomServerConfigurationProvider.RefreshAsync();

				if (_dicomServerConfigurationProvider.ConfigurationExists)
					ae = _dicomServerConfigurationProvider.AETitle;
			}
			catch(Exception e)
			{
				Platform.Log(LogLevel.Warn, e, "Failed to retrieve the dicom server's ae title.");
			}

			return ae ?? OfflineAE;
		}

		public string OfflineAE
		{
			get { return _offlineAE ?? DefaultOfflineAE; }
			set { _offlineAE = value ?? DefaultOfflineAE; }
		}

		#region IServerTreeNode Members

		public bool IsDefault
		{
			get { return true; }	
		}

		public bool IsLocalDataStore
		{
			get { return true; }
		}

		public bool IsServer
		{
			get { return false; }
		}

		public bool IsServerGroup
		{
			get { return false;	}
		}

		public bool IsRoot
		{
			get { return false; }
		}

		public string ParentPath
		{
			get { return _parentPath; }
		}

		public string Path
		{
			get { return _path; }
		}

		public string Name
		{
			get { return ServerTree.MyDataStoreTitle; }
		}

		#endregion

		internal void ChangeParentPath(string parentPath)
		{
			_parentPath = parentPath ?? "";
			_path = _parentPath + Name;
		}
	}
}

