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

        public DicomServerGroup(String serverGroupName, String serverGroupPath, List<DicomAEGroup> childServerGroups, List<DicomAEServer> childServers)
        {
            base.ServerName = serverGroupName;
            base.ServerPath = serverGroupPath;
            if (childServerGroups != null && childServerGroups.Count > 0)
            {
                foreach(DicomAEGroup dsg in childServerGroups)
                    base.ChildServers.Add(new DicomServerGroup(dsg.Name, dsg.Path, dsg.ChildGroups, dsg.ChildServers));
            }
            if (childServers != null && childServers.Count > 0)
            {
                foreach (DicomAEServer ds in childServers)
                    base.ChildServers.Add(new DicomServer(ds.Name, ds.Path, ds.Location, ds.Host, ds.AETitle, ds.Port));
            }
        }

        public override bool IsServer
        {
            get { return false; }
        }

        public override string ServerDetails
        {
            get { return base.GroupID; }
        }

    }
}
