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
    /// Used to create/update/delete service  entries in the database.
    /// </summary>
    /// 
    public class ServiceLockDataAdapter : BaseAdaptor<ServiceLock, IServiceLockEntityBroker, ServiceLockSelectCriteria, ServiceLockUpdateColumns>
    {
        /// <summary>
        /// Retrieve list of service s.
        /// </summary>
        /// <returns></returns>
        public IList<ServiceLock> GetServiceLocks()
        {
            return Get();
        }

        /// <summary>
        /// Delete a service  in the database.
        /// </summary>
        /// <param name="dev"></param>
        /// <returns></returns>
        public bool DeleteServiceLock(ServiceLock dev)
        {
            return base.Delete(dev.Key);
        }

        /// <summary>
        /// Update a service  entry in the database.
        /// </summary>
        /// <param name="service"></param>
        /// <returns></returns>
        public bool Update(ServiceLock service)
        {
            bool ok = true;

            ServiceLockUpdateColumns param = new ServiceLockUpdateColumns();
            param.Enabled = service.Enabled;
            param.FilesystemKey = service.FilesystemKey;
            param.Lock = service.Lock;
            param.ProcessorId = service.ProcessorId;
            param.ScheduledTime = service.ScheduledTime;
            param.ServiceLockTypeEnum = service.ServiceLockTypeEnum;

            ok = base.Update(service.Key,param);

            return ok;
        }

        

        /// <summary>
        /// Retrieve a list of service s with specified criteria.
        /// </summary>
        /// <returns></returns>
        public IList<ServiceLock> GetServiceLocks(ServiceLockSelectCriteria criteria)
        {
            return base.Get(criteria);
        }

        /// <summary>
        /// Create a new service .
        /// </summary>
        /// <param name="newService"></param>
        /// <returns></returns>
        public ServiceLock AddServiceLock(ServiceLock newService)
        {
            ServiceLockUpdateColumns param = new ServiceLockUpdateColumns();
            param.Enabled = newService.Enabled;
            param.FilesystemKey = newService.FilesystemKey;
            param.Lock = newService.Lock;
            param.ProcessorId = newService.ProcessorId;
            param.ScheduledTime = newService.ScheduledTime;
            param.ServiceLockTypeEnum = newService.ServiceLockTypeEnum;

            return base.Add(param);
        }
    }
}

