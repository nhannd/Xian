using System.Collections.Generic;
using System.Linq;
using System.Net;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;
using ClearCanvas.ImageViewer.Common.ServerDirectory;
using System;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    public static class DeviceExtensions
    {
        public static ServerDirectoryEntry ToServerDirectoryEntry(this Device device)
        {
            bool isStreaming = device.StreamingHeaderPort != null && device.StreamingImagePort != null;
            ApplicationEntity server = isStreaming
                                           ?
                                               new StreamingServerApplicationEntity
                                                   {
                                                       Name = device.Name,
                                                       AETitle = device.AETitle,
                                                       Port = device.Port,
                                                       HostName = Dns.GetHostName(),
                                                       HeaderServicePort = device.StreamingHeaderPort.Value,
                                                       WadoServicePort = device.StreamingImagePort.Value
                                                   }
                                           :
                                               new DicomServerApplicationEntity
                                                   {
                                                       Name = device.Name,
                                                       AETitle = device.AETitle,
                                                       Port = device.Port,
                                                       HostName = Dns.GetHostName()
                                                   };

            return new ServerDirectoryEntry {Oid = device.Oid, Server = server};
        }

        public static Device ToDevice(this IDicomServerApplicationEntity server)
        {
            int? streamingHeaderPort = null;
            int? streamingImagePort = null;
            if (server.IsStreaming)
            {
                var streaming = (IStreamingServerApplicationEntity) server;
                streamingHeaderPort = streaming.HeaderServicePort;
                streamingImagePort = streaming.WadoServicePort;
            }

            return new Device
                       {
                           AETitle = server.AETitle,
                           Location = server.Location,
                           Name = server.Name,
                           Port = server.Port,
                           StreamingHeaderPort = streamingHeaderPort,
                           StreamingImagePort = streamingImagePort
                       };
         }
    }

    public class DeviceBroker : Broker
    {
        public DeviceBroker(DicomStoreDataContext context)
            : base(context)
		{
		}

        public List<Device> GetDevices()
        {
            return Context.Devices.ToList();
        }

        public Device GetDeviceByOid(Int64 oid)
        {
            var devices = from d in Context.Devices where d.Oid == oid select d;
            return devices.FirstOrDefault();
        }

        public Device GetDeviceByName(string name)
        {
            var devices = from d in Context.Devices where d.Name == name select d;
            return devices.FirstOrDefault();
        }

        public List<Device> GetDevicesByAETitle(string aeTitle)
        {
            var devices = from d in Context.Devices where d.AETitle == aeTitle select d;
            return devices.ToList();
        }

        internal void AddDevice(Device device)
        {
            Context.Devices.InsertOnSubmit(device);
        }

        internal void DeleteDevice(Device device)
        {
            Context.Devices.DeleteOnSubmit(device);
        }
    }
}
