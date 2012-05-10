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
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using ClearCanvas.ImageViewer.StudyManagement.Core.Storage;

namespace ClearCanvas.ImageViewer.StudyManagement.Core.ServiceProviders
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class ServerDirectoryServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof (IServerDirectory))
                return null;

            return new ServerDirectoryProxy();
        }

        #endregion
    }

    internal class ServerDirectoryProxy : IServerDirectory
    {
        private readonly IServerDirectory _real;

        public ServerDirectoryProxy()
        {
            _real = new ServerDirectory();
        }

        #region IServerDirectory Members

        public GetServersResult GetServers(GetServersRequest request)
        {
            return Call(_real.GetServers, request);
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            return Call(_real.AddServer, request);
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            return Call(_real.UpdateServer, request);
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            return Call(_real.DeleteServer, request);
        }

        #endregion

        private TResult Call<TInput, TResult>(Func<TInput, TResult> function, TInput input)
        {
            try
            {
                return function(input);
            }
            catch (ArgumentException e)
            {
                Platform.Log(LogLevel.Error, e);
                if (function.Method.Name == "AddServer")
                    throw new FaultException<ServerExistsFault>(new ServerExistsFault());

                throw new FaultException<ServerNotFoundFault>(new ServerNotFoundFault());
            }
            catch (FaultException)
            {
                throw;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                throw new FaultException();
            }
        }
    }

    internal class ServerDirectory : IServerDirectory
    {
        #region Implementation of IServerDirectory

        public GetServersResult GetServers(GetServersRequest request)
        {
            using (var context = new DataAccessContext())
            {
                List<Device> devices;
                if (!String.IsNullOrEmpty(request.Name))
                {
                    var device = context.GetDeviceBroker().GetDeviceByName(request.Name);
                    devices = new List<Device> ();
                    if (device != null) devices.Add(device);
                }
                else if (!String.IsNullOrEmpty(request.AETitle))
                    devices = context.GetDeviceBroker().GetDevicesByAETitle(request.AETitle);
                else
                    devices = context.GetDeviceBroker().GetDevices();

                var converted = devices.Select(d => d.ToDataContract()).ToList();
                return new GetServersResult {ServerEntries = converted };
            }
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetDeviceBroker();

                var existing = broker.GetDeviceByName(request.ServerEntry.Server.Name);
                if (existing != null)
                    throw new ArgumentException();

                var device = request.ServerEntry.ToDevice();
                broker.AddDevice(device);

                context.Commit();

                return new AddServerResult { ServerEntry = device.ToDataContract() };
            }
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetDeviceBroker();

                var existing = broker.GetDeviceByName(request.ServerEntry.Server.Name);
                if (existing == null)
                    throw new ArgumentException();

                var ae = request.ServerEntry.Server;

                existing.AETitle = ae.AETitle;
                existing.HostName = ae.ScpParameters.HostName;
                existing.Port = ae.ScpParameters.Port;
                existing.Location = ae.Location;
                existing.Description = ae.Description;
                existing.IsPriorsServer = request.ServerEntry.IsPriorsServer;

                if (ae.StreamingParameters != null)
                {
                    existing.StreamingHeaderPort = ae.StreamingParameters.HeaderServicePort;
                    existing.StreamingImagePort = ae.StreamingParameters.WadoServicePort;
                }
                else
                {
                    existing.StreamingHeaderPort = null;
                    existing.StreamingImagePort = null;
                }

                existing.ExtensionData = request.ServerEntry.Data == null 
                                            ? null : Serializer.SerializeServerExtensionData(request.ServerEntry.Data);
                context.Commit();
                return new UpdateServerResult { ServerEntry = existing.ToDataContract() };
            }
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetDeviceBroker();
                var existing = broker.GetDeviceByName(request.ServerEntry.Server.Name);
                if (existing == null)
                    throw new ArgumentException();
                    
                broker.DeleteDevice(existing);

                context.Commit();
                return new DeleteServerResult();
            }
        }

        public DeleteAllServersResult DeleteAllServers(DeleteAllServersRequest request)
        {
            using (var scope = new DataAccessContext())
            {
                var broker = scope.GetDeviceBroker();
                broker.DeleteAllDevices();
                scope.Commit();
                return new DeleteAllServersResult();
            }
        }

        #endregion
    }
}
