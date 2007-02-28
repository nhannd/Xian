using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace ClearCanvas.Server.DicomServerShred
{
    [Serializable]
    public class DicomAEServer
    {
        public DicomAEServer()
        {
        }

        public DicomAEServer(String serverName, String serverPath, String serverLocation, String hostName, String aeTitle, int listenPort)
        {
            Name = serverName;
            Path = serverPath;
            Location = serverLocation;
            Host = hostName;
            AETitle = aeTitle;
            Port = listenPort;
        }

        public String Name;
        public String Location;
        public String Path;
        public String Host;
        public String AETitle;
        public int Port;

    }

    [Serializable]
    public class DicomAEGroup
    {
        public DicomAEGroup()
        {
        }

        public DicomAEGroup(String serverGroupName, String serverGroupPath)
        {
            Name = serverGroupName;
            Path = serverGroupPath;
            ChildGroups = new List<DicomAEGroup>();
            ChildServers = new List<DicomAEServer>();
        }

        public DicomAEGroup(String serverGroupName, String serverGroupPath, List<DicomAEGroup> childServerGroups, List<DicomAEServer> childServers)
        {
            Name = serverGroupName;
            Path = serverGroupPath;
            ChildGroups = new List<DicomAEGroup>();
            ChildServers = new List<DicomAEServer>();
            if (childServerGroups != null && childServerGroups.Count > 0)
            {
                foreach (DicomAEGroup dsgs in childServerGroups)
                    ChildGroups.Add(new DicomAEGroup(dsgs.Name, dsgs.Path, dsgs.ChildGroups, dsgs.ChildServers));
            }
            if (childServers != null && childServers.Count > 0)
            {
                foreach (DicomAEServer dss in childServers)
                    ChildServers.Add(new DicomAEServer(dss.Name, dss.Path, dss.Location, dss.Host, dss.AETitle, dss.Port));
            }
        }

        public String Name;
        public String Path;
        public List<DicomAEGroup> ChildGroups;
        public List<DicomAEServer> ChildServers;

    }
}
