#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.Common.ServerTree.LegacyXml
{
    public static class Extensions
    {
        public static DicomServerApplicationEntity ToDataContract(this Server server)
        {
            if (server.IsStreaming)
                return new StreamingServerApplicationEntity(server.AETitle, server.Host, server.Port,
                                                            server.HeaderServicePort, server.WadoServicePort,
                                                            server.NameOfServer, "", server.Location);

            return new DicomServerApplicationEntity(server.AETitle, server.Host, server.Port,
                                                    server.NameOfServer, "", server.Location);
        }

        internal static ServerTreeGroup ToServerTreeGroup(this ServerGroup legacyGroup)
        {
            var group = new ServerTreeGroup(legacyGroup.NameOfGroup);
            foreach (var legacyChildGroup in legacyGroup.ChildGroups)
                group.ChildGroups.Add(legacyChildGroup.ToServerTreeGroup());

            foreach (var legacyChildServer in legacyGroup.ChildServers)
                group.Servers.Add(new ServerTreeDicomServer(legacyChildServer.ToDataContract()));

            return group;
        }
    }
}