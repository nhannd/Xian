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

using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class WorkQueue
    {
        #region Private Members
        private readonly object _syncLock = new object();
        private StudyStorage _studyStorage;
        private Study _study;
        #endregion


        /// <summary>
        /// Delete the Work Queue record from the system.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public bool Delete(IPersistenceContext context)
        {
            IWorkQueueUidEntityBroker workQueueUidBroker = context.GetBroker<IWorkQueueUidEntityBroker>();
            WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
            criteria.WorkQueueKey.EqualTo(GetKey());
            workQueueUidBroker.Delete(criteria);

            IWorkQueueEntityBroker workQueueBroker = context.GetBroker<IWorkQueueEntityBroker>();
            return workQueueBroker.Delete(GetKey());
        }

        /// <summary>
        /// Loads the related <see cref="Study"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Study LoadStudy(IPersistenceContext context)
        {
            StudyStorage storage = LoadStudyStorage(context);

            if (_study == null)
            {
                lock (_syncLock)
                {
                    _study = storage.LoadStudy(context);
                }
            }
            return _study;
        }

        /// <summary>
        /// Loads the related <see cref="StudyStorage"/> entity.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private StudyStorage LoadStudyStorage(IPersistenceContext context)
        {
            if (_studyStorage==null)
            {
                lock(_syncLock)
                {
                    _studyStorage = StudyStorage.Load(context, StudyStorageKey); 
                }
            }
            return _studyStorage;
        }
    }
}
