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

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class PartitionArchiveConfigController
    {
        #region Private members

        /// <summary>
        /// The adapter class to set/retrieve archive partitions from archive partition table
        /// </summary>
        private readonly PartitionArchiveDataAdapter _archiveAdapter = new PartitionArchiveDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a partition in the database.
        /// </summary>
        /// <param name="partition"></param>
        public bool AddPartition(PartitionArchive partition)
        {
            Platform.Log(LogLevel.Info, "Adding new partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            bool result = _archiveAdapter.AddPartitionArchive(partition);

            if (result)
                Platform.Log(LogLevel.Info, "Added new partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);
            else
                Platform.Log(LogLevel.Info, "Failed to add new partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            return result;
        }

        /// <summary>
        /// Update the partition whose GUID and new information are specified in <paramref name="partition"/>.
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool UpdatePartition(PartitionArchive partition)
        {
            Platform.Log(LogLevel.Info, "Updating partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            bool result = _archiveAdapter.Update(partition);

            if (result)
                Platform.Log(LogLevel.Info, "Updated partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);
            else
                Platform.Log(LogLevel.Info, "Failed to update partition archive : Type = {0} , Description = {1}", partition.ArchiveTypeEnum.Description, partition.Description);

            return result;
        }

        

        /// <summary>
        /// Retrieves a list of <seealso cref="PartitionArchive"/> matching the specified criteria.
        /// </summary>
        /// <returns>A list of partitions</returns>
        public IList<PartitionArchive> GetPartitions(PartitionArchiveSelectCriteria criteria)
        {
            return _archiveAdapter.GetPartitionArchives(criteria);
        }

        /// <summary>
        /// Retrieves all archive paritions.
        /// </summary>
        /// <returns></returns>
        public IList<PartitionArchive> GetAllPartitions()
        {
        	PartitionArchiveSelectCriteria searchCriteria = new PartitionArchiveSelectCriteria();
			return GetPartitions(searchCriteria);
        }

        /// <summary>
        /// Delete the specified partition
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool Delete(PartitionArchive partition)
        {
            return _archiveAdapter.Delete(partition.GetKey());
        }

        /// <summary>
        /// Determine if the specified partition can be deleted. If studies are scheduled
        /// to be archived on that partition or studies are already archived on that partition,
        /// then the partition may not be deleted.
        /// 
        /// </summary>
        /// <param name="partition"></param>
        /// <returns></returns>
        public bool CanDelete(PartitionArchive partition)
        {          
            ArchiveQueueAdaptor archiveQueueAdaptor = new ArchiveQueueAdaptor();
            ArchiveQueueSelectCriteria selectCriteria = new ArchiveQueueSelectCriteria();
            selectCriteria.PartitionArchiveKey.EqualTo(partition.GetKey());

            ArchiveStudyStorageAdaptor archiveStudyStorageAdaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria criteria = new ArchiveStudyStorageSelectCriteria();
            criteria.PartitionArchiveKey.EqualTo(partition.GetKey());

            IList<ArchiveQueue> archiveItems = archiveQueueAdaptor.Get(selectCriteria);
            IList<ArchiveStudyStorage> storageItems = archiveStudyStorageAdaptor.Get(criteria);

            return ((archiveItems != null && archiveItems.Count > 0) ||
                (storageItems != null && storageItems.Count > 0)) ? false : true;


        }

        #endregion // public methods

    }
}
