using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml.Serialization;
using ClearCanvas.Dicom.Services;

namespace ClearCanvas.ImageViewer.Explorer.Dicom
{
    public class DicomServerService
    {
        public static DicomServerGroup LoadDicomServers()
        {
            DicomServerGroup dsg = new DicomServerGroup(); 
            bool isupdated = true;
            if (File.Exists("MyDICOMServerSettings.xml"))
            {
                Stream fStream = File.OpenRead("MyDICOMServerSettings.xml");
                XmlSerializer xmlFormat = new XmlSerializer(typeof(DicomServerGroupSettings), new Type[] { typeof(List<DicomServerGroupSettings>), typeof(DicomServerGroupSettings), typeof(List<DicomServerSettings>), typeof(DicomServerSettings) });
                DicomServerGroupSettings dsgs = (DicomServerGroupSettings)xmlFormat.Deserialize(fStream);
                if (dsgs != null)
                {
                    dsg = new DicomServerGroup(dsgs.ServerGroupName, dsgs.ServerGroupPath, dsgs._childServerGroups, dsgs._childServers);
                    isupdated = false;
                }
                fStream.Close();
            }

            //check the default server nodes
            dsg = CheckDefaultServerSettings(dsg, isupdated);
            
            return dsg;
        }

        private static DicomServerGroup CheckDefaultServerSettings(DicomServerGroup dsg, bool isupdated)
        {
            if (isupdated)
            {
                dsg = new DicomServerGroup();
                LocalAESettings myAESettings = new LocalAESettings();
                dsg.AddChild(new DicomServer(AENavigatorComponent.MyDatastoreTitle, dsg.ServerPath, "", "localhost", myAESettings.AETitle, myAESettings.Port));
                dsg.AddChild(new DicomServerGroup(AENavigatorComponent.MyServersTitle, dsg.ServerPath));
            }
            else
            {
                if (FindDicomServer(dsg, AENavigatorComponent.MyDatastoreTitle, ".", 0) == null)
                {
                    LocalAESettings myAESettings = new LocalAESettings();
                    dsg.AddChild(new DicomServer(AENavigatorComponent.MyDatastoreTitle, ".", "", "localhost", myAESettings.AETitle, myAESettings.Port));
                    isupdated = true;
                }
                if (FindDicomServer(dsg, AENavigatorComponent.MyServersTitle, ".", 0) == null)
                {
                    dsg.AddChild(new DicomServerGroup(AENavigatorComponent.MyServersTitle, "."));
                    isupdated = true;
                }
            }

            if (isupdated)
                SaveDicomServers(dsg);
            return dsg;
        }

        public static IDicomServer FindDicomServer(IDicomServer idsp, string serverName, string serverPath, int depth)
        {
            if (idsp == null || idsp.IsServer || serverName == null || serverName.Equals("") 
                || serverPath == null || serverPath.Equals("") || depth < 0)
                return null;
            string[] svrPaths = serverPath.Split('/');
            if (svrPaths == null || svrPaths.Length <= depth)
                return null;
            DicomServerGroup dsg = (DicomServerGroup)idsp;
            foreach (IDicomServer ids in dsg.ChildServers)
            {
                if (ids.ServerName.Equals(serverName))
                {
                    if (ids.IsServer || svrPaths.Length == (depth + 1))
                        return ids;
                    return FindDicomServer(ids, serverName, serverPath, depth + 1);
                }
            }
            return null;
        }

        public static void SaveDicomServers(DicomServerGroup dsg)
        {
            DicomServerGroupSettings dsgs = ConvertDicomServers(dsg);
            XmlSerializer xmlFormat = new XmlSerializer(typeof(DicomServerGroupSettings), new Type[] { typeof(List<DicomServerGroupSettings>), typeof(DicomServerGroupSettings), typeof(List<DicomServerSettings>), typeof(DicomServerSettings)});
            Stream fStream = new FileStream("MyDICOMServerSettings.xml", FileMode.Create, FileAccess.Write, FileShare.None);
            xmlFormat.Serialize(fStream, dsgs);
            fStream.Close();
            return;
        }

        private static DicomServerGroupSettings ConvertDicomServers(DicomServerGroup dsg)
        {
            if (dsg == null)
                return null;

            List<DicomServerGroupSettings> chdGroup = new List<DicomServerGroupSettings>();
            List<DicomServerSettings> chdServer = new List<DicomServerSettings>();
            if (dsg.ChildServers != null && dsg.ChildServers.Count > 0)
            {
                foreach (IDicomServer ids in dsg.ChildServers)
                {
                    if (ids.IsServer)
                    {
                        DicomServer cds = (DicomServer)ids;
                        chdServer.Add(new DicomServerSettings(cds.ServerName, cds.ServerPath, cds.ServerLocation, cds.DicomAE.Host, cds.DicomAE.AE, cds.DicomAE.Port));
                    }
                    else
                    {
                        DicomServerGroup cdsg = (DicomServerGroup)ids;
                        chdGroup.Add(ConvertDicomServers(cdsg));
                    }
                }
            }
            DicomServerGroupSettings dsgs = new DicomServerGroupSettings(dsg.ServerName, dsg.ServerPath, chdGroup, chdServer);
            return dsgs;
        }

    }
}
