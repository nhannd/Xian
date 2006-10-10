using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class DicomServerGroup : DicomServerBase
    {
        public DicomServerGroup()
        {
            base.ServerName = AENavigatorComponent.MyServersRoot;
            base.ServerPath = ".";
        }

        public DicomServerGroup(String serverGroupName, String serverGroupPath)
        {
            base.ServerName = serverGroupName;
            base.ServerPath = serverGroupPath;
        }

        public DicomServerGroup(String serverGroupName, String serverGroupPath, List<DicomServerGroupSettings> childServerGroups, List<DicomServerSettings> childServers)
        {
            base.ServerName = serverGroupName;
            base.ServerPath = serverGroupPath;
            if (childServerGroups != null && childServerGroups.Count > 0)
            {
                foreach(DicomServerGroupSettings dsg in childServerGroups)
                    base.ChildServers.Add(new DicomServerGroup(dsg.ServerGroupName, dsg.ServerGroupPath, dsg._childServerGroups, dsg._childServers));
            }
            if (childServers != null && childServers.Count > 0)
            {
                foreach (DicomServerSettings ds in childServers)
                    base.ChildServers.Add(new DicomServer(ds.ServerName, ds.ServerPath, ds.ServerLocation, ds.HostName, ds.AeTitle, ds.ListenPort));
            }
        }

        public override bool IsServer
        {
            get
            {
                return false;
            }
        }

        public override string ServerDetails
        {
            get
            {
                return base.ServerPath + "/" + base.ServerName;
            }
        }

    }
}
