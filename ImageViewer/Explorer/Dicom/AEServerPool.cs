using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class AEServerPool
    {
        #region Fields

        private AEServer _currentserver;
        private List<AEServer> _serverlist;
        private List<AEServer> _childServers;
        private int _currentserverid;

        public AEServer Currentserver
        {
            get { return _currentserver; }
            set { _currentserver = value; }
        }

        public List<AEServer> Serverlist
        {
            get
            {
                if (_serverlist == null)
                    LoadServerSettings();
                return _serverlist;
            }
            set { _serverlist = value; }
        }

        public int Currentserverid
        {
            get
            {
                if (_currentserver == null)
                    _currentserverid = -1;
                return _currentserverid;
            }
            set { _currentserverid = value; }
        }

        public List<AEServer> ChildServers
        {
            get { return _childServers; }
            set { _childServers = value; }
        }

        #endregion

        public AEServerPool()
        {
            _currentserver = null;
            _serverlist = null;
            _childServers = null;
            _currentserverid = -1;
        }

        public void SaveServerSettings()
        {
            List<AEServerSettings> _settinglist = new List<AEServerSettings>();
            if (_serverlist == null)
                _serverlist = new List<AEServer>();
            ComposeServerPath();
            foreach (AEServer aeserver in _serverlist)
            {
                _settinglist.Add(new AEServerSettings(aeserver.Servername, aeserver.Serverpath, aeserver.Serverlocation, aeserver.Host, aeserver.AE, aeserver.Port));
            }
            XmlSerializer xmlFormat = new XmlSerializer(typeof(List<AEServerSettings>), new Type[] { typeof(AEServerSettings) });
            Stream fStream = new FileStream("DICOMServerSettings.xml", FileMode.Create, FileAccess.Write, FileShare.None);
            xmlFormat.Serialize(fStream, _settinglist);
            fStream.Close();
            
        }

        public void LoadServerSettings()
        {
            if (_serverlist != null)
                return;
            _serverlist = new List<AEServer>();
            List<AEServerSettings> _settinglist = new List<AEServerSettings>();
            if (File.Exists("DICOMServerSettings.xml"))
            {
                Stream fStream = File.OpenRead("DICOMServerSettings.xml");
                XmlSerializer xmlFormat = new XmlSerializer(typeof(List<AEServerSettings>), new Type[] { typeof(AEServerSettings) });
                _settinglist = (List<AEServerSettings>)xmlFormat.Deserialize(fStream);
                foreach (AEServerSettings svrsettings in _settinglist)
                {
                    _serverlist.Add(new AEServer(svrsettings.Servername, svrsettings.Serverpath, svrsettings.Description, svrsettings.Hostname, svrsettings.AeTitle, svrsettings.ListenPort));
                }
                fStream.Close();
            }
            else
            {
                LocalAESettings myAESettings = new LocalAESettings();
                _serverlist.Add(new AEServer(AENavigatorComponent.EmptyNodeName, "/" + AENavigatorComponent.MyDatastoreTitle, "", "Host", "AeTitle", 100));
                _serverlist.Add(new AEServer(AENavigatorComponent.MyDatastoreTitle, "/" + AENavigatorComponent.MyDatastoreTitle, "", "localhost", myAESettings.AETitle, myAESettings.Port));
                _serverlist.Add(new AEServer(AENavigatorComponent.EmptyNodeName, "/" + AENavigatorComponent.MyServersTitle, "", "Host0", "AeTitle0", 100));
                _serverlist.Add(new AEServer(AENavigatorComponent.NewServerName, "/" + AENavigatorComponent.MyServersTitle, "", "Host", "AeTitle", 100));
                SaveServerSettings();
            }
            //check the default server nodes
            CheckDefaultServerSettings();
        }

        private void CheckDefaultServerSettings()
        {
            if (_serverlist == null)
                return;
            //check the default server nodes
            bool ds_exists = false;
            bool dsroot_exists = false;
            bool svrroot_exists = false;
            bool isupdated = false;
            for (int i = 0; i < _serverlist.Count; i++)
            {
                AEServer ae = _serverlist[i];
                if (ae.Servername.Equals(AENavigatorComponent.EmptyNodeName) && ae.Serverpath.Equals("/" + AENavigatorComponent.MyDatastoreTitle))
                    dsroot_exists = true;
                else if (ae.Servername.Equals(AENavigatorComponent.EmptyNodeName) && ae.Serverpath.Equals("/" + AENavigatorComponent.MyServersTitle))
                    svrroot_exists = true;
                else if (ae.Servername.Equals(AENavigatorComponent.MyDatastoreTitle) && ae.Serverpath.Equals("/" + AENavigatorComponent.MyDatastoreTitle))
                    ds_exists = true;
                if (!ae.getServerRootName().Equals(AENavigatorComponent.MyDatastoreTitle) && !ae.getServerRootName().Equals(AENavigatorComponent.MyServersTitle))
                {
                    _serverlist[i].Serverpath = "/" + AENavigatorComponent.MyServersTitle + _serverlist[i].Serverpath;
                    _serverlist[i].BuildServerPathGroup();
                    isupdated = true;
                }
            }
            if (!dsroot_exists)
                _serverlist.Add(new AEServer(AENavigatorComponent.EmptyNodeName, "/" + AENavigatorComponent.MyDatastoreTitle, "", "Host", "AeTitle", 100));
            if (!svrroot_exists)
                _serverlist.Add(new AEServer(AENavigatorComponent.EmptyNodeName, "/" + AENavigatorComponent.MyServersTitle, "", "Host0", "AeTitle0", 100));
            if (!ds_exists)
            {
                LocalAESettings myAESettings = new LocalAESettings();
                _serverlist.Add(new AEServer(AENavigatorComponent.MyDatastoreTitle, "/" + AENavigatorComponent.MyDatastoreTitle, "", "localhost", myAESettings.AETitle, myAESettings.Port));
            }

            for (int i = 0; i < _serverlist.Count; i++)
            {
                AEServer ae = _serverlist[i];
                ae.ServerID = i;
                ae.ServerParentID = -1;
                String[] pgroup = ae.ServerPathGroups;
                int k = 0;
                if (ae.Servername.Equals(AENavigatorComponent.EmptyNodeName))
                {
                    k = 1;
                }
                for (int j = 0; j < _serverlist.Count; j++)
                {
                    if (i == j || !_serverlist[j].Servername.Equals(AENavigatorComponent.EmptyNodeName))
                        continue;

                    String[] p2group = _serverlist[j].ServerPathGroups;
                    if (pgroup.Length != (p2group.Length + k))
                        continue;
                    bool samepath = true;
                    for (int m = 0; m < pgroup.Length-k; m++)
                    {
                        if (!pgroup[m].Equals(p2group[m]))
                        {
                            samepath = false;
                            break;
                        }
                    }
                    if (!samepath)
                        continue;
                    ae.ServerParentID = j;
                    break;
                }
            }

            if (!dsroot_exists || !svrroot_exists || !ds_exists || !isupdated)
            {
                SaveServerSettings();
            }
        }

        private void ComposeServerPath()
        {
            if (_serverlist == null)
                return;

            for (int i = 0; i < _serverlist.Count; i++)
            {
                AEServer ae = _serverlist[i];
                String svrpath = "";
                String[] pgroup = ae.ServerPathGroups;
                for (int m = 0; m < pgroup.Length; m++)
                {
                    if (!pgroup[m].Equals(""))
                    {
                        svrpath += "/" + pgroup[m];
                    }
                }
                ae.Serverpath = svrpath;
            }

        }

        public void SetCurrentServerByName(string svrName)
        {
            _currentserver = null;
            _currentserverid = -1;
            if (svrName == null || svrName.Equals("") || _serverlist == null)
                return;

            for (int i = 0; i < _serverlist.Count; i++)
            {
                if (!_serverlist[i].Servername.Equals(svrName))
                    continue;
                _currentserver = _serverlist[i];
                _currentserverid = i;
                break;
            }
        }

        public int AddNewServer(int parentid)
        {
            _currentserver = null;
            _currentserverid = -1;
            _currentserver = new AEServer(AENavigatorComponent.NewServerName, "/" + AENavigatorComponent.MyServersTitle, "", "1", "AETitle", 100);
            _currentserverid = _serverlist.Count;
            _currentserver.ServerID = _serverlist.Count;
            _currentserver.ServerParentID = parentid;
            _serverlist.Add(_currentserver);
            SaveServerSettings();
            return _serverlist.Count - 1;
        }

        public List<AEServer> GetServerRoots()
        {
            _childServers = new List<AEServer>();
            for (int i = 0; i < _serverlist.Count; i++)
            {
                if (_serverlist[i].getServerRootName().Equals(AENavigatorComponent.MyDatastoreTitle) && !_serverlist[i].Servername.Equals(AENavigatorComponent.EmptyNodeName))
                {
                    _childServers.Add(_serverlist[i]);
                    break;
                }
            }
            for (int i = 0; i < _serverlist.Count; i++)
            {
                if (_serverlist[i].ServerParentID == -1 && _serverlist[i].getServerRootName().Equals(AENavigatorComponent.MyServersTitle))
                {
                    _childServers.Add(_serverlist[i]);
                    break;
                }
            }
            return _childServers;
        }

        public List<AEServer> GetChildServers(int serverid, bool serveronly, bool recursive) 
        {
            _childServers = new List<AEServer>();
            FindChildServers(serverid, serveronly, recursive);
            return _childServers;
        }

        public void FindChildServers(int serverid, bool serveronly, bool recursive)
        {
            if (_serverlist == null || serverid < 0 || serverid >= _serverlist.Count
                || !_serverlist[serverid].Servername.Equals(AENavigatorComponent.EmptyNodeName))
                return;
            String gname = _serverlist[serverid].getServerGroupName();
            String[] pgroup = _serverlist[serverid].ServerPathGroups;
            for (int i = 0; i < _serverlist.Count; i++)
            {
                if (_serverlist[i].ServerParentID != serverid)
                    continue;
                if (!_serverlist[i].Servername.Equals(AENavigatorComponent.EmptyNodeName) || !serveronly)
                    _childServers.Add(_serverlist[i]);
                if (recursive)
                {
                    FindChildServers(i, serveronly, recursive);
                }
            }
            return;
        }

    }
}
