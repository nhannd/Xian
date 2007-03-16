using System;
using System.Collections.Generic;
using System.Text;

namespace ClearCanvas.ImageViewer.Shreds.ServerTree
{
    public static class AENavigatorComponent
    {
        private static String _myServersTitle = "My Servers";
        private static String _myDatastoreTitle = "My Studies";
        private static String _myServersRoot = "MyServersRoot";
        private static String _myServersXmlFile = "DicomAEServers.xml";

        public static string MyServersTitle
        {
            get { return _myServersTitle; }
        }

        public static string MyDatastoreTitle
        {
            get { return _myDatastoreTitle; }
        }

        public static string MyServersRoot
        {
            get { return _myServersRoot; }
        }

        public static string MyServersXmlFile
        {
            get { return _myServersXmlFile; }
        }
    }

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
                foreach (DicomAEGroup dsg in childServerGroups)
                    AddChild(new DicomServerGroup(dsg.Name, dsg.Path, dsg.ChildGroups, dsg.ChildServers));
            }
            if (childServers != null && childServers.Count > 0)
            {
                foreach (DicomAEServer ds in childServers)
                    AddChild(new DicomServer(ds.Name, ds.Path, ds.Location, ds.Host, ds.AETitle, ds.Port));
            }
        }

        public override bool IsServer
        {
            get { return false; }
        }

        public override string ServerDetails
        {
            get { return base.ServerPath + "/" + base.ServerName; }
        }

        public List<IDicomServer> ChildServers
        {
            get
            {
                if (_childServers == null)
                {
                    _childServers = new List<IDicomServer>();
                }
                return _childServers;
            }
        }

        public void AddChild(IDicomServer child)
        {
            ChildServers.Add(child);
            ChildServers.Sort(delegate(IDicomServer s1, IDicomServer s2)
            {
                string s1param = s1.IsServer ? "cc" : "bb"; s1param += s1.ServerName;
                string s2param = s2.IsServer ? "cc" : "bb"; s2param += s2.ServerName;
                return s1param.CompareTo(s2param);
            });
        }

        private List<IDicomServer> _childServers;
    }
}
