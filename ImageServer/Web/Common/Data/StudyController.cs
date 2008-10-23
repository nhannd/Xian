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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    public class StudyController : BaseController
    {
        #region Private Members

        private readonly StudyAdaptor _adaptor = new StudyAdaptor();
        private readonly SeriesSearchAdaptor _seriesAdaptor = new SeriesSearchAdaptor();
		private readonly PartitionArchiveAdaptor _partitionArchiveAdaptor = new PartitionArchiveAdaptor();
        #endregion

        #region Public Methods

        public IList<Study> GetStudies(StudySelectCriteria criteria)
        {
            return _adaptor.Get(criteria);
        }
		public IList<Study> GetRangeStudies(StudySelectCriteria criteria, int startIndex, int maxRows)
		{
			return _adaptor.GetRange(criteria,startIndex,maxRows);
		}

		public int GetStudyCount(StudySelectCriteria criteria)
		{
			return _adaptor.GetCount(criteria);
		}

        public IList<Series> GetSeries(Study study)
        {
            SeriesSelectCriteria criteria = new SeriesSelectCriteria();

            criteria.StudyKey.EqualTo(study.Key);

            return _seriesAdaptor.Get(criteria);
        }

		public IList<StudyIntegrityQueue> GetStudyIntegrityQueueItems(ServerEntityKey studyStorageKey)
        {
			Platform.CheckForNullReference(studyStorageKey, "storageKey");

            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IStudyIntegrityQueueEntityBroker integrityQueueBroker = ctx.GetBroker<IStudyIntegrityQueueEntityBroker>();
                StudyIntegrityQueueSelectCriteria parms = new StudyIntegrityQueueSelectCriteria();

				parms.StudyStorageKey.EqualTo(studyStorageKey);

                return integrityQueueBroker.Find(parms);
            }
        }

		/// <summary>
		/// Delete a Study.
		/// </summary>
		/// <param name="study">The <see cref="Study"/> to delete.</param>
		/// <returns>true on success, false on failure.</returns>
        public bool DeleteStudy(Study study)
        {
			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				StudyStorage storage = StudyStorage.Load(ctx, study.ServerPartitionKey, study.StudyInstanceUid);
				
				LockStudyParameters lockParms = new LockStudyParameters();
				lockParms.QueueStudyStateEnum = QueueStudyStateEnum.WebDeleteScheduled;
				lockParms.StudyStorageKey = storage.Key;

				ILockStudy broker = ctx.GetBroker<ILockStudy>();
				broker.Execute(lockParms);
				if (!lockParms.Successful)
					return false;
				
				WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
				columns.WorkQueueTypeEnum = WorkQueueTypeEnum.WebDeleteStudy;
				columns.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
				columns.ServerPartitionKey = study.ServerPartitionKey;
				columns.StudyStorageKey = storage.Key;
				columns.ScheduledTime = DateTime.Now; // spread by 15 seconds
				columns.ExpirationTime = DateTime.Now.AddMinutes(1);
				columns.FailureCount = 0;

				WorkQueueAdaptor workqueueAdaptor = new WorkQueueAdaptor();
				workqueueAdaptor.Add(ctx, columns);

				ctx.Commit();

				return true;
			}
        }

		/// <summary>
		/// Restore a nearline study.
		/// </summary>
		/// <param name="study">The <see cref="Study"/> to restore.</param>
		/// <returns>true on success, false on failure.</returns>
		public bool RestoreStudy(Study study)
		{
			return _partitionArchiveAdaptor.RestoreStudy(study);
		}

        public bool MoveStudy(Study study, Device device)
        {
            WorkQueueAdaptor workqueueAdaptor = new WorkQueueAdaptor();
            WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
            columns.WorkQueueTypeEnum = WorkQueueTypeEnum.WebMoveStudy;
            columns.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
            columns.ServerPartitionKey = study.ServerPartitionKey;

            StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
            criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

            StudyStorage storage = studyStorageAdaptor.GetFirst(criteria);

            columns.StudyStorageKey = storage.Key;
            DateTime time = Platform.Time;
            columns.ScheduledTime = time;
            columns.ExpirationTime = time;
            columns.FailureCount = 0;
            columns.DeviceKey = device.Key;

            workqueueAdaptor.Add(columns);

            return true;
        }

        public bool EditStudy(Study study, XmlDocument modifiedFields)
        {
			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
				StudyStorage storage = StudyStorage.Load(ctx, study.ServerPartitionKey, study.StudyInstanceUid);

				LockStudyParameters lockParms = new LockStudyParameters();
				lockParms.QueueStudyStateEnum = QueueStudyStateEnum.EditScheduled;
				lockParms.StudyStorageKey = storage.Key;

				ILockStudy broker = ctx.GetBroker<ILockStudy>();
				broker.Execute(lockParms);
				if (!lockParms.Successful)
					return false;

				WorkQueueAdaptor workqueueAdaptor = new WorkQueueAdaptor();
				WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
				columns.WorkQueueTypeEnum = WorkQueueTypeEnum.WebEditStudy;
				columns.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
				columns.ServerPartitionKey = study.ServerPartitionKey;
				columns.WorkQueuePriorityEnum = WorkQueuePriorityEnum.High;
				columns.StudyStorageKey = storage.Key;
				DateTime time = Platform.Time;
				columns.ScheduledTime = time;
				columns.ExpirationTime = time;
				columns.FailureCount = 0;
				columns.Data = modifiedFields;

				workqueueAdaptor.Add(columns);
				ctx.Commit();
				return true;
			}
        }

		public bool UpdateStudy(Study study, StudyUpdateColumns columns)
		{
			return _adaptor.Update(study.Key, columns);
		}

        public bool IsScheduledForEdit(Study study)
        {
            return IsStudyInWorkQueue(study, WorkQueueTypeEnum.WebEditStudy);
        }

        public bool IsScheduledForDelete(Study study)
        {
            return IsStudyInWorkQueue(study, WorkQueueTypeEnum.WebDeleteStudy);
        }

        private ServerEntityKey GetStudyStorageGUID(Study study)
        {
            
            return GetStudyStorage(study).Key;
        }

        /// <summary>
        /// Returns a value indicating whether the specified study has been scheduled for delete.
        /// </summary>
        /// <param name="study"></param>
        /// <param name="workQueueType"></param>
        /// <returns></returns>           
        private bool IsStudyInWorkQueue(Study study, WorkQueueTypeEnum workQueueType)
        {
            Platform.CheckForNullReference(study, "Study");
            
            StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
            criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

            IList<StudyStorage> storages = studyStorageAdaptor.Get(criteria);
            foreach (StudyStorage storage in storages)
            {
                WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
                WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
                workQueueCriteria.WorkQueueTypeEnum.EqualTo(workQueueType);
                workQueueCriteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
                workQueueCriteria.StudyStorageKey.EqualTo(storage.Key);

                workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.Pending);

                IList<WorkQueue> list = adaptor.Get(workQueueCriteria);
                if (list != null && list.Count > 0)
                    return true;
                else
                {
                    workQueueCriteria.WorkQueueStatusEnum.EqualTo(WorkQueueStatusEnum.Idle); // not likely but who knows
                    list = adaptor.Get(workQueueCriteria);
                    if (list != null && list.Count > 0)
                        return true;
                }
            }
            return false;
        }

		/// <summary>
		/// Returns a value indicating whether the specified study has been scheduled for delete.
		/// </summary>
		/// <param name="study"></param>
		/// <returns></returns>
		public string GetModalitiesInStudy(Study study)
		{
			Platform.CheckForNullReference(study, "Study");
			SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
			SeriesSelectCriteria criteria = new SeriesSelectCriteria();
			
			criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
			criteria.StudyKey.EqualTo(study.Key);

			IList<Series> seriesList = seriesAdaptor.Get(criteria);

			List<string> modalities = new List<string>();
			
			foreach (Series series in seriesList)
			{
				bool found = false;
				foreach (string modality in modalities)
					if (modality.Equals(series.Modality))
					{
						found = true;
						break;
					}
				if (!found)
					modalities.Add(series.Modality);
			}

			string modalitiesInStudy = "";
			foreach (string modality in modalities)
				if (modalitiesInStudy.Length == 0)
					modalitiesInStudy = modality;
				else
					modalitiesInStudy += "\\" + modality;

			return modalitiesInStudy;
		}

        public IList<WorkQueue> GetWorkQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            WorkQueueAdaptor adaptor = new WorkQueueAdaptor();
            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(GetStudyStorageGUID(study));

            return adaptor.Get(workQueueCriteria);
        }

        public IList<FilesystemQueue> GetFileSystemQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            FileSystemQueueAdaptor adaptor = new FileSystemQueueAdaptor();
            FilesystemQueueSelectCriteria fileSystemQueueCriteria = new FilesystemQueueSelectCriteria();
            fileSystemQueueCriteria.StudyStorageKey.EqualTo(GetStudyStorageGUID(study));

            return adaptor.Get(fileSystemQueueCriteria);
        }

        public IList<ArchiveQueue> GetArchiveQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            ArchiveQueueAdaptor adaptor = new ArchiveQueueAdaptor();
            ArchiveQueueSelectCriteria archiveQueueCriteria = new ArchiveQueueSelectCriteria();
            archiveQueueCriteria.StudyStorageKey.EqualTo(GetStudyStorageGUID(study));
			archiveQueueCriteria.ScheduledTime.SortDesc(0);
            return adaptor.Get(archiveQueueCriteria);
        }

        public IList<ArchiveStudyStorage> GetArchiveStudyStorage(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
            archiveStudyStorageCriteria.StudyStorageKey.EqualTo(GetStudyStorageGUID(study));
        	archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

            return adaptor.Get(archiveStudyStorageCriteria);
        }

        public IList<StudyStorageLocation> GetStudyStorageLocation(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            using (IReadContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenReadContext())
            {
                IQueryStudyStorageLocation select = ctx.GetBroker<IQueryStudyStorageLocation>();
                StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();

                parms.StudyStorageKey = GetStudyStorageGUID(study);

                IList<StudyStorageLocation> storage = select.Find(parms);

                if (storage == null)
				{
					storage = new List<StudyStorageLocation>();
				    Platform.Log(LogLevel.Warn, "Unable to find storage location for Study item: {0}",
                                 study.GetKey().ToString());
                }

                if (storage.Count > 1)
                {
                    Platform.Log(LogLevel.Warn,
                                 "StudyController:GetStudyStorageLocation: multiple study storage found for study {0}",
                                 study.GetKey().Key);
                }

                return storage;
            }
        }
		public StudyStorage GetStudyStorage(Study study)
		{
			Platform.CheckForNullReference(study, "Study");

			StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
			StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
			criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
			criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

			return studyStorageAdaptor.GetFirst(criteria);
		}

		public bool CanScheduleDelete(StudySummary study)
		{
			return !study.IsLocked && !study.IsReconcileRequired;
		}

		public bool CanScheduleEdit(StudySummary study)
		{
			if (study.IsLocked || study.IsProcessing)
				return false;

			if (study.IsNearline)
			{
				Platform.CheckTrue(study.IsArchived, "study.IsArchived");

				StudyStorageLocation studyStorage = CollectionUtils.FirstElement(GetStudyStorageLocation(study.TheStudy));

				if (study.TheArchiveLocation != null && studyStorage != null)
				{
					if (study.TheArchiveLocation.ServerTransferSyntax.Lossless &&
						TransferSyntax.GetTransferSyntax(studyStorage.TransferSyntaxUid).LossyCompressed)
					{
						// archive is lossless but current copy is lossy. can't edit until the lossless is restored.
						return false;
					}

				}
			}

			return true;
		}

		public bool CanScheduleMove(StudySummary study)
		{
			return !study.IsLocked && !study.IsReconcileRequired;
		}

		public bool CanScheduleRestore(StudySummary study)
		{
			return study.IsArchived && !study.IsLocked && !study.IsReconcileRequired;
		}

		public bool CanScheduleReconcile(StudySummary study)
		{
			return !study.IsLocked && !study.IsReconcileRequired;
		}
        #endregion
    }
}
