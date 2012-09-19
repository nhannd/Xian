#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.Web.Enterprise.Authentication;

namespace ClearCanvas.ImageViewer.Web.Server.ImageServer
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class ServerDirectoryServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof(IServerDirectory))
                return null;

            return new ImageServerServerDirectory();
        }

        #endregion
    }

    internal class ImageServerServerDirectory : IServerDirectory
    {
        private ServerDirectoryEntry FindLocalPartition(string retrieveAeTitle)
        {
            var webUser = Thread.CurrentPrincipal as CustomPrincipal;
            if (webUser == null)
            {
                Platform.Log(LogLevel.Debug, "ImageServerServerDirectory: FindLocalPartition(): user is not logged in ?");
                return null;
            }

            ServerPartition partition = ServerPartitionMonitor.Instance.GetPartition(retrieveAeTitle);
            if (partition == null)
            {
                Platform.Log(LogLevel.Debug, "ImageServerServerDirectory: FindLocalPartition(): Could not find partition with AE Title '{0}'", retrieveAeTitle);
                return null;
            }

            foreach (var oid in webUser.Credentials.DataAccessAuthorityGroups)
            {
                if (partition.IsAccessAllowed(oid.ToString()))
                {
                    throw new PermissionDeniedException(string.Empty);
                }
            }

            return new ServerDirectoryEntry()
            {
                IsPriorsServer = true,
                Server = new ApplicationEntity()
                {
                    AETitle = partition.AeTitle,
                    Description = partition.Description,
                    Name = partition.AeTitle,
                    ScpParameters = new ScpParameters(WebViewerServices.Default.ArchiveServerHostname, partition.Port),
                    StreamingParameters = new StreamingParameters(WebViewerServices.Default.ArchiveServerHeaderPort, WebViewerServices.Default.ArchiveServerWADOPort)
                }
            };
        }
        private static Device FindServer(string retrieveAeTitle)
        {
         
            var webUser = Thread.CurrentPrincipal as CustomPrincipal;
            if (webUser != null)
            {

                foreach (var partition in ServerPartitionMonitor.Instance)
                {
                    if (partition.AeTitle.Equals(retrieveAeTitle, StringComparison.InvariantCulture))
                    {
                        foreach (var oid in webUser.Credentials.DataAccessAuthorityGroups)
                        {
                            if (partition.IsAccessAllowed(oid.ToString()))
                            {
                                throw new PermissionDeniedException(string.Format("User does not have permission to access partition {0}", partition.AeTitle));
                            }
                        }
                    }
                }
            }

            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                var broker = ctx.GetBroker<IDeviceEntityBroker>();
                var criteria = new DeviceSelectCriteria();
                criteria.AeTitle.EqualTo(retrieveAeTitle);
                IList<Device> list = broker.Find(criteria);
                foreach (Device theDevice in list)
                {
                    if (string.Compare(theDevice.AeTitle, retrieveAeTitle, false, CultureInfo.InvariantCulture) == 0)
                        return theDevice;
                }
            }

            return null;
        }
        private static ServerDirectoryEntry FromDeviceToServerDirectoryEntry(Device theDevice)
        {
            var entry = new ServerDirectoryEntry(new ApplicationEntity
                                                     {
                                                         AETitle = theDevice.AeTitle,
                                                         Description = theDevice.Description,
                                                         Name = theDevice.Name,
                                                         ScpParameters =
                                                             new ScpParameters(theDevice.IpAddress, theDevice.Port)
                                                     });

            return entry;
        }

        public GetServersResult GetServers(GetServersRequest request)
        {
            var result = new GetServersResult
                       {
                           ServerEntries = new List<ServerDirectoryEntry>()
                       };

            if (!string.IsNullOrEmpty(request.AETitle))
            {
                var entry = this.FindLocalPartition(request.AETitle);
                if (entry != null)
                    result.ServerEntries.Add(entry);
                else
                {
                    var device = FindServer(request.AETitle);
                    if (device != null)
                        result.ServerEntries.Add(FromDeviceToServerDirectoryEntry(device));
                }
            }

            return result;
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            return new AddServerResult();
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            return new UpdateServerResult();
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            return new DeleteServerResult();
        }
    }
}
