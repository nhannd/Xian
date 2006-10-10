using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    [Serializable]
    public class DicomServerSettings
    {
        public DicomServerSettings()
        {
        }

        public DicomServerSettings(String serverName, String serverPath, String serverLocation, String hostName, String aeTitle, int listenPort)
        {
            _serverName = serverName;
            _serverPath = serverPath;
            _serverLocation = serverLocation;
            _hostName = hostName;
            _aeTitle = aeTitle;
            _listenPort = listenPort;
        }

        public String ServerName
        {
            get { return _serverName; }
        }

        public String ServerLocation
        {
            get { return _serverLocation; }
        }

        public String HostName
        {
            get { return _hostName; }
        }

        public int ListenPort
        {
            get { return _listenPort; }
        }

        public String AeTitle
        {
            get { return _aeTitle; }
        }


        public String ServerPath
        {
            get { return _serverPath; }
        }

        public String _serverName;
        public String _serverLocation;
        public String _serverPath;
        public String _hostName;
        public String _aeTitle;
        public int _listenPort;

    }

    [Serializable]
    public class DicomServerGroupSettings
    {
        public DicomServerGroupSettings()
        {
        }

        public DicomServerGroupSettings(String serverGroupName, String serverGroupPath)
        {
            _serverGroupName = serverGroupName;
            _serverGroupPath = serverGroupPath;
            _childServerGroups = new List<DicomServerGroupSettings>();
            _childServers = new List<DicomServerSettings>();
        }

        public DicomServerGroupSettings(String serverGroupName, String serverGroupPath, List<DicomServerGroupSettings> childServerGroups, List<DicomServerSettings> childServers)
        {
            _serverGroupName = serverGroupName;
            _serverGroupPath = serverGroupPath;
            _childServerGroups = new List<DicomServerGroupSettings>();
            _childServers = new List<DicomServerSettings>();
            if (childServerGroups != null && childServerGroups.Count > 0)
            {
                foreach(DicomServerGroupSettings dsgs in childServerGroups)
                    _childServerGroups.Add(new DicomServerGroupSettings(dsgs.ServerGroupName, dsgs.ServerGroupPath, dsgs._childServerGroups, dsgs._childServers));
            }
            if (childServers != null && childServers.Count > 0)
            {
                foreach (DicomServerSettings dss in childServers)
                    _childServers.Add(new DicomServerSettings(dss.ServerName, dss.ServerPath, dss.ServerLocation, dss.HostName, dss.AeTitle, dss.ListenPort));
            }
        }

        public String ServerGroupName
        {
            get { return _serverGroupName; }
        }

        public String ServerGroupPath
        {
            get { return _serverGroupPath; }
        }

/*        protected List<DicomServerGroupSettings> ChildServerGroups 
        {
            get { return _childServerGroups; }
        }

        protected List<DicomServerSettings> ChildServers
        {
            get { return _childServers; }
        }
*/
        public String _serverGroupName;
        public String _serverGroupPath;
        public List<DicomServerGroupSettings> _childServerGroups;
        public List<DicomServerSettings> _childServers;

    }
}
