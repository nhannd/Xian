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
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.Common;

namespace ClearCanvas.ImageServer.Web.Common.Data
{

    /// <summary>
    /// ServiceLock configuration screen controller.
    /// </summary>
    public class ServiceLockConfigurationController
    {
        #region Private members

        /// <summary>
        /// The adapter class to retrieve/set services from service table
        /// </summary>
        private ServiceLockDataAdapter _adapter = new ServiceLockDataAdapter();

        /// <summary>
        /// The adapter class to set/retrieve server partitions from server partition table
        /// </summary>
        private ServerPartitionDataAdapter _serverAdapter = new ServerPartitionDataAdapter();

        #endregion

        #region public methods

        /// <summary>
        /// Add a service in the database.
        /// </summary>
        /// <param name="service"></param>
        public ServiceLock AddServiceLock(ServiceLock service)
        {
            Platform.Log(LogLevel.Info, "User adding new service lock { type={0}, filesystem={1} }", service.ServiceLockTypeEnum, service.FilesystemKey);

            ServiceLock dev = _adapter.AddServiceLock(service);

            if (dev!=null)
                Platform.Log(LogLevel.Info, "New service added by user : {Key={0}, type={1}, filesystem={2}", service.Key, service.ServiceLockTypeEnum, service.FilesystemKey);
            else
                Platform.Log(LogLevel.Info, "Failed to add new service : {  type={1}, filesystem={2} }", service.ServiceLockTypeEnum, service.FilesystemKey);

            return dev;
        }

        /// <summary>
        /// Delete a service from the database.
        /// </summary>
        /// <param name="service"></param>
        /// <returns><b>true</b> if the record is deleted successfully. <b>false</b> otherwise.</returns>
        public bool DeleteServiceLock(ServiceLock service)
        {
            Platform.Log(LogLevel.Info, "User deleting service lock {0}", service.Key);

            bool ok = _adapter.DeleteServiceLock(service);

            Platform.Log(LogLevel.Info, "User delete service lock {0}: {1}", service.Key, ok ? "Successful" : "Failed");

            return ok;
        }

        /// <summary>
        /// Update a service in the database.
        /// </summary>
        /// <param name="key"></param>
        /// <returns><b>true</b> if the record is updated successfully. <b>false</b> otherwise.</returns>
        /// <param name="enabled"></param>
        /// <param name="scheduledDateTime"></param>
        public bool UpdateServiceLock(ServerEntityKey key, bool enabled, DateTime scheduledDateTime)
        {
            Platform.Log(LogLevel.Info, "User updating service Key={0}", key.Key);
            ServiceLockUpdateColumns columns = new ServiceLockUpdateColumns();
            columns.Enabled = enabled;
            columns.ScheduledTime = scheduledDateTime;

            bool ok = _adapter.Update(key, columns);

            Platform.Log(LogLevel.Info, "ServiceLock Key={0} {1}", key.Key, ok ? "updated by user" : " failed to update");

            return ok;
        
        }

        /// <summary>
        /// Retrieve list of services.
        /// </summary>
        /// <param name="criteria"/>
        /// <returns>List of <see cref="ServiceLock"/> matches <paramref name="criteria"/></returns>
        public IList<ServiceLock> GetServiceLocks(ServiceLockSelectCriteria criteria)
        {
            return _adapter.GetServiceLocks(criteria);
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
