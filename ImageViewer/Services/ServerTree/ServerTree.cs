using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Services.ServerTree
{
    public class ServerTree
    {
		public static readonly string MyServersXmlFileDefault = "DicomAEServers.xml";
		public static readonly string MyDataStoreTitleDefault = SR.MyDataStoreTitle;
		public static readonly string MyServersTitleDefault = SR.MyServersTitle;
		public static readonly string RootNodeName = "Root";
		public static readonly string MyServersRootName = "MyServersRoot";
		
		#region Private fields

		private ServerTreeRoot _rootNode;
		private IServerTreeNode _currentNode;
		private event EventHandler _serverTreeUpdated;

		private string _myServersXmlFile = MyServersXmlFileDefault;
		private string _myDataStoreTitle = MyDataStoreTitleDefault;
		private string _myServersTitle = MyServersTitleDefault;

		#endregion

		public ServerTree()
        {
            LoadServers();
            CurrentNode = _rootNode.LocalDataStoreNode;
        }

        public void FireServerTreeUpdatedEvent()
        {
            EventsHelper.Fire(_serverTreeUpdated, this, EventArgs.Empty);
        }

        public void ReplaceDicomServerInCurrentGroup(Server newServer)
        {
            ServerGroup serverGroup = FindParentGroup(CurrentNode) as ServerGroup;
            for (int i = 0; i < serverGroup.ChildServers.Count; i++)
            {
                if (serverGroup.ChildServers[i].Name.Equals(CurrentNode.Name))
                {
                    serverGroup.ChildServers.RemoveAt(i);
                    break;
                }
            }

            serverGroup.AddChild(newServer);
        }


        public bool IsDuplicateServerInGroup(IServerTreeNode node, string toFindServerName, string toFindServerAE, string toFindServerHost, int toFindPort, out string conflictingServerPath)
        {
            if (node is ServerGroup)
                return IsDuplicateServerInGroup(node as ServerGroup, toFindServerName, toFindServerAE, toFindServerHost, toFindPort, out conflictingServerPath);
            else
            {
                conflictingServerPath = "";
                return false;
            }
        }

        public bool IsDuplicateServerGroupInGroup(string toFindServerGroupName, out string conflictingGroupPath)
        {
            if (CurrentNode is ServerGroup)
                return IsDuplicateServerGroupInGroup(CurrentNode as ServerGroup, toFindServerGroupName, out conflictingGroupPath);
            else
            {
                conflictingGroupPath = "";
                return false;
            }
        }

        public void DeleteServerGroup()
        {
            ServerGroup parentGroup = FindParentGroup(CurrentNode) as ServerGroup;

            if (null == parentGroup)
                return;

            for (int i = 0; i < parentGroup.ChildGroups.Count; ++i)
            {
                if (parentGroup.ChildGroups[i].Name == CurrentNode.Name)
                {
                    parentGroup.ChildGroups.RemoveAt(i);
                    CurrentNode = parentGroup;
                    SaveDicomServers();
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

        public void DeleteDicomServer()
        {
            ServerGroup parentGroup = FindParentGroup(CurrentNode) as ServerGroup;

            if (parentGroup == null)
                return;

            for (int i = 0; i < parentGroup.ChildServers.Count; i++)
            {
                if (parentGroup.ChildServers[i].NameOfServer.Equals(CurrentNode.Name))
                {
                    parentGroup.ChildServers.RemoveAt(i);
                    CurrentNode = parentGroup;
                    SaveDicomServers();
                    FireServerTreeUpdatedEvent();
                    break;
                }
            }
        }

		public void SaveDicomServers()
		{
            XmlSerializer xmlFormat = new XmlSerializer(typeof(ServerTreeRoot), new Type[] { 
                    typeof(ServerGroup),
                    typeof(Server),
                    typeof(List<ServerGroup>),
                    typeof(List<Server>)
                });

			Stream fStream = new FileStream(this.MyServersXmlFile, FileMode.Create, FileAccess.Write, FileShare.Read);
            xmlFormat.Serialize(fStream, _rootNode);
            fStream.Close();
            return;
        }

        public List<IServerTreeNode> FindChildServers(ServerGroup serverGroup)
        {
            List<IServerTreeNode> listOfChildrenServers = new List<IServerTreeNode>();
            FindChildServers(serverGroup, listOfChildrenServers);
            return listOfChildrenServers;
        }

        #region Properties

		public string MyServersXmlFile
		{
			get { return _myServersXmlFile; }
			set { _myServersXmlFile = value; }
		}

		public string MyDatastoreTitle
		{
			get { return _myDataStoreTitle; }
			set { _myDataStoreTitle = value; }
		}

		public string MyServersTitle
		{
			get { return _myServersTitle; }
			set { _myServersTitle = value; }
		}

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

        #region Private methods

        private void LoadServers()
        {
            _rootNode = new ServerTreeRoot();

            if (File.Exists(this.MyServersXmlFile))
            {
				Stream fStream = File.OpenRead(this.MyServersXmlFile);

                using (fStream)
                {
                    XmlSerializer xmlFormat = new XmlSerializer(typeof(ServerTreeRoot), new Type[] { 
                            typeof(ServerGroup),
                            typeof(Server),
                            typeof(List<ServerGroup>),
                            typeof(List<Server>)
                        });

                    ServerTreeRoot serverTreeRoot = (ServerTreeRoot)xmlFormat.Deserialize(fStream);

                    if (serverTreeRoot != null)
                        _rootNode = serverTreeRoot;
                }
            }
            else
            {
                // create default entries and save them to disk
                _rootNode.ServerGroupNode = new ServerGroup(this.MyServersTitle, ".");
                _rootNode.ServerGroupNode.ChildGroups.Add(new ServerGroup(SR.ExampleGroup, "./" + this.MyServersTitle));
                _rootNode.ServerGroupNode.ChildServers.Add(new Server(SR.ExampleServer, SR.ExampleLocation, "./" + this.MyServersTitle, "localhost", "SAMPLE", 104));
                SaveDicomServers();
            }

			_rootNode.LocalDataStoreNode = new LocalDataStore(this.MyDatastoreTitle, ".");
        }

		private IServerTreeNode FindParentGroup(IServerTreeNode node)
        {
            if (node.ParentPath.Equals("."))
                return _rootNode.ServerGroupNode;

            // a node's ParentPath component is automatically the path that leads to the node's parent
            IServerTreeNode foundNode = FindNode(_rootNode.ServerGroupNode, node.ParentPath);

            if (!foundNode.IsServerGroup)
                return null;
            else
                return foundNode;
        }
        private IServerTreeNode FindNode(IServerTreeNode startSearchNode, string path)
        {
            if (!startSearchNode.IsServerGroup)
                return null;

            if (startSearchNode.Path == path)
                return startSearchNode;

            ServerGroup serverGroup = startSearchNode as ServerGroup;
            foreach (Server server in serverGroup.ChildServers)
            {
                if (server.Path == path)
                    return server;
            }

            foreach (ServerGroup childrenServerGroup in serverGroup.ChildGroups)
            {
                IServerTreeNode foundNode = FindNode(childrenServerGroup, path);
                if (null != foundNode)
                    return foundNode;
            }

            return null;
        }
        private IServerTreeNode FindDicomServer(IServerTreeNode startSearchNode, string serverName, string[] svrPaths, int depth)
        {
            if (startSearchNode == null ||
                !startSearchNode.IsServerGroup ||
                serverName == null ||
                serverName == "" ||
                svrPaths == null ||
                depth <= 0 ||
                svrPaths.Length < depth)
                return null;

            if (depth == 1 && !startSearchNode.Path.Equals("."))
                return null;

            ServerGroup serverGroup = startSearchNode as ServerGroup;
            if (null == serverGroup)
                return null;

            foreach (Server server in serverGroup.ChildServers)
            {
                if (svrPaths.Length == depth && server.NameOfServer.Equals(serverName))
                    return server;

                if (svrPaths.Length > depth && !server.IsServer && server.NameOfServer.Equals(svrPaths[depth]))
                {
                    IServerTreeNode foundServer = FindDicomServer(server, serverName, svrPaths, depth + 1);
                    if (foundServer != null)
                        return foundServer;
                }
            }

            return null;
        }
        private bool IsDuplicateServerInGroup(ServerGroup serverGroup, string toFindServerName, string toFindServerAE, string toFindServerHost, int toFindServerPort, out string conflictingServerPath)
        {
            foreach (Server server in serverGroup.ChildServers)
            {
                // if we find a duplicate
                if (server.AETitle == toFindServerAE &&
                    server.Host == toFindServerHost &&
                    server.Port == toFindServerPort)
                {
                    conflictingServerPath = server.Path;
                    return true;
                }

            }

            conflictingServerPath = "";
            return false;
        }
        private bool IsDuplicateServerGroupInGroup(ServerGroup searchSite, string toFindServerGroupName, out string conflictingGroupPath)
        {
            foreach (ServerGroup serverGroup in searchSite.ChildGroups)
            {
                // if we find a duplicate
                if (serverGroup.Name == toFindServerGroupName)
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
        bool IsLocalDataStore { get; }
        bool IsServer { get; }
        bool IsServerGroup { get; }
        bool IsRoot { get; }
        string ParentPath { get; }
        string Path { get; }
        string Name { get; }
        bool CanAddAsChild(IServerTreeNode node);
        void ChangeParentPath(string newParentPath);
    }

    [Serializable]
    public class ServerTreeRoot : IServerTreeNode
    {
        public ServerTreeRoot()
        {
        }

		[NonSerialized]
		[XmlIgnore]
		public LocalDataStore LocalDataStoreNode;
        public ServerGroup ServerGroupNode;

        #region IServerTreeNode Members

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
            get { return false; }
        }

        public string Path
        {
            get { return "."; }
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
            get { return "-"; }
        }

        public bool CanAddAsChild(IServerTreeNode node)
        {
            return false;
        }

        public void ChangeParentPath(string newParentPath)
        {
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion
    }

    [Serializable]
    public class ServerGroup : IServerTreeNode
    {
		protected ServerGroup()
			: this(ServerTree.MyServersRootName, ".")
		{
		}

		public ServerGroup(string name, string pathToParent)
        {
            NameOfGroup = name;
            PathToParent = pathToParent;
            PathToGroup = PathToParent + "/" + NameOfGroup;
            ChildGroups = new List<ServerGroup>();
            ChildServers = new List<Server>();
        }

        public void AddChild(IServerTreeNode child)
        {
            if (child.IsServer)
                ChildServers.Add(child as Server);
            else if (child.IsServerGroup)
                ChildGroups.Add(child as ServerGroup);
        }

        public override string ToString()
        {
            return Path;
        }

        public String NameOfGroup;
        public String PathToGroup;
        public String PathToParent;
        public List<ServerGroup> ChildGroups;
        public List<Server> ChildServers;

        #region IServerTreeNode Members

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
            get { return PathToGroup; }
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
            get { return PathToParent; }
        }

        public bool CanAddAsChild(IServerTreeNode node)
        {
            // can't add me to myself
            if (node == this)
                return false;

            if (node.ParentPath == this.Path)
                return false;

            // if the node that's being added is actually a direct parent of
            // me, then it's not possible for it to be added as a child
            // direct parent:
            //      me - ./a/b/c/me
            //      direct parents - ./a/b/c; ./a/b; ./a
            //      non-d parents - ./a/b/d; ./a/b/c/e
            // thus, if the proposed node's path is NOT wholly contained in my own
            // path, then it's okay to add the proposed node to me
            return (this.Path.IndexOf(node.Path) == -1);
        }

        public void ChangeParentPath(string newParentPath)
        {
            // change the parent path of myself and all my children
            this.PathToParent = newParentPath;
            this.PathToGroup = newParentPath + "/" + this.NameOfGroup;

            foreach (Server server in ChildServers)
                server.ChangeParentPath(this.Path);

            foreach (ServerGroup serverGroup in ChildGroups)
                serverGroup.ChangeParentPath(this.Path);
        }

        #endregion
    }

    [Serializable]
    public class Server : IServerTreeNode
    {
        public Server()
        {
        }

        public Server(string name, string location, string pathToParent, string host, string aeTitle, int port)
        {
            NameOfServer = name;
            Location = location;
            PathToParent = pathToParent;
            PathToServer = PathToParent + "/" + NameOfServer;
            Host = host;
            AETitle = aeTitle;
            Port = port;
        }

        public override string ToString()
        {
            StringBuilder aeDescText = new StringBuilder();
			aeDescText.AppendFormat(SR.FormatServerDetails, this.Name, this.AETitle, this.Host, this.Port, this.Location);
            return aeDescText.ToString();
        }

        public String NameOfServer;
        public String Location;
        public String PathToParent;
        public String PathToServer;
        public String Host;
        public String AETitle;
        public int Port;

        #region IServerTreeNode Members

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
            get { return PathToServer; }
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
            get { return PathToParent; }
        }

        public bool CanAddAsChild(IServerTreeNode node)
        {
            return false;
        }

        public void ChangeParentPath(string newParentPath)
        {
            this.PathToParent = newParentPath;
            this.PathToServer = newParentPath + "/" + this.NameOfServer;
        }

        #endregion
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

	public class LocalDataStore : IServerTreeNode
    {
		private string _name;
		private string _path;
		private string _parentPath;

		private IDicomServerConfigurationProvider _dicomServerConfigurationProvider;

		public LocalDataStore()
		{ 
		}

		public LocalDataStore(string name, string pathToParent)
        {
			_name = name;
			_parentPath = pathToParent;
			_path = _parentPath + "/" + _name;
        }

		public IDicomServerConfigurationProvider DicomServerConfigurationProvider
		{
			get { return _dicomServerConfigurationProvider; }
			set { _dicomServerConfigurationProvider = value; }
		}

		public override string ToString()
		{
			StringBuilder aeDescText = new StringBuilder();

			if (_dicomServerConfigurationProvider == null)
			{
				aeDescText.AppendFormat(SR.LocalDataStoreConfigurationUnavailable);
			}
			else
			{
				try
				{
					if (_dicomServerConfigurationProvider.NeedsRefresh)
						_dicomServerConfigurationProvider.RefreshAsync();

					if (_dicomServerConfigurationProvider.ConfigurationExists)
						aeDescText.AppendFormat(SR.FormatLocalDataStoreDetails, _name, _dicomServerConfigurationProvider.AETitle, _dicomServerConfigurationProvider.Host, _dicomServerConfigurationProvider.Port, _dicomServerConfigurationProvider.InterimStorageDirectory);
					else
						aeDescText.AppendFormat(SR.LocalDataStoreConfigurationUnavailable);
				}
				catch (Exception e)
				{
					Platform.Log(e);
					aeDescText.AppendFormat(SR.LocalDataStoreConfigurationUnavailable);
				}
			}

			return aeDescText.ToString();
		}

		#region IServerTreeNode Members

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
			get { return _name; }
		}

		public bool CanAddAsChild(IServerTreeNode node)
		{
			return false;
		}

		public void ChangeParentPath(string newParentPath)
		{
			_parentPath = newParentPath;
			_path = newParentPath + "/" + _name;
		}

		#endregion
	}
}

