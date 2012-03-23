#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Common.DicomServer;

namespace ClearCanvas.ImageViewer.Common.ServerTree
{
    public class ServerTree
	{
		internal static readonly string MyServersXmlFile = "DicomAEServers.xml";
		
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

		internal static string GetServersXmlFileName()
		{
			return Path.Combine(Platform.InstallDirectory, MyServersXmlFile);
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
			Stream fStream = new FileStream(GetServersXmlFileName(), FileMode.Create, FileAccess.Write, FileShare.Read);
			_serializer.Serialize(fStream, _rootNode);
            fStream.Close();
            return;
        }

		public Server FindServer(string path)
		{
			return FindServer(this.RootNode.ServerGroupNode, path);
		}

		public Server FindServer(ServerGroup group, string path)
		{
			foreach (Server server in group.ChildServers)
			{
				if (server.Path == path)
					return server;
			}

			foreach (ServerGroup childGroup in group.ChildGroups)
			{
				Server server = FindServer(childGroup, path);
				if (server != null)
					return server;
			}

			return null;
		}
		
		public List<IServerTreeNode> FindChildServers()
		{
			return FindChildServers(this.RootNode.ServerGroupNode);
		}

    	public List<IServerTreeNode> FindChildServers(ServerGroup serverGroup)
        {
            List<IServerTreeNode> listOfChildrenServers = new List<IServerTreeNode>();
            FindChildServers(serverGroup, listOfChildrenServers);
            return listOfChildrenServers;
		}
		
    	public ServerGroup FindServerGroup(string path)
		{
			return FindServerGroup(this.RootNode.ServerGroupNode, path);
		}

    	public ServerGroup FindServerGroup(ServerGroup startNode, string path)
		{
			if (!startNode.IsServerGroup)
				return null;

			if (startNode.Path == path)
				return startNode;

			foreach (ServerGroup childrenServerGroup in startNode.ChildGroups)
			{
				ServerGroup foundNode = FindServerGroup(childrenServerGroup, path);
				if (null != foundNode)
					return foundNode;
			}

			return null;
		}
		
		#endregion

		#region Private methods

		private void LoadServers()
        {
            _rootNode = new ServerTreeRoot();

			string file = GetServersXmlFileName();
			if (File.Exists(file))
            {
				Stream fStream = File.OpenRead(file);

                using (fStream)
                {
					ServerTreeRoot serverTreeRoot = (ServerTreeRoot)_serializer.Deserialize(fStream);
					_rootNode = serverTreeRoot;
                }
            }
            else
            {
                // create default entries and save them to disk
                _rootNode.ServerGroupNode = new ServerGroup(@"My Servers");
				_rootNode.ServerGroupNode.ChildGroups.Add(new ServerGroup(SR.ExampleGroup));
                _rootNode.ServerGroupNode.ChildServers.Add(
					new Server(SR.ExampleServer, "", "localhost", "SAMPLE", 104, false, 50221, 1000));
            	Save();
            }

			_rootNode.InitializeChildPaths();
        }

		private ServerGroup FindParentGroup(IServerTreeNode node)
        {
            return FindServerGroup(_rootNode.ServerGroupNode, node.ParentPath);
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

    	public bool IsChecked
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
            get { return @"Root"; }
        }

        string IServerTreeNode.DisplayName
        {
            get { return string.Empty; }
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
		private const string _rootServersGroupPath = @"./My Servers/";
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

    	public bool IsChecked
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

        public string DisplayName
        {
            // if this group is the default "My Servers" node (and not manually customized by user), display the localized name
            get { return Path == _rootServersGroupPath ? SR.MyServersTitle : Name; }
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

		public bool IsEntireGroupChecked()
		{
			return IsEntireGroupChecked(this);
		}

		public bool IsEntireGroupUnchecked()
		{
			return IsEntireGroupUnchecked(this);
		}

		public List<Server> GetCheckedServers(bool recursive)
		{
			List<Server> checkedServers = new List<Server>();
			if (recursive)
			{
				foreach (ServerGroup childGroup in ChildGroups)
					checkedServers.AddRange(childGroup.GetCheckedServers(true));
			}

			checkedServers.AddRange(CollectionUtils.Select(ChildServers, delegate(Server server) { return server.IsChecked; }));
			return checkedServers;
		}

		private static bool IsEntireGroupChecked(ServerGroup group)
		{
			foreach (Server server in group.ChildServers)
			{
				if (!server.IsChecked)
					return false;
			}

			foreach (ServerGroup subGroup in group.ChildGroups)
			{
				if (!IsEntireGroupChecked(subGroup))
					return false;
			}

			return true;
		}

		private static bool IsEntireGroupUnchecked(ServerGroup group)
		{
			foreach (Server server in group.ChildServers)
			{
				if (server.IsChecked)
					return false;
			}

			foreach (ServerGroup subGroup in group.ChildGroups)
			{
				if (!IsEntireGroupUnchecked(subGroup))
					return false;
			}

			return true;
		}

    	public override string ToString()
    	{
			return Path.StartsWith(_rootServersGroupPath) ? string.Format(@"./{0}/{1}", SR.MyServersTitle, Path.Substring(_rootServersGroupPath.Length)) : Path;
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
}
