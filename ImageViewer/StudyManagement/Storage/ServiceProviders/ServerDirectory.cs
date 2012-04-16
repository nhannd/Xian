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
using Castle.Core.Interceptor;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using Castle.DynamicProxy;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    // TODO (Marmot): This seems like the best place for this, since it has to be available (in process) whenever the database is present.
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class ServerDirectoryServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType != typeof (IServerDirectory))
                return null;

            return new ProxyGenerator().CreateInterfaceProxyWithTargetInterface(
                typeof (IServerDirectory), new ServerDirectory()
                , new IInterceptor[] {new ServerDirectoryFaultInterceptor()});
        }

        #endregion
    }

    internal class ServerDirectoryFaultInterceptor : IInterceptor
    {
        #region IInterceptor Members

        public void Intercept(IInvocation invocation)
        {
            try
            {
                invocation.Proceed();
            }
            catch (ArgumentException e)
            {
                Platform.Log(LogLevel.Error, e);
                if (invocation.Method.Name == "AddServer")
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

        #endregion
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
                    devices = new List<Device> { context.GetDeviceBroker().GetDeviceByName(request.Name) };
                else if (!String.IsNullOrEmpty(request.AETitle))
                    devices = context.GetDeviceBroker().GetDevicesByAETitle(request.AETitle);
                else
                    devices = context.GetDeviceBroker().GetDevices();

                var converted = devices.Select(d => d.ToDataContract()).ToList();
                return new GetServersResult {Servers = converted };
            }
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetDeviceBroker();

                var existing = broker.GetDeviceByName(request.Server.Name);
                if (existing != null)
                    throw new ArgumentException();

                var server = (IApplicationEntity)request.Server;
                var device = server.ToDevice();
                broker.AddDevice(device);

                context.Commit();

                return new AddServerResult { Server = device.ToDataContract() };
            }
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetDeviceBroker();

                var existing = broker.GetDeviceByName(request.Server.Name);
                if (existing == null)
                    throw new ArgumentException();

                var ae = (IApplicationEntity) request.Server;

                existing.AETitle = ae.AETitle;
                existing.HostName = ae.ScpParameters.HostName;
                existing.Port = ae.ScpParameters.Port;
                existing.Location = ae.Location;
                existing.Description = ae.Description;
                    
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

                context.Commit();
                return new UpdateServerResult { Server = existing.ToDataContract() };
            }
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            using (var context = new DataAccessContext())
            {
                var broker = context.GetDeviceBroker();
                var existing = broker.GetDeviceByName(request.Server.Name);
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
