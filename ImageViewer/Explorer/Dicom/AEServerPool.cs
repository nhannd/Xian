using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class AEServerPool
    {
        private AEServer _currentserver;
        private List<AEServer> _serverlist;
        private int _currentserverid;

        public void SaveServerSettings()
        {
            List<AEServerSettings> _settinglist = new List<AEServerSettings>();
            if (_serverlist == null)
                _serverlist = new List<AEServer>();
            foreach (AEServer aeserver in _serverlist)
            {
                _settinglist.Add(new AEServerSettings(aeserver.Servername, aeserver.Serverpath, aeserver.Description, aeserver.Host, aeserver.AE, aeserver.Port));
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
                _serverlist.Add(new AEServer("MyAEServer", "/ServerGroup/", "", "111.1.1.1", "MyAETitle", 100));
                SaveServerSettings();
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

        public void SetNewServer(string svrName)
        {
            _currentserver = null;
            _currentserverid = -1;
            if (svrName == null || svrName.Equals("") || _serverlist == null)
                return;

            _currentserver = new AEServer(svrName, "/ServerGroup/", "", "1.1.1.1", "AETitle", 100);
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

    }
}
