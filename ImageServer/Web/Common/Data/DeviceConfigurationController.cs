using System.Collections.Generic;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Defines the interface of a device configuration controller.
    /// </summary>
    public interface IDeviceConfigurationController
    {
        bool AddDevice(Device device);
        bool DeleteDevice(Device device);
        IList<Device> GetDevices(DeviceSelectCriteria criteria);
        bool UpdateDevice(Device device);
        IList<ServerPartition> GetServerPartitions();
    }

    /// <summary>
    /// Device configuration screen controller.
    /// </summary>
    public class DeviceConfigurationController:IDeviceConfigurationController
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
        public bool AddDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Adding new device : AETitle = {0}", device.AeTitle);

            bool ok = _adapter.AddDevice(device);
            
            Platform.Log(LogLevel.Info, "New device added : AETitle = {0}", device.AeTitle);

            return ok;
        }

        /// <summary>
        /// Delete a device from the database.
        /// </summary>
        /// <param name="device"></param>
        /// <returns><b>true</b> if the record is deleted successfully. <b>false</b> otherwise.</returns>
        public bool DeleteDevice(Device device)
        {
            Platform.Log(LogLevel.Info, "Deleting {0}, GUID={1}", device.AeTitle, device.GetKey());
            
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
            Platform.Log(LogLevel.Info, "Updating device GUID={1} : AETitle={0}",  device.GetKey(), device.AeTitle);
            bool ok = _adapter.Update(device);
            Platform.Log(LogLevel.Info, "Device GUID={0} {1}", device.GetKey(), ok? "updated":" failed to update");

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
