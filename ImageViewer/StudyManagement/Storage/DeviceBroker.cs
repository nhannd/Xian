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
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement.Storage
{
    public static class DeviceExtensions
    {
        public static ApplicationEntity ToDataContract(this Device device)
        {
            bool isStreaming = device.StreamingHeaderPort != null && device.StreamingImagePort != null;
            var server = new ApplicationEntity
                                           {
                                               Name = device.Name,
                                               AETitle = device.AETitle,
                                               ScpParameters = new ScpParameters(device.HostName, device.Port),
                                               Location = device.Location,
                                               Description = device.Description
                                           };

            if (isStreaming)
                server.StreamingParameters = new StreamingParameters(device.StreamingHeaderPort.Value, device.StreamingImagePort.Value);
            
            return server;
        }

        public static Device ToDevice(this IApplicationEntity server)
        {
            Platform.CheckForNullReference(server, "server");
            Platform.CheckMemberIsSet(server.ScpParameters, "ScpParameters");

            int? streamingHeaderPort = null;
            int? streamingImagePort = null;
            if (server.StreamingParameters != null)
            {
                streamingHeaderPort = server.StreamingParameters.HeaderServicePort;
                streamingImagePort = server.StreamingParameters.WadoServicePort;
            }
            
            return new Device
                       {
                           Name = server.Name,
                           AETitle = server.AETitle,
                           HostName = server.ScpParameters.HostName,
                           Port = server.ScpParameters.Port,
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
            return devices.SingleOrDefault();
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
