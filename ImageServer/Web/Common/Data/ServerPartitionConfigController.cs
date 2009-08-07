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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class ServerPartitionConfigController
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
        /// Retrieves a list of <seealso cref="ServerPartition"/> matching the specified criteria.
        /// </summary>
        /// <returns>A list of partitions</returns>
        public ServerPartition GetPartition(ServerEntityKey key)
        {
            return _serverAdapter.GetServerPartition(key);
        }

        /// <summary>
        /// Retrieves all server paritions.
        /// </summary>
        /// <returns></returns>
        public IList<ServerPartition> GetAllPartitions()
        {
        	ServerPartitionSelectCriteria searchCriteria = new ServerPartitionSelectCriteria();
        	searchCriteria.AeTitle.SortAsc(0);
			return GetPartitions(searchCriteria);
        }

        /// <summary>
        /// Checks if a specified partition can be deleted
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool CanDelete(ServerPartition partition)
        {
            return partition.StudyCount == 0;
        }


        /// <summary>
        /// Delete the specified partition
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool Delete(ServerPartition partition)
        {
            Platform.Log(LogLevel.Info, "Deleting server partition: AETitle = {0}", partition.AeTitle);

            try
            {
                using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    IDeleteServerPartition broker = ctx.GetBroker<IDeleteServerPartition>();
                    ServerPartitionDeleteParameters parms = new ServerPartitionDeleteParameters();
                    parms.ServerPartitionKey = partition.Key;
                    if (!broker.Execute(parms))
                        throw new Exception("Unable to delete server partition from database");
                    ctx.Commit();
                }

                Platform.Log(LogLevel.Info, "Server Partition deleted : AETitle = {0}", partition.AeTitle);
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Info, e, "Failed to delete Server Partition: AETitle = {0}", partition.AeTitle);
                return false;
            }


        }

        #endregion // public methods

    }
}
