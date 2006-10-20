using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class DicomServerTree
    {
        public DicomServerTree()
        {
            LoadDicomServers();
        }

        public bool DicomServerNameExists(string serverName)
        {
            if (!CurrentServer.IsServer)
                return DicomServerNameExists(MyServerGroup, serverName, "", false, true);
            else
                return DicomServerNameExists(MyServerGroup, serverName, CurrentServer.ServerPath, true, true);
        }

        public bool DicomServerGroupNameExists(string serverGroupName, bool isNewServerGroup)
        {
            if (isNewServerGroup)
                return DicomServerNameExists(MyServerGroup, serverGroupName, "", false, true);
            else
                return DicomServerNameExists(MyServerGroup, serverGroupName, CurrentServer.ServerPath, true, true);
        }

        public bool DicomServerAEExists(string serverAE)
        {
            if (CurrentServer == null || !CurrentServer.IsServer)
                return DicomServerAEExists(MyServerGroup, serverAE, "", false, true);
            else
                return DicomServerAEExists(MyServerGroup, serverAE, CurrentServer.ServerPath, true, true);
        }

        public List<DicomServer> FindChildServers(IDicomServer idsp, bool recursive)
        {
            _childServers = new List<DicomServer>();
            if (idsp == null || idsp.IsServer)
                return _childServers;
            GetChildServers((DicomServerGroup)idsp, recursive);
            _childServers.Sort(delegate(DicomServer s1, DicomServer s2) { return s1.ServerName.CompareTo(s2.ServerName); });
            return _childServers;
        }

        public DicomServerGroup RemoveDicomServer(IDicomServer ids)
        {
            DicomServerGroup dsgp = FindParentDicomServer(ids);
            if (dsgp == null)
                return null;
            for (int i = 0; i < dsgp.ChildServers.Count; i++)
            {
                if (dsgp.ChildServers[i].ServerName.Equals(ids.ServerName))
                {
                    dsgp.ChildServers.RemoveAt(i);
                    return dsgp;
                }
            }
            return null;
        }

        public bool RenameDicomServerGroup(DicomServerGroup dsg, string newName, string oldPath, string newPath, int depth)
        {
            if (dsg == null || depth < 0)
                return false;
            if (depth == 0)
            {
                oldPath = dsg.GroupID;
                dsg.ServerName = newName;
                newPath = dsg.GroupID;
            }
            foreach (IDicomServer ids in dsg.ChildServers)
            {
                ids.ServerPath = ids.ServerPath.Replace(oldPath, newPath);
                if (!ids.IsServer)
                    RenameDicomServerGroup((DicomServerGroup)ids, "", oldPath, newPath, depth+1);
            }
            return true;
        }

        public DicomServer ReplaceDicomServer(DicomServer oldDS, DicomServer newDS)
        {
            if (oldDS == null || newDS == null)
                return null;
            DicomServer ds = (DicomServer)FindDicomServer(_myServerGroup, oldDS.ServerName, oldDS.ServerPath.Split('/'), 1);
            DicomServerGroup dsg = RemoveDicomServer(ds);
            if (dsg == null)
                return null;
            dsg.AddChild(newDS);
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

        private bool DicomServerNameExists(IDicomServer ids, string serverName, string serverPath, bool exclusive, bool recursive)
        {
            if (ids == null || ids.IsServer || serverName == null || serverPath == null)
                return false;
            foreach (IDicomServer cids in ((DicomServerGroup)ids).ChildServers)
            {
                if (cids.ServerName.Equals(serverName) && (!exclusive || !cids.ServerPath.Equals(serverPath)))
                    return true;
                if (!cids.IsServer && recursive && DicomServerNameExists(cids, serverName, serverPath, exclusive, recursive))
                    return true;
            }
            return false;
        }

        private bool DicomServerAEExists(IDicomServer ids, string serverAE, string serverPath, bool exclusive, bool recursive)
        {
            if (ids == null || ids.IsServer || serverAE == null || serverPath == null)
                return false;
            foreach (IDicomServer cids in ((DicomServerGroup)ids).ChildServers)
            {
                if (cids.IsServer && ((DicomServer)cids).DicomAE.AE.Equals(serverAE) && (!exclusive || !cids.ServerPath.Equals(serverPath)))
                    return true;
                if (!cids.IsServer && recursive && DicomServerAEExists(cids, serverAE, serverPath, exclusive, recursive))
                    return true;
            }
            return false;
        }

        private void CheckDefaultServerSettings(bool isupdated)
        {
            string[] svrPaths = new string[] { "." };
            if (isupdated)
            {
                _myServerGroup = new DicomServerGroup();
                LocalAESettings myAESettings = new LocalAESettings();
                _myServerGroup.AddChild(new DicomServer(AENavigatorComponent.MyDatastoreTitle, _myServerGroup.ServerPath, "", "localhost", myAESettings.AETitle, myAESettings.Port));
                _myServerGroup.AddChild(new DicomServerGroup(AENavigatorComponent.MyServersTitle, _myServerGroup.ServerPath));
            }
            else
            {
                if (FindDicomServer(_myServerGroup, AENavigatorComponent.MyDatastoreTitle, svrPaths, 1) == null)
                {
                    LocalAESettings myAESettings = new LocalAESettings();
                    _myServerGroup.AddChild(new DicomServer(AENavigatorComponent.MyDatastoreTitle, ".", "", "localhost", myAESettings.AETitle, myAESettings.Port));
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

        private void GetChildServers(DicomServerGroup dsp, bool recursive)
        {
            foreach (IDicomServer ids in dsp.ChildServers)
            {
                if (ids.IsServer)
                {
                    bool exists = false;
                    foreach (DicomServer ds in _childServers)
                    {
                        if (ids.ServerName.Equals(ds.ServerName) || ((DicomServer)ids).DicomAE.AE.Equals(ds.DicomAE.AE))
                            exists = true;
                    }
                    if (!exists)
                        _childServers.Add((DicomServer)ids);
                }
                else if (recursive)
                {
                    GetChildServers((DicomServerGroup)ids, recursive);
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

        private void LoadDicomServers()
        {
            _myServerGroup = new DicomServerGroup();
            bool isupdated = true;
            if (File.Exists(AENavigatorComponent.MyServersXmlFile))
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

        #endregion
    }
}
