#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using System.Collections.Generic;

namespace ClearCanvas.ImageViewer.Common.ServerTree
{
    public static class Extensions
    {
        public static DicomServerApplicationEntity ToDataContract(this IServerTreeDicomServer server)
        {
            Platform.CheckForNullReference(server, "server");
            if (server.IsStreaming)
                return new StreamingServerApplicationEntity(server.AETitle, server.HostName, server.Port,
                                                            server.HeaderServicePort, server.WadoServicePort,
                                                            server.Name, "", server.Location);

            return new DicomServerApplicationEntity(server.AETitle, server.HostName, server.Port, server.Name, "", server.Location);
        }

        public static IDicomServiceNode ToDicomServiceNode(this IServerTreeDicomServer server)
        {
            Platform.CheckForNullReference(server, "server");
            var contract = server.ToDataContract();
            return contract.ToServiceNode();
        }

        public static StoredServerGroup ToStoredServerGroup(this IServerTreeGroup serverTreeGroup)
        {
            var storedServerGroup = new StoredServerGroup(serverTreeGroup.Name);
            foreach (var childGroup in serverTreeGroup.ChildGroups)
                storedServerGroup.ChildGroups.Add(childGroup.ToStoredServerGroup());

            foreach (var childServer in serverTreeGroup.Servers)
                storedServerGroup.DirectoryServerReferences.Add(new DirectoryServerReference { Name = childServer.Name });

            return storedServerGroup;
        }

        internal static ServerTreeGroup ToServerTreeGroup(this StoredServerGroup storedServerGroup, List<DicomServerApplicationEntity> servers)
        {
            var serverTreeGroup = new ServerTreeGroup(storedServerGroup.Name);
            foreach (var childGroup in storedServerGroup.ChildGroups)
                serverTreeGroup.ChildGroups.Add(childGroup.ToServerTreeGroup(servers));

            foreach (var directoryServerReference in storedServerGroup.DirectoryServerReferences)
            {
                var reference = directoryServerReference;
                var foundServerIndex = servers.FindIndex(d => d.Name == reference.Name);
                if (foundServerIndex >= 0)
                {
                    var server = (IDicomServerApplicationEntity)servers[foundServerIndex];
                    servers.RemoveAt(foundServerIndex);
                    serverTreeGroup.Servers.Add(new ServerTreeDicomServer(server));
                }
            }

            return serverTreeGroup;
        }
    }
}
