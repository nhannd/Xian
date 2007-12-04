using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Criteria;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Defines the interface of a device configuration controller.
    /// </summary>
    public interface IServerPartitionConfigurationController
    {
        bool AddPartition(ServerPartition partition);
        bool UpdatePartition(ServerPartition partition);
        IList<ServerPartition> GetPartitions(ServerPartitionSelectCriteria criteria);
        IList<ServerPartition> GetAllPartitions();
    }

    public class ServerParitionConfigController:IServerPartitionConfigurationController
    {
        #region Private members
        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private readonly ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods
        /// <summary>
        /// Add a partition in the database.
        /// </summary>
        /// <param name="partition"></param>
        public bool AddPartition(ServerPartition partition)
        {
            Platform.Log(LogLevel.Info, "Adding new server partition : AETitle = {0}", partition.AeTitle);

            bool result = _serverAdapter.AddServerPartition(partition);

            if (result)
                Platform.Log(LogLevel.Info, "Server Partition added : AETitle = {0}", partition.AeTitle);
            else
                Platform.Log(LogLevel.Info, "Failed to add Server Partition: AETitle = {0}", partition.AeTitle);

            return result;
        }

        /// <summary>
        /// Update the partition whose GUID and new information are specified in <paramref name="partition"/>.
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool UpdatePartition(ServerPartition partition)
        {
            Platform.Log(LogLevel.Info, "Updating server partition: AETitle = {0}", partition.AeTitle);

            bool result = _serverAdapter.Update(partition);

            if (result)
                Platform.Log(LogLevel.Info, "Server Partition updated : AETitle = {0}", partition.AeTitle);
            else
                Platform.Log(LogLevel.Info, "Failed to update Server Partition: AETitle = {0}", partition.AeTitle);

            return result;
        }

        /// <summary>
        /// Retrieves a list of <seealso cref="ServerPartition"/> matching the specified criteria.
        /// </summary>
         /// <returns>A list of partitions</returns>
        public IList<ServerPartition> GetPartitions(ServerPartitionSelectCriteria criteria)
        {
            //ServerPartitionSelectCriteria criteria = new ServerPartitionSelectCriteria();

            //if (String.IsNullOrEmpty(AETitle)==false)
            //{
            //    AETitle = AETitle.Replace("*", "%");
            //    criteria.AETitle.Like(AETitle + "%");
            //}

            //if (String.IsNullOrEmpty(description) == false)
            //{
            //    description = description.Replace("*", "%");
            //    criteria.Description.Like(description + "%");
            //}

            //if (String.IsNullOrEmpty(partitionFolder)==false)
            //{
            //    partitionFolder = partitionFolder.Replace("*", "%");
            //    criteria.PartitionFolder.Like(partitionFolder + "%");
            //}

            //if (enabledOnly)
            //{
            //    criteria.Enabled.EqualTo(true);
            //}

            return _serverAdapter.GetServerPartitions(criteria);
        }

        /// <summary>
        /// Retrieves all server paritions.
        /// </summary>
        /// <returns></returns>
        public IList<ServerPartition> GetAllPartitions()
        {
            return GetPartitions(new ServerPartitionSelectCriteria());
        }

        #endregion // public methods
    }
}
