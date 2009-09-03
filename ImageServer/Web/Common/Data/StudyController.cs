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
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Edit;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data.DataSource;
using ClearCanvas.ImageServer.Web.Common.Data.Model;

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


            IStudyIntegrityQueueEntityBroker integrityQueueBroker = HttpContextData.Current.ReadContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueueSelectCriteria parms = new StudyIntegrityQueueSelectCriteria();

			parms.StudyStorageKey.EqualTo(studyStorageKey);

            return integrityQueueBroker.Find(parms);
        
        }

		/// <summary>
		/// Delete a Study.
		/// </summary>
		public void DeleteStudy(ServerEntityKey studyKey, string reason)
        {
			StudySummary study;

            study = StudySummaryAssembler.CreateStudySummary(HttpContextData.Current.ReadContext, Study.Load(HttpContextData.Current.ReadContext, studyKey));
			if (study.IsReconcileRequired)
			{
				throw new ApplicationException(
					String.Format("Deleting the study is not allowed at this time : there are items to be reconciled."));

				// NOTE: another check will occur when the delete is actually processed
			}
			

			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
                LockStudyParameters lockParms = new LockStudyParameters();
				lockParms.QueueStudyStateEnum = QueueStudyStateEnum.WebDeleteScheduled;
                lockParms.StudyStorageKey = study.TheStudyStorage.Key;
				ILockStudy broker = ctx.GetBroker<ILockStudy>();
				broker.Execute(lockParms);
				if (!lockParms.Successful)
				{
				    throw new ApplicationException(String.Format("Unable to lock the study : {0}", lockParms.FailureReason));
				}
				

				InsertWorkQueueParameters insertParms = new InsertWorkQueueParameters();
				insertParms.WorkQueueTypeEnum = WorkQueueTypeEnum.WebDeleteStudy;
				insertParms.WorkQueuePriorityEnum = WorkQueuePriorityEnum.Medium;
			    insertParms.ServerPartitionKey = study.ThePartition.Key;
				insertParms.StudyStorageKey = study.TheStudyStorage.Key;
                insertParms.ScheduledTime = Platform.Time; // spread by 15 seconds
                insertParms.ExpirationTime = Platform.Time.AddMinutes(1);

			    WebDeleteStudyLevelQueueData extendedData = new WebDeleteStudyLevelQueueData();
			    extendedData.Level = DeletionLevel.Study;
                extendedData.Reason = reason;
			    extendedData.UserId = ServerHelper.CurrentUserId;
			    extendedData.UserName = ServerHelper.CurrentUserName;
                insertParms.WorkQueueData = XmlUtils.SerializeAsXmlDoc(extendedData);
				IInsertWorkQueue insertWorkQueue = ctx.GetBroker<IInsertWorkQueue>();
				
                if (insertWorkQueue.FindOne(insertParms)==null)
                {
                    throw new ApplicationException("DeleteStudy failed");
                }


				ctx.Commit();
			}
        }

        public void DeleteSeries(Study study, IList<Series> series, string reason)
        {
            // Load the Partition
            ServerPartitionConfigController partitionConfigController = new ServerPartitionConfigController();
            ServerPartition partition = partitionConfigController.GetPartition(study.ServerPartitionKey);

            List<string> seriesUids = new List<string>();
            foreach (Series s in series)
            {
                seriesUids.Add(s.SeriesInstanceUid);
            }

            using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                StudyEditorHelper.DeleteSeries(ctx, partition, study.StudyInstanceUid, seriesUids, reason);
                ctx.Commit();
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
            return MoveStudy(study, device, null);
        }

        public bool MoveStudy(Study study, Device device, IList<Series> seriesList)
        {
            DateTime scheduledTime = Platform.Time.AddSeconds(10);
			if (seriesList != null)
			{
                using (
					IUpdateContext context = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
				{
                    ServerPartition partition = ServerPartition.Load(study.ServerPartitionKey);

				    List<string> seriesUids = new List<string>();
                    foreach (Series series in seriesList)
					{
					    seriesUids.Add(series.SeriesInstanceUid);    
					}

                    StudyEditorHelper.MoveSeries(context, partition, study.StudyInstanceUid, device.Key, seriesUids);

					return true;
				}
			}
			else
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
                columns.ExpirationTime = time.AddMinutes(4);
                columns.FailureCount = 0;
                columns.DeviceKey = device.Key;

                workqueueAdaptor.Add(columns);
            }
               
            return true;
	    }

        public void EditStudy(Study study, List<UpdateItem> updateItems)
        {
            Platform.Log(LogLevel.Info, String.Format("Editing study {0}", study.StudyInstanceUid));

            ServerPartition partition = ServerPartition.Load(study.ServerPartitionKey);
			    
			using (IUpdateContext ctx = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
			{
                IList<WorkQueue> entries = StudyEditorHelper.EditStudy(ctx, partition, study.StudyInstanceUid, updateItems);
                if (entries!=null)
			        ctx.Commit();
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

        public bool CanManipulateSeries(Study study)
        {
            StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
            StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
            criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
            criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

            StudyStorage storage = studyStorageAdaptor.GetFirst(criteria);
            
            if(!storage.QueueStudyStateEnum.Equals(QueueStudyStateEnum.Idle))
            {
                return false;
            }
            
            return true;
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
		/// <param name="read"></param>
		/// <returns></returns>
		public string GetModalitiesInStudy(IPersistenceContext read, Study study)
		{
			Platform.CheckForNullReference(study, "Study");
			SeriesSearchAdaptor seriesAdaptor = new SeriesSearchAdaptor();
			SeriesSelectCriteria criteria = new SeriesSelectCriteria();
			
			criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
			criteria.StudyKey.EqualTo(study.Key);

			IList<Series> seriesList = seriesAdaptor.Get(read, criteria);

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
            workQueueCriteria.ScheduledTime.SortAsc(0);
            return adaptor.Get(workQueueCriteria);
        }

        public IList<FilesystemQueue> GetFileSystemQueueItems(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            FileSystemQueueAdaptor adaptor = new FileSystemQueueAdaptor();
            FilesystemQueueSelectCriteria fileSystemQueueCriteria = new FilesystemQueueSelectCriteria();
            fileSystemQueueCriteria.StudyStorageKey.EqualTo(GetStudyStorageGUID(study));
            fileSystemQueueCriteria.ScheduledTime.SortAsc(0);
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

		public IList<ArchiveStudyStorage> GetArchiveStudyStorage(IPersistenceContext read, ServerEntityKey studyStorageKey)
        {
            Platform.CheckForNullReference(studyStorageKey, "studyStorageKey");

            ArchiveStudyStorageAdaptor adaptor = new ArchiveStudyStorageAdaptor();
            ArchiveStudyStorageSelectCriteria archiveStudyStorageCriteria = new ArchiveStudyStorageSelectCriteria();
            archiveStudyStorageCriteria.StudyStorageKey.EqualTo(studyStorageKey);
        	archiveStudyStorageCriteria.ArchiveTime.SortDesc(0);

            return adaptor.Get(read, archiveStudyStorageCriteria);
        }

        public IList<StudyStorageLocation> GetStudyStorageLocation(Study study)
        {
            Platform.CheckForNullReference(study, "Study");

            
            IQueryStudyStorageLocation select = HttpContextData.Current.ReadContext.GetBroker<IQueryStudyStorageLocation>();
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
		public StudyStorage GetStudyStorage(Study study)
		{
			Platform.CheckForNullReference(study, "Study");

			StudyStorageAdaptor studyStorageAdaptor = new StudyStorageAdaptor();
			StudyStorageSelectCriteria criteria = new StudyStorageSelectCriteria();
			criteria.ServerPartitionKey.EqualTo(study.ServerPartitionKey);
			criteria.StudyInstanceUid.EqualTo(study.StudyInstanceUid);

			return studyStorageAdaptor.GetFirst(criteria);
		}   

        public void ReprocessStudy(String reason, ServerEntityKey key)
        {
            StudyStorageAdaptor adaptor = new StudyStorageAdaptor();
            StudyStorage storage = adaptor.Get(key);
            StudyStorageLocation storageLocation = StudyStorageLocation.FindStorageLocations(storage)[0];
            StudyReprocessor reprocessor = new StudyReprocessor();
            reprocessor.ReprocessStudy(reason, storageLocation, Platform.Time, WorkQueuePriorityEnum.Medium);
        }

        #endregion

        
    }
}
