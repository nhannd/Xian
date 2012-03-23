#region License

// Copyright (c) 2012, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Net;
using ClearCanvas.Dicom.Iod;
using ClearCanvas.Dicom.ServiceModel;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    public static class DeviceExtensions
    {
        public static ApplicationEntity ToDataContract(this Device device)
        {
            bool isStreaming = device.StreamingHeaderPort != null && device.StreamingImagePort != null;
            ApplicationEntity server = isStreaming
                                           ?
                                               new StreamingServerApplicationEntity
                                                   {
                                                       Name = device.Name,
                                                       AETitle = device.AETitle,
                                                       Port = device.Port,
                                                       HostName = device.HostName,
                                                       HeaderServicePort = device.StreamingHeaderPort.Value,
                                                       WadoServicePort = device.StreamingImagePort.Value,
                                                       Location = device.Location,
                                                       Description = device.Description
                                                   }
                                           :
                                               new DicomServerApplicationEntity
                                                   {
                                                       Name = device.Name,
                                                       AETitle = device.AETitle,
                                                       Port = device.Port,
                                                       HostName = device.HostName,
                                                       Location = device.Location,
                                                       Description = device.Description
                                                   };

            return server;
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
                           Name = server.Name,
                           AETitle = server.AETitle,
                           HostName = server.HostName,
                           Port = server.Port,
                           Location = server.Location,
                           Description = server.Description,
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

        internal void DeleteAllDevices()
        {
            Context.Devices.DeleteAllOnSubmit(Context.Devices);
        }
    }
}
