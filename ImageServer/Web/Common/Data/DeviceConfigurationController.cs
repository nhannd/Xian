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
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Data
{

    /// <summary>
    /// Device configuration screen controller.
    /// </summary>
    public class DeviceConfigurationController
    {
        #region Private members

        /// <summary>
        /// The adapter class to retrieve/set devices from device table
        /// </summary>
        private DeviceDataAdapter _adapter = new DeviceDataAdapter();

        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a device in the database.
        /// </summary>
        /// <param name="device"></param>
        public Device AddDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Adding new device : AETitle = {0}", device.AeTitle);

            Device dev = _adapter.AddDevice(device);

            if (dev!=null)
                Platform.Log(LogLevel.Info, "New device added :AE={0}, Key={1}",dev.AeTitle, dev.Key);
            else
                Platform.Log(LogLevel.Info, "Failed to add new device : AETitle={0}", dev.AeTitle);

            return dev;
        }

        /// <summary>
        /// Delete a device from the database.
        /// </summary>
        /// <param name="device"></param>
        /// <returns><b>true</b> if the record is deleted successfully. <b>false</b> otherwise.</returns>
        public bool DeleteDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Deleting {0}, Key={1}", device.AeTitle, device.Key);

            bool ok = _adapter.DeleteDevice(device);

            Platform.Log(LogLevel.Info, "Delete of {0} {1}", device.AeTitle, ok ? "Successful" : "Failed");

            return ok;
        }

        /// <summary>
        /// Update a device in the database.
        /// </summary>
        /// <param name="device"></param>
        /// <returns><b>true</b> if the record is updated successfully. <b>false</b> otherwise.</returns>
        public bool UpdateDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Updating device Key={1} : AETitle={0}", device.Key, device.AeTitle);
            bool ok = _adapter.Update(device);
            Platform.Log(LogLevel.Info, "Device Key={0} {1}", device.Key, ok ? "updated" : " failed to update");

            return ok;
        }

        /// <summary>
        /// Retrieve list of devices.
        /// </summary>
        /// <param name="criteria"/>
        /// <returns>List of <see cref="Device"/> matches <paramref name="criteria"/></returns>
        public IList<Device> GetDevices(DeviceSelectCriteria criteria)
        {
            return _adapter.GetDevices(criteria);
        }

        /// <summary>
        /// Retrieve a list of server partitions.
        /// </summary>
        /// <returns>List of all <see cref="ServerPartition"/>.</returns>
        public IList<ServerPartition> GetServerPartitions()
        {
            return _serverAdapter.GetServerPartitions();
        }

        #endregion public methods
    }
}
