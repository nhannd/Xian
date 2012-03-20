using System;
using System.Linq;
using System.ServiceModel;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.ImageViewer.Common.ServerDirectory;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage.ServiceProviders
{
    [ExtensionOf(typeof(ServiceProviderExtensionPoint))]
    internal class ServerDirectoryServiceProvider : IServiceProvider
    {
        #region IServiceProvider Members

        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(IServerDirectory))
                return new ServerDirectory();

            return null;
        }

        #endregion
    }

    internal class ServerDirectory : IServerDirectory
    {
        #region Implementation of IServerDirectory

        public GetServersResult GetServers(GetServersRequest request)
        {
            try
            {
                using (var scope = new DataAccessScope())
                {
                    var devices = scope.GetDeviceBroker().GetDevices();
                    var converted = devices.Select(d => d.ToServerDirectoryEntry()).ToList();
                    return new GetServersResult {DirectoryEntries = converted };
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                // TODO (Marmot): Implement fault contracts.
                throw new FaultException();
            }
        }

        public AddServerResult AddServer(AddServerRequest request)
        {
            try
            {
                using (var scope = new DataAccessScope())
                {
                    var server = (IDicomServerApplicationEntity) request.Server;
                    var device = server.ToDevice();
                    scope.GetDeviceBroker().AddDevice(device);
                    return new AddServerResult {DirectoryEntry = device.ToServerDirectoryEntry()};
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e);
                // TODO (Marmot): Implement fault contracts.
                throw new FaultException();
            }
        }

        public UpdateServerResult UpdateServer(UpdateServerRequest request)
        {
            try
            {
                using (var scope = new DataAccessScope())
                {
                    var broker = scope.GetDeviceBroker();

                    var existing = broker.GetDeviceByOid(request.DirectoryEntry.Oid);
                    if (existing == null)
                        throw new ArgumentException();

                    var ae = (IDicomServerApplicationEntity) request.DirectoryEntry.Server;

                    // TODO (Marmot): Don't use the name.
                    existing.Name = ae.Name;
                    
                    existing.Port = ae.Port;
                    existing.Location = ae.Location;
                    if (ae.IsStreaming)
                    {
                        var streaming = (IStreamingServerApplicationEntity) ae;
                        existing.StreamingHeaderPort = streaming.HeaderServicePort;
                        existing.StreamingImagePort = streaming.WadoServicePort;
                    }

                    scope.SubmitChanges();
                    return new UpdateServerResult {DirectoryEntry = existing.ToServerDirectoryEntry()};
                }
            }
            catch (Exception e)
            {
                // TODO (Marmot): Implement fault contracts.
                Platform.Log(LogLevel.Error, e);
                throw new FaultException();
            }
        }

        public DeleteServerResult DeleteServer(DeleteServerRequest request)
        {
            try
            {
                using (var scope = new DataAccessScope())
                {
                    var broker = scope.GetDeviceBroker();
                    var existing = broker.GetDeviceByOid(request.DirectoryEntry.Oid);
                    if (existing == null) // TODO (Marmot): Better exceptions.
                        throw new ArgumentException();
                    
                    broker.DeleteDevice(existing);
                    
                    scope.SubmitChanges();
                    return new DeleteServerResult();
                }
            }
            catch (Exception e)
            {
                // TODO (Marmot): Implement fault contracts.
                Platform.Log(LogLevel.Error, e);
                throw new FaultException();
            }
        }

        #endregion
    }
}
