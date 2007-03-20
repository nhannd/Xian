using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Dicom.Services;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class DicomServerTree
    {
        public DicomServerTree()
        {
            LoadDicomServers(false);
        }

        public void DeleteDicomServer()
        { 
            DicomServerGroup dsgp = FindParentDicomServer(CurrentServer);
            if (dsgp == null)
                return;
            for (int i = 0; i < dsgp.ChildServers.Count; i++)
            {
                if (dsgp.ChildServers[i].ServerName.Equals(CurrentServer.ServerName))
                {
                    dsgp.ChildServers.RemoveAt(i);
                    CurrentServer = dsgp;
                    SaveDicomServers();
                    FireServerTreeUpdatedEvent();
                    return;
                }
            }
            return;
        }

        public string DicomServerValidation(string serverName, string serverAE, string serverHost, int port)
        {
            if (serverName.Equals(AENavigatorComponent.MyDatastoreTitle) || serverName.Equals(AENavigatorComponent.MyServersTitle))
                return "Root Server";
            return DicomServerValidation(MyServerGroup, serverName, serverAE, serverHost, port);
        }

        public string DicomServerGroupNameValidation(string serverGroupName)
        {
            if (serverGroupName.Equals(AENavigatorComponent.MyDatastoreTitle) || serverGroupName.Equals(AENavigatorComponent.MyServersTitle))
                return "Root Server";
            return DicomServerGroupNameValidation(MyServerGroup, serverGroupName);
        }

        public List<DicomServer> FindChildServers(IDicomServer idsp)
        {
            _childServers = new List<DicomServer>();
            if (idsp == null || idsp.IsServer)
                return _childServers;
            GetChildServers((DicomServerGroup)idsp, true, true);
            _childServers.Sort(delegate(DicomServer s1, DicomServer s2) { return s1.ServerName.CompareTo(s2.ServerName); });
            return _childServers;
        }

        public void FireServerTreeUpdatedEvent()
        {
            EventsHelper.Fire(_serverTreeUpdated, this, EventArgs.Empty);
        }

        public void RenameDicomServerGroup(DicomServerGroup dsg, string newName, string oldPath, string newPath, int depth)
        {
            if (depth == 0)
            {
                oldPath = dsg.ServerPath + "/" + dsg.ServerName;
                dsg.ServerName = newName;
                DicomServerGroup pdsg = FindParentDicomServer(dsg);
                pdsg.ChildServers.Sort(delegate(IDicomServer s1, IDicomServer s2)
                {
                    string s1param = s1.IsServer ? "cc" : "bb"; s1param += s1.ServerName;
                    string s2param = s2.IsServer ? "cc" : "bb"; s2param += s2.ServerName;
                    return s1param.CompareTo(s2param);
                });
                newPath = dsg.ServerPath + "/" + dsg.ServerName;
            }
            foreach (IDicomServer ids in dsg.ChildServers)
            {
                ids.ServerPath = ids.ServerPath.Replace(oldPath, newPath);
                if (!ids.IsServer)
                    RenameDicomServerGroup((DicomServerGroup)ids, "", oldPath, newPath, depth+1);
            }
            return;
        }

        public DicomServer ReplaceDicomServer(DicomServer newDS)
        {
            DicomServerGroup dsg = FindParentDicomServer(CurrentServer);
            for (int i = 0; i < dsg.ChildServers.Count; i++)
            {
                if (dsg.ChildServers[i].ServerName.Equals(CurrentServer.ServerName))
                {
                    dsg.ChildServers.RemoveAt(i);
                    break;
                }
            }
            dsg.AddChild(newDS);
            ReplaceDicomServersByName(_myServerGroup, newDS);
            return newDS;
        }

        public void SaveDicomServers()
        {
            DicomAEGroup dsgs = ConvertDicomServers(_myServerGroup);
            XmlSerializer xmlFormat = new XmlSerializer(typeof(DicomAEGroup), new Type[] { typeof(List<DicomAEGroup>), typeof(DicomAEGroup), typeof(List<DicomAEServer>), typeof(DicomAEServer)});
            Stream fStream = new FileStream(AENavigatorComponent.MyServersXmlFile, FileMode.Create, FileAccess.Write, FileShare.None);
            xmlFormat.Serialize(fStream, dsgs);
            fStream.Close();
            return;
        }

        #region Internal Methods

        private string DicomServerValidation(DicomServerGroup ds, string serverName, string serverAE, string serverHost, int port)
        {
            foreach (IDicomServer cids in (ds.ChildServers))
            {
                if (!cids.IsServer)
                {
                    if (cids.ServerName.Equals(serverName) && cids.ServerPath.Equals(CurrentServer.ServerPath + "/" + CurrentServer.ServerName))
                        return cids.ServerPath + "/" + cids.ServerName;
                    string msg = DicomServerValidation((DicomServerGroup)cids, serverName, serverAE, serverHost, port);
                    if (!msg.Equals(""))
                        return msg;
                    continue;
                }
                // editting server
                if (CurrentServer.IsServer)
                {
                    if (cids.ServerName.Equals(serverName) && !serverName.Equals(CurrentServer.ServerName) 
                        && (cids.ServerPath.Equals(CurrentServer.ServerPath) || !((DicomServer)cids).DicomAE.AE.Equals(serverAE) 
                        || !((DicomServer)cids).DicomAE.Host.Equals(serverHost) || ((DicomServer)cids).DicomAE.Port != port))
                        return cids.ServerPath + "/" + cids.ServerName;
                }
                // New server
                else
                {
                    if (cids.ServerName.Equals(serverName) && (cids.ServerPath.Equals(CurrentServer.ServerPath + "/" + CurrentServer.ServerName)
                        || !((DicomServer)cids).DicomAE.AE.Equals(serverAE) || !((DicomServer)cids).DicomAE.Host.Equals(serverHost)
                        || ((DicomServer)cids).DicomAE.Port != port))
                        return cids.ServerPath + "/" + cids.ServerName;
                }
            }
            return "";
        }

        private string DicomServerGroupNameValidation(DicomServerGroup ds, string serverGroupName)
        {
            foreach (IDicomServer cids in (ds.ChildServers))
            {
                if (cids.IsServer)
                {
                    if (cids.ServerName.Equals(serverGroupName) && cids.ServerPath.Equals(CurrentServer.ServerPath + "/" + CurrentServer.ServerName))
                        return cids.ServerPath + "/" + cids.ServerName;
                    continue;
                }
                if (cids.ServerName.Equals(serverGroupName))
                {
                    return cids.ServerPath + "/" + cids.ServerName;
                }
                string msg = DicomServerGroupNameValidation((DicomServerGroup)cids, serverGroupName);
                if (!msg.Equals(""))
                    return msg;
            }
            return "";
        }

        private void CheckDefaultServerSettings(bool isupdated)
        {
            string[] svrPaths = new string[] { "." };
            if (isupdated)
            {
                _myServerGroup = new DicomServerGroup();

				//!!
				_myServerGroup.AddChild(new DicomServer(AENavigatorComponent.MyDatastoreTitle, _myServerGroup.ServerPath, "", "localhost", "AETITLE", 4006));
                _myServerGroup.AddChild(new DicomServerGroup(AENavigatorComponent.MyServersTitle, _myServerGroup.ServerPath));
            }
            else
            {
                if (FindDicomServer(_myServerGroup, AENavigatorComponent.MyDatastoreTitle, svrPaths, 1) == null)
                {
					_myServerGroup.AddChild(new DicomServer(AENavigatorComponent.MyDatastoreTitle, ".", "", "localhost", "AETITLE", 4006));
                    isupdated = true;
                }
                if (FindDicomServer(_myServerGroup, AENavigatorComponent.MyServersTitle, svrPaths, 1) == null)
                {
                    _myServerGroup.AddChild(new DicomServerGroup(AENavigatorComponent.MyServersTitle, "."));
                    isupdated = true;
                }
            }

            if (isupdated)
                SaveDicomServers();
            _currentServer = FindDicomServer(_myServerGroup, AENavigatorComponent.MyDatastoreTitle, svrPaths, 1);
            return;
        }

        private DicomAEGroup ConvertDicomServers(DicomServerGroup dsg)
        {
            if (dsg == null)
                return null;

            List<DicomAEGroup> chdGroup = new List<DicomAEGroup>();
            List<DicomAEServer> chdServer = new List<DicomAEServer>();
            if (dsg.ChildServers != null && dsg.ChildServers.Count > 0)
            {
                foreach (IDicomServer ids in dsg.ChildServers)
                {
                    if (ids.IsServer)
                    {
                        DicomServer cds = (DicomServer)ids;
                        chdServer.Add(new DicomAEServer(cds.ServerName, cds.ServerPath, cds.ServerLocation, cds.DicomAE.Host, cds.DicomAE.AE, cds.DicomAE.Port));
                    }
                    else
                    {
                        DicomServerGroup cdsg = (DicomServerGroup)ids;
                        chdGroup.Add(ConvertDicomServers(cdsg));
                    }
                }
            }
            DicomAEGroup dsgs = new DicomAEGroup(dsg.ServerName, dsg.ServerPath, chdGroup, chdServer);
            return dsgs;
        }

        private void GetChildServers(DicomServerGroup dsp, bool recursive, bool identicalAE)
        {
            foreach (IDicomServer ids in dsp.ChildServers)
            {
                if (ids.IsServer)
                {
                    if (!identicalAE)
                    {
                        _childServers.Add((DicomServer)ids);
                        continue;
                    }
                    bool exists = false;
                    foreach (DicomServer ds in _childServers)
                    {
                        if (((DicomServer)ids).DicomAE.AE.Equals(ds.DicomAE.AE) 
                            && ((DicomServer)ids).DicomAE.Host.Equals(ds.DicomAE.Host)
                            && ((DicomServer)ids).DicomAE.Port == ds.DicomAE.Port)
                        {
                            exists = true;
                            break;
                        }
                    }
                    if (!exists)
                        _childServers.Add((DicomServer)ids);
                }
                else if (recursive)
                {
                    GetChildServers((DicomServerGroup)ids, recursive, identicalAE);
                }
            }
        }

        private IDicomServer FindDicomServer(IDicomServer idsp, string serverName, string[] svrPaths, int depth)
        {
            if (idsp == null || idsp.IsServer || serverName == null || serverName.Equals("")
                || svrPaths == null || depth <= 0 || svrPaths.Length < depth)
                return null;
            if (depth == 1 && !idsp.ServerPath.Equals("."))
                return null;
            DicomServerGroup dsg = (DicomServerGroup)idsp;
            foreach (IDicomServer ids in dsg.ChildServers)
            {
                if (svrPaths.Length == depth && ids.ServerName.Equals(serverName))
                {
                    return ids;
                }
                if (svrPaths.Length > depth && !ids.IsServer && ids.ServerName.Equals(svrPaths[depth]))
                {
                    IDicomServer nids = FindDicomServer(ids, serverName, svrPaths, depth + 1);
                    if (nids != null)
                        return nids;
                }
            }
            return null;
        }

        private bool ReplaceDicomServersByName(DicomServerGroup dsg, DicomServer newDs)
        {
            bool isUpdated = false;
            for (int i = 0; i < dsg.ChildServers.Count; i++)
            {
                IDicomServer ids = dsg.ChildServers[i];
                if (!ids.IsServer)
                {
                    if (ReplaceDicomServersByName((DicomServerGroup)ids, newDs) && !isUpdated)
                        isUpdated = true;
                    continue;
                }
                if (ids.ServerName.Equals(newDs.ServerName) && (!((DicomServer)ids).DicomAE.AE.Equals(newDs.DicomAE.AE)
                        || !((DicomServer)ids).DicomAE.Host.Equals(newDs.DicomAE.Host) || ((DicomServer)ids).DicomAE.Port != newDs.DicomAE.Port))
                {
                    dsg.ChildServers[i] = new DicomServer(ids.ServerName, ids.ServerPath, ((DicomServer)ids).ServerLocation, newDs.DicomAE.Host, newDs.DicomAE.AE, newDs.DicomAE.Port);
                    dsg.ChildServers.Sort(delegate(IDicomServer s1, IDicomServer s2)
                    {
                        string s1param = s1.IsServer ? "cc" : "bb"; s1param += s1.ServerName;
                        string s2param = s2.IsServer ? "cc" : "bb"; s2param += s2.ServerName;
                        return s1param.CompareTo(s2param);
                    });
                    if (!isUpdated)
                        isUpdated = true;
                }
            }
            return isUpdated;
        }

        private DicomServerGroup FindParentDicomServer(IDicomServer ids)
        {
            if (ids == null)
                return null;
            if (ids.ServerPath.Equals("."))
            {
                return _myServerGroup;
            }
            string svrName = ids.ServerPath.Substring(ids.ServerPath.LastIndexOf('/') + 1);
            string[] svrPaths = ids.ServerPath.Substring(0, ids.ServerPath.LastIndexOf('/')).Split('/');
            IDicomServer dsg = FindDicomServer(_myServerGroup, svrName, svrPaths, 1);
            if (dsg == null || dsg.IsServer)
                return null;
            return (DicomServerGroup)dsg;
        }

        public void LoadDicomServers(bool initDefault)
        {
            NewServerTree tree = new NewServerTree();

            _myServerGroup = new DicomServerGroup();
            bool isupdated = true;
            if (!initDefault && File.Exists(AENavigatorComponent.MyServersXmlFile))
            {
                Stream fStream = File.OpenRead(AENavigatorComponent.MyServersXmlFile);
                XmlSerializer xmlFormat = new XmlSerializer(typeof(DicomAEGroup), new Type[] { typeof(List<DicomAEGroup>), typeof(DicomAEGroup), typeof(List<DicomAEServer>), typeof(DicomAEServer) });
                DicomAEGroup dsgs = (DicomAEGroup)xmlFormat.Deserialize(fStream);
                if (dsgs != null)
                {
                    _myServerGroup = new DicomServerGroup(dsgs.Name, dsgs.Path, dsgs.ChildGroups, dsgs.ChildServers);
                    isupdated = false;
                }
                fStream.Close();
            }

            //check the default server nodes
            CheckDefaultServerSettings(isupdated);

            return;
        }

        #endregion

        #region Fields

        DicomServerGroup _myServerGroup;
        List<DicomServer> _childServers;
        IDicomServer _currentServer;
        private event EventHandler _serverTreeUpdated;

        public DicomServerGroup MyServerGroup
        {
            get { return _myServerGroup; }
            set { _myServerGroup = value; }
        }

        public List<DicomServer> ChildServers
        {
            get { return _childServers; }
            set { _childServers = value; }
        }

        public IDicomServer CurrentServer
        {
            get { return _currentServer; }
            set { _currentServer = value; }
        }

        public event EventHandler ServerTreeUpdated
        {
            add { _serverTreeUpdated += value; }
            remove { _serverTreeUpdated -= value; }
        }

        #endregion
    }

    public class NewServerTree
    {
        public NewServerTree()
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
            return;
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
                    return;
                }
            }

            return;
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
                    return;
                }
            }
            return;
        }

        public void SaveDicomServers()
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(ServerTreeRoot), new Type[] { 
                    typeof(ServerGroup),
                    typeof(LocalDataStore),
                    typeof(Server),
                    typeof(List<ServerGroup>),
                    typeof(List<Server>)
                });

            Stream fStream = new FileStream("New_" + AENavigatorComponent.MyServersXmlFile, FileMode.Create, FileAccess.Write, FileShare.None);
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

            if (File.Exists("New_" + AENavigatorComponent.MyServersXmlFile))
            {
                Stream fStream = File.OpenRead("New_" + AENavigatorComponent.MyServersXmlFile);

                using (fStream)
                {
                    XmlSerializer xmlFormat = new XmlSerializer(typeof(ServerTreeRoot), new Type[] { 
                            typeof(ServerGroup),
                            typeof(LocalDataStore),
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
                _rootNode.LocalDataStoreNode = new LocalDataStore(AENavigatorComponent.MyDatastoreTitle, "", ".", AENavigatorComponent.MyDatastoreTitle, LocalApplicationEntity.AETitle, LocalApplicationEntity.Port);
                _rootNode.ServerGroupNode = new ServerGroup(AENavigatorComponent.MyServersTitle, ".");
                _rootNode.ServerGroupNode.ChildGroups.Add(new ServerGroup("Example Group", "./" + AENavigatorComponent.MyServersTitle));
                _rootNode.ServerGroupNode.ChildServers.Add(new Server("Sample server", "Rm 101", "./" + AENavigatorComponent.MyServersTitle, "localhost", "SAMPLE", 104));
                SaveDicomServers();
            }

            return; 
        }
        private IServerTreeNode FindParentGroup(IServerTreeNode node)
        {
            if (node.ParentPath.Equals("."))
                return _rootNode;

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

            return;
        }

        private void LoadDefaultServers()
        {

        }
        #endregion

        #region Private fields
        private ServerTreeRoot _rootNode;
        private IServerTreeNode _currentNode;
        private event EventHandler _serverTreeUpdated;
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
            get { return "Root"; }
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
        public ServerGroup()
            : this(AENavigatorComponent.MyServersRoot, ".")
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

        public Server(string name, string location, string path, string host, string aeTitle, int port)
        {
            NameOfServer = name;
            Location = location;
            PathToParent = path;
            PathToServer = PathToParent + "/" + NameOfServer;
            Host = host;
            AETitle = aeTitle;
            Port = port;
        }

        public ApplicationEntity GetApplicationEntity()
        {
            return new ApplicationEntity(new HostName(Host), new AETitle(AETitle), new ListeningPort(Port));
        }

        public override string ToString()
        {
            ApplicationEntity ae = GetApplicationEntity();
            StringBuilder aeDescText = new StringBuilder();
			aeDescText.AppendFormat(SR.FormatTooltipServerDetails, Name, ae.AE, ae.Host, ae.Port, Location);
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

    [Serializable]
    public class LocalDataStore : Server, IServerTreeNode
    {
        public LocalDataStore()
        {

        }

        public LocalDataStore(string name, string location, string path, string host, string aeTitle, int port)
            : base(name, location, path, host, aeTitle, port)
        {

        }

        #region IServerTreeNode Members

        public new bool IsLocalDataStore
        {
            get { return true; }
        }

        public new bool IsServer
        {
            get { return true; }
        }

        #endregion
    }
}

