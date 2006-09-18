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
        //private AEServer _mydatastore;
        private List<AEServer> _serverlist;
        private List<AEServer> _currentgroup;
        private int _currentserverid;
        private List<int> _parentids;
        private string _currentgroupname;

        public AEServer Currentserver
        {
            get { return _currentserver; }
            set { _currentserver = value; }
        }

        public List<AEServer> Serverlist
        {
            get
            {
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

        public List<AEServer> Currentgroup
        {
            get { return _currentgroup; }
            set { _currentgroup = value; }
        }

        public string Currentgroupname
        {
            get { return _currentgroupname; }
            set { _currentgroupname = value; }
        }

        //public AEServer Mydatastore
        //{
        //    get { return _mydatastore; }
        //    set { _mydatastore = value; }
        //}

        #endregion

        public void SaveServerSettings()
        {
            List<AEServerSettings> _settinglist = new List<AEServerSettings>();
            if (_serverlist == null)
                _serverlist = new List<AEServer>();
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
            _parentids = new List<int>();
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
                //check the default server nodes
                bool ds_exists = false;
                bool dsroot_exists = false;
                bool svrroot_exists = false;
                for (int i = 0; i < _serverlist.Count; i++)
                {
                    if (_serverlist[i].Servername.Equals(AENavigatorComponent.EmptyNodeName) && _serverlist[i].Serverpath.Equals("/" + AENavigatorComponent.MyDatastoreTitle))
                        dsroot_exists = true;
                    else if (_serverlist[i].Servername.Equals(AENavigatorComponent.EmptyNodeName) && _serverlist[i].Serverpath.Equals("/" + AENavigatorComponent.MyServersTitle))
                        svrroot_exists = true;
                    else if (_serverlist[i].Servername.Equals(AENavigatorComponent.MyDatastoreTitle) && _serverlist[i].Serverpath.Equals("/" + AENavigatorComponent.MyDatastoreTitle))
                        ds_exists = true;
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
                if (!dsroot_exists || !svrroot_exists || !ds_exists)
                    SaveServerSettings();
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
            LoadParentID(-1);
        }

        public void LoadParentID(int serverid)
        {
            if (_serverlist == null || _parentids == null
                || serverid < -1 || serverid >= _serverlist.Count)
                return;

            string[][] serverPaths = new string[_serverlist.Count][];
            for (int i = 0; i < _serverlist.Count; i++)
            {
                serverPaths[i] = _serverlist[i].getServerPathGroup();
            }

            if (serverid >= 0)
            {
                return;
            }

            _parentids.Clear();
            for (int i = 0; i < _serverlist.Count; i++)
            {
                _parentids.Add(FindParentID(i, serverPaths));
            }
        }

        public int FindParentID(int serverid, string[][] serverPaths)
        {
            int myparent = -1;
            if (_serverlist == null || _parentids == null || _parentids.Count < _serverlist.Count
                || serverid < 0 || serverid >= _serverlist.Count)
                return myparent; 

            if (serverPaths == null || serverPaths.Length < _serverlist.Count)
            {
                serverPaths = new string[_serverlist.Count][];
                for (int i = 0; i < _serverlist.Count; i++)
                {
                    serverPaths[i] = _serverlist[i].getServerPathGroup();
                }
            }

            if (serverPaths[serverid] == null || serverPaths[serverid].Length <= 0
                || (_serverlist[serverid].Servername.Equals(AENavigatorComponent.EmptyNodeName) && serverPaths[serverid].Length == 1))
            {
                return myparent;
            }
            int m = 0;
            if (_serverlist[serverid].Servername.Equals(AENavigatorComponent.EmptyNodeName))
            {
                m = 1;
            }
            for (int j = 0; j < _serverlist.Count; j++)
            {
                if (serverid == j || serverPaths[j] == null || serverPaths[j].Length <= 0
                    || serverPaths[serverid].Length != (serverPaths[j].Length + m)
                    || !_serverlist[j].Servername.Equals(AENavigatorComponent.EmptyNodeName))
                    continue;
                bool hasparent = true;
                for (int k = serverPaths[serverid].Length - 1 - m; k >= 0; k--)
                {
                    if (!serverPaths[serverid][k].Equals(serverPaths[j][k]))
                    {
                        hasparent = false;
                        break;
                    }
                }
                if (hasparent)
                {
                    myparent = j;
                    break;
                }
            }
            return myparent; 
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

        public void SetNewServer(string svrName, string tstamp)
        {
            _currentserver = null;
            _currentserverid = -1;
            if (svrName == null || svrName.Equals("") || _serverlist == null || tstamp == null)
                return;

            _currentserver = new AEServer(svrName + tstamp, "/ServerGroup/", "", "1.1.1." + tstamp, "AETitle" + tstamp, 100);
            _currentserverid = _serverlist.Count;
            _serverlist.Add(_currentserver);
            SaveServerSettings();
        }

        public List<AEServer> GetChildServers(int n) 
        {
            List<AEServer> childList = new List<AEServer>();
            if (_serverlist == null || n < 0 || n >= _serverlist.Count)
                return childList;
            String serverPath = _serverlist[n].Serverpath;
            for (int i = 0; i < _serverlist.Count; i++)
            {
                if (i == n || !_serverlist[i].Serverpath.StartsWith(serverPath))
                    continue;
                childList.Add(_serverlist[i]);
            }
            return childList;
        }

    }
}
