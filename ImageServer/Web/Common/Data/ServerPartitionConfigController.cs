#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

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

    public class ServerPartitionConfigController : IServerPartitionConfigurationController
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
