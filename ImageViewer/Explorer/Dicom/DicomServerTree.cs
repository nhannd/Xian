using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
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
            for(int i = 0; i < dsg.ChildServers.Count; i++) 
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
}
