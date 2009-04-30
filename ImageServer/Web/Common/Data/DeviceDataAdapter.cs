#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections.Generic;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Used to create/update/delete device entries in the database.
    /// </summary>
    /// 
    public class DeviceDataAdapter : BaseAdaptor<Device, IDeviceEntityBroker, DeviceSelectCriteria, DeviceUpdateColumns>
    {
        /// <summary>
        /// Retrieve list of devices.
        /// </summary>
        /// <returns></returns>
        public IList<Device> GetDevices()
        {
            return Get();
        }

        /// <summary>
        /// Delete a device in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool DeleteDevice(Device dev)
        {
            return Delete(dev.Key);
        }

        /// <summary>
        /// Update a device entry in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool Update(Device dev)
        {
            bool ok = true;

            DeviceUpdateColumns param = new DeviceUpdateColumns();
            param.ServerPartitionKey = dev.ServerPartitionKey;
            param.Enabled = dev.Enabled;
            param.AeTitle = dev.AeTitle;
            param.Description = dev.Description;
            param.Dhcp = dev.Dhcp;
            param.IpAddress = dev.IpAddress;
            param.Port = dev.Port;
            param.AllowQuery = dev.AllowQuery;
            param.AllowRetrieve = dev.AllowRetrieve;
            param.AllowStorage = dev.AllowStorage;
            param.AllowAutoRoute = dev.AllowAutoRoute;
            param.ThrottleMaxConnections = dev.ThrottleMaxConnections;
            Update(dev.Key, param);

            return ok;
        }

        public IList<Device> DummyList
        {
            get
            {
                // return dummy list
                List<Device> list = new List<Device>();
                Device dev = new Device();
                dev.AeTitle = "Checking";

                dev.ServerPartitionKey = new ServerEntityKey("Testing", "Checking");
                list.Add(dev);

                return list;
            }
        }

        /// <summary>
        /// Retrieve a list of devices with specified criteria.
        /// </summary>
        /// <returns></returns>
        public IList<Device> GetDevices(DeviceSelectCriteria criteria)
        {
            return Get(criteria);
        }

        /// <summary>
        /// Create a new device.
        /// </summary>
        /// <param name="newDev"></param>
        /// <returns></returns>
        public Device AddDevice(Device newDev)
        {
            DeviceUpdateColumns param = new DeviceUpdateColumns();
            param.ServerPartitionKey = newDev.ServerPartitionKey;
            param.AeTitle = newDev.AeTitle;
            param.Description = newDev.Description;
            param.IpAddress = newDev.IpAddress;
            param.Port = newDev.Port;
            param.Enabled = newDev.Enabled;
            param.Dhcp = newDev.Dhcp;
            param.AllowQuery = newDev.AllowQuery;
            param.AllowRetrieve = newDev.AllowRetrieve;
            param.AllowStorage = newDev.AllowStorage;
            param.AllowAutoRoute = newDev.AllowAutoRoute;
            param.ThrottleMaxConnections = newDev.ThrottleMaxConnections;
            return Add(param);
        }
    }
}

