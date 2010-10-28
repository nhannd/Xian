#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Model
{
    public partial class WorkQueue
    {
        #region Private Members
        protected StudyStorage _studyStorage;
        private Study _study;
        #endregion

        

        private void LoadRelatedEntities()
        {
            if (_study==null || _studyStorage==null)
            {
                using (IReadContext context = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
                {
                    lock (SyncRoot)
                    {
                        if (_study == null)
                            _study = LoadStudy(context);

                        if (_studyStorage == null)
                            _studyStorage = LoadStudyStorage(context);
                    }

                }    
            }
            
        }


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
        private Study LoadStudy(IPersistenceContext context)
        {
            if (_study == null)
            {
                lock (SyncRoot)
                {
                    _study = Study.Find(context, StudyStorageKey);
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
                lock (SyncRoot)
                {
                    _studyStorage = StudyStorage.Load(context, StudyStorageKey); 
                }
            }
            return _studyStorage;
        }

        public IList<StudyStorageLocation> LoadStudyLocations(IPersistenceContext context)
        {
            StudyStorage storage = LoadStudyStorage(context);
            return StudyStorageLocation.FindStorageLocations(context, storage);
        }

        public StudyStorage StudyStorage
        {
            get
            {
                LoadRelatedEntities();
                return _studyStorage;
            }
        }

        public Study Study
        {
            get
            {
                LoadRelatedEntities();
                return _study;
            }
            set { _study = value; }
        }
    }
}
