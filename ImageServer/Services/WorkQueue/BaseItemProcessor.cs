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
using System.IO;
using System.Text;
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core.Process;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ExecutionContext=ClearCanvas.ImageServer.Common.CommandProcessor.ExecutionContext;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    public class WorkQueueAlertContextData
    {
        #region Private Members
        private String _workQueueItemKey;
        private StudyInfo _studyInfo; 
        #endregion

        #region Public Properties
        public String WorkQueueItemKey
        {
            get { return _workQueueItemKey; }
            set { _workQueueItemKey = value; }
        }

        public StudyInfo StudyInfo
        {
            get { return _studyInfo; }
            set { _studyInfo = value; }
        } 
        #endregion
    }


	/// <summary>
	/// Enum telling if a work queue entry had a fatal or nonfatal error.
	/// </summary>
	public enum WorkQueueProcessorFailureType
	{
		Fatal,
		NonFatal
	}

	/// <summary>
	/// Enum for telling when processing is complete for a WorkQueue item.
	/// </summary>
	public enum WorkQueueProcessorStatus
	{
		Complete,
		Pending,
		Idle,
		IdleNoDelete,
        CompleteDelayDelete
	}

	/// <summary>
	/// Flag telling if a database update should be done for a WorkQueue entry.
	/// </summary>
	public enum WorkQueueProcessorDatabaseUpdate
	{
		ResetQueueState,
		None
	}



    /// <summary>
    /// Base class used when implementing WorkQueue item processors.
    /// </summary>
    public abstract class BaseItemProcessor: IWorkQueueItemProcessor
    {
        #region Static Fields
        private static readonly Dictionary<Type, StudyIntegrityValidationModes> _processorsValidationSettings = new Dictionary<Type, StudyIntegrityValidationModes>();
        private static readonly Dictionary<Type, RecoveryModes> _processorsRecoverySettings = new Dictionary<Type, RecoveryModes>();
        
        #endregion

        #region Private Fields
        private string _name = "Work Queue";
        private IReadContext _readContext;
        private TimeSpanStatistics _storageLocationLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _uidsLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _dBUpdateTime = new TimeSpanStatistics();
        private TimeSpanStatistics _studyXmlLoadTime = new TimeSpanStatistics();
        private TimeSpanStatistics _processTime = new TimeSpanStatistics();
        private Model.WorkQueue _workQueueItem;
        private StudyStorageLocation _storageLocation;
        private IList<WorkQueueUid> _uidList;
        private ServerPartition _partition;
        private Study _theStudy;
    	private bool _cancelPending = false;
    	private readonly object _syncRoot = new object();
        private StudyIntegrityValidationModes _ValidationModes = StudyIntegrityValidationModes.None;

        #endregion

        #region Constructors
        public BaseItemProcessor()
        {
            _ValidationModes = GetValidationTypes();
        }
        #endregion

        #region Protected Properties

        protected StudyStorageLocation StorageLocation
        {
            get { return _storageLocation; }
			set { _storageLocation = value; }
        }

        protected IReadContext ReadContext
        {
            get
            {
                if (_readContext==null)
                {
                    _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
                    
                }
                return _readContext;
            }
        }

        protected IList<WorkQueueUid> WorkQueueUidList
        {
            get
            {
                if (_uidList==null)
                {
                    LoadUids(WorkQueueItem);
                }
                return _uidList;
            }
        }

        protected TimeSpanStatistics StorageLocationLoadTime
        {
            get { return _storageLocationLoadTime; }
            set { _storageLocationLoadTime = value; }
        }

        protected TimeSpanStatistics UidsLoadTime
        {
            get { return _uidsLoadTime; }
            set { _uidsLoadTime = value; }
        }

        protected TimeSpanStatistics DBUpdateTime
        {
            get { return _dBUpdateTime; }
            set { _dBUpdateTime = value; }
        }

        protected TimeSpanStatistics StudyXmlLoadTime
        {
            get { return _studyXmlLoadTime; }
            set { _studyXmlLoadTime = value; }
        }

        protected TimeSpanStatistics ProcessTime
        {
            get { return _processTime; }
            set { _processTime = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        protected Model.WorkQueue WorkQueueItem
        {
            get { return _workQueueItem; }
        }

        protected ServerPartition ServerPartition
        {
            get
            {
                if (_workQueueItem == null || _workQueueItem.ServerPartitionKey == null)
                    return null;

                if (_partition==null)
                {
                    _partition = ServerPartitionMonitor.Instance.FindPartition(_workQueueItem.ServerPartitionKey); ;
                }

                return _partition;
            }
        }

    	protected bool CancelPending
    	{
			get { lock (_syncRoot) return _cancelPending; }
    	}

        protected Study Study
        {
            get
            {
                lock(_syncRoot)
                {
                    if (_theStudy==null)
                    {
                        _theStudy = WorkQueueItem.LoadStudy(ReadContext);
                    }
                }
                return _theStudy;
            }
        }

        protected StudyIntegrityValidationModes ValidationModes
        {
            get { return _ValidationModes; }
            set { _ValidationModes = value; }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Load the storage location for the WorkQueue item.
        /// </summary>
        /// <param name="item">The item to load the location for.</param>
        protected bool LoadStorageLocation(Model.WorkQueue item)
        {
        	bool found = false;
            StorageLocationLoadTime.Add(
                delegate
                    {
                    	found = FilesystemMonitor.Instance.GetOnlineStudyStorageLocation(item.StudyStorageKey, out _storageLocation);
                    }
                );

        	return found;
        }

        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        /// <param name="item">The WorkQueue item.</param>
        protected void LoadUids(Model.WorkQueue item)
        {
            
            if (_uidList==null)
            {
                UidsLoadTime.Add(delegate
                        {
                            IWorkQueueUidEntityBroker select = ReadContext.GetBroker<IWorkQueueUidEntityBroker>();

                            WorkQueueUidSelectCriteria parms = new WorkQueueUidSelectCriteria();

                            parms.WorkQueueKey.EqualTo(item.GetKey());
                            _uidList = select.Find(parms);

                            _uidList = TruncateList(item, _uidList);
                        }
                );
            }
        }

        /// <summary>
        /// Returns the max batch size for a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item to be processed</param>
        /// <returns>The maximum batch size for the <see cref="WorkQueue"/> item</returns>
        /// <param name="listCount">The number of available list items.</param>
        protected static int GetMaxBatchSize(Model.WorkQueue item, int listCount)
        {
            int maxSize;

            WorkQueueSettings settings = WorkQueueSettings.Instance;

            if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.Low)
            {
                maxSize = settings.LowPriorityMaxBatchSize;
            }
            else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.Medium)
            {
                maxSize = settings.MedPriorityMaxBatchSize;
            }
            else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.High)
            {
                maxSize = listCount;
            }
            else
            {
                maxSize = settings.MedPriorityMaxBatchSize;
            }

            return maxSize;
        }

        /// <summary>
        /// Truncate the SOP Instance Uid list
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item to be processed</param>
        /// <param name="list">The list of <see cref="WorkQueueUid"/> to be truncated, if needed</param>
        /// <return>A truncated list of <see cref="WorkQueueUid"/></return>
        protected static IList<WorkQueueUid> TruncateList(Model.WorkQueue item, IList<WorkQueueUid> list)
        {
			if (item != null && list != null)
			{
				int maxSize = GetMaxBatchSize(item, list.Count);
				if (list.Count <= maxSize)
					return list;

				List<WorkQueueUid> newList = new List<WorkQueueUid>();
				foreach (WorkQueueUid uid in list)
				{
					if (!uid.Failed)
						newList.Add(uid);

					if (newList.Count >= maxSize)
						return newList;
				}

				// just return the whole list, they're all going to be skipped anyways!
				if (newList.Count == 0)
					return list;

				return newList;
			}

			return list;
        }

        /// <summary>
        /// Updates the status of a study to a new status
        /// </summary>
		protected virtual void UpdateStudyStatus(StudyStorageLocation theStudyStorage, StudyStatusEnum theStatus, TransferSyntax theSyntax)
        {
        	DBUpdateTime.Add(
        		delegate
        			{
        				using (
        					IUpdateContext updateContext =
        						PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
        				{
							// Select the Server Transfer Syntax
							ServerTransferSyntaxSelectCriteria syntaxCriteria = new ServerTransferSyntaxSelectCriteria();
							IServerTransferSyntaxEntityBroker syntaxBroker =
								updateContext.GetBroker<IServerTransferSyntaxEntityBroker>();
							syntaxCriteria.Uid.EqualTo(theSyntax.UidString);

							ServerTransferSyntax serverSyntax = syntaxBroker.FindOne(syntaxCriteria);
							if (serverSyntax == null)
							{
								Platform.Log(LogLevel.Error, "Unable to load ServerTransferSyntax for {0}.  Unable to update study status.", theSyntax.Name);
								return;
							}

							// Get the FilesystemStudyStorage update broker ready
        					IFilesystemStudyStorageEntityBroker filesystemQueueBroker =
								updateContext.GetBroker<IFilesystemStudyStorageEntityBroker>();
							FilesystemStudyStorageUpdateColumns filesystemQueueUpdate = new FilesystemStudyStorageUpdateColumns();
        					filesystemQueueUpdate.ServerTransferSyntaxKey = serverSyntax.GetKey();
							FilesystemStudyStorageSelectCriteria filesystemQueueCriteria = new FilesystemStudyStorageSelectCriteria();
        					filesystemQueueCriteria.StudyStorageKey.EqualTo(theStudyStorage.GetKey());

							// Get the StudyStorage update broker ready
        					IStudyStorageEntityBroker studyStorageBroker =
        						updateContext.GetBroker<IStudyStorageEntityBroker>();
							StudyStorageUpdateColumns studyStorageUpdate = new StudyStorageUpdateColumns();
        					studyStorageUpdate.StudyStatusEnum = theStatus;
        					studyStorageUpdate.LastAccessedTime = Platform.Time;

							if (!filesystemQueueBroker.Update(filesystemQueueCriteria,filesystemQueueUpdate))
							{
								Platform.Log(LogLevel.Error, "Unable to update FilesystemQueue row: Study {0}, Server Entity {1}",
											 theStudyStorage.StudyInstanceUid, theStudyStorage.ServerPartitionKey);
								
							}
							else if (!studyStorageBroker.Update(theStudyStorage.GetKey(),studyStorageUpdate))
							{
								Platform.Log(LogLevel.Error, "Unable to update StudyStorage row: Study {0}, Server Entity {1}",
											 theStudyStorage.StudyInstanceUid, theStudyStorage.ServerPartitionKey);								
							}
							else
								updateContext.Commit();
        				}
        			}
        		);
        }

		/// <summary>
		/// Set a status of <see cref="WorkQueue"/> item after batch processing has been completed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will set the status of the <paramref name="item"/> to one of the followings
		/// <list type="bullet">
		/// <item>Failed: if the current process failed and number of retries has been reached.</item>
		/// <item>Pending: if the current batch has been processed successfully</item>
		/// <item>Idle : if current batch size = 0.</item>
		/// <item>Completed: if batch size =0 (idle) and the item has expired.</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <param name="item">The <see cref="WorkQueue"/> item to set.</param>
		/// <param name="status">Indicates if complete.</param>
		/// <param name="resetQueueStudyState">Reset the queue study state back to Idle</param>
		protected virtual void PostProcessing(Model.WorkQueue item, WorkQueueProcessorStatus status, WorkQueueProcessorDatabaseUpdate resetQueueStudyState)
		{
		    bool completed = status == WorkQueueProcessorStatus.Complete
		                     || (status == WorkQueueProcessorStatus.Idle && item.ExpirationTime < Platform.Time);
            if (completed)
            {
                VerifyStudy();
            }

			DBUpdateTime.Add(
				delegate
				{
					using (
						IUpdateContext updateContext =
							PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
					{
						IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
						UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters();
						parms.WorkQueueKey = item.GetKey();
						parms.StudyStorageKey = item.StudyStorageKey;
						parms.ProcessorID = item.ProcessorID;

						WorkQueueSettings settings = WorkQueueSettings.Instance;


						DateTime scheduledTime;

						if (item.FailureDescription != null)
							parms.FailureDescription = item.FailureDescription;

						if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.Low)
						{
							scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayLowPrioritySeconds);
						}
						else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.High)
						{
							scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayHighPrioritySeconds);
						}
						else
						{
							scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayMedPrioritySeconds);
						}


						if (scheduledTime > item.ExpirationTime)
							scheduledTime = item.ExpirationTime;

                        if (status == WorkQueueProcessorStatus.CompleteDelayDelete)
                        {
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Idle;
                            parms.FailureCount = item.FailureCount;
                            parms.FailureDescription = "";
                            parms.ScheduledTime = parms.ExpirationTime = Platform.Time.AddSeconds(settings.CompletedWorkQueueDelayDeleteSeconds);
                            if (resetQueueStudyState == WorkQueueProcessorDatabaseUpdate.ResetQueueState)
                                parms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;
                        }
                        else if (status == WorkQueueProcessorStatus.Complete
							|| (status == WorkQueueProcessorStatus.Idle && item.ExpirationTime < Platform.Time))
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Completed;
							parms.FailureCount = item.FailureCount;
							parms.ScheduledTime = scheduledTime;
							if (resetQueueStudyState == WorkQueueProcessorDatabaseUpdate.ResetQueueState)
								parms.QueueStudyStateEnum = QueueStudyStateEnum.Idle;

							parms.ExpirationTime = item.ExpirationTime; // Keep the same
						    completed = true;
						}
						else if (status == WorkQueueProcessorStatus.Idle
						      || status == WorkQueueProcessorStatus.IdleNoDelete)
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Idle;
							parms.ScheduledTime = scheduledTime;
							parms.ExpirationTime = item.ExpirationTime; // keep the same
							parms.FailureCount = item.FailureCount;
						}
						else
						{
							parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;

							parms.ExpirationTime = scheduledTime.AddSeconds(settings.WorkQueueExpireDelaySeconds);
							parms.ScheduledTime = scheduledTime;
							parms.FailureCount = item.FailureCount;
						}


						if (false == update.Execute(parms))
						{
							Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue Key: {1}", item.WorkQueueTypeEnum, item.Key.ToString());
						}
						else
							updateContext.Commit();
					}
				}
				);

            if (completed)
            {
                VerifyStudy();
            }
		}

        /// <summary>
        /// Gets the <see cref="QueueStudyStateEnum"/> value to be used for the specified work queue item status
        /// </summary>
        /// <param name="workQueueStatus"></param>
        /// <returns></returns>
        protected virtual QueueStudyStateEnum GetQueryStudyState(WorkQueueStatusEnum workQueueStatus)
        {
            if (workQueueStatus == WorkQueueStatusEnum.Completed)
                return QueueStudyStateEnum.Idle;
            else
                return null;
        }

    	/// <summary>
		/// Set a status of <see cref="WorkQueue"/> item after batch processing has been completed.
		/// </summary>
		/// <remarks>
		/// <para>
		/// This routine will set the status of the <paramref name="item"/> to one of the following
		/// <list type="bullet">
		/// <item>Failed: if the current process failed and number of retries has been reached or a fatal error.</item>
		/// <item>Pending: if the number of retries has not been reached</item>
		/// </list>
		/// </para>
		/// </remarks>
		/// <param name="item">The <see cref="WorkQueue"/> item to set.</param>
		/// <param name="processorFailureType">The failure is unrecoverable</param>
		protected virtual void PostProcessingFailure(Model.WorkQueue item, WorkQueueProcessorFailureType processorFailureType)
		{
			DBUpdateTime.Add(
				delegate
					{
						using (
							IUpdateContext updateContext =
								PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
						{
							IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
							UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters();
							parms.WorkQueueKey = item.GetKey();
							parms.StudyStorageKey = item.StudyStorageKey;
							parms.ProcessorID = item.ProcessorID;

							WorkQueueSettings settings = WorkQueueSettings.Instance;

							if (item.FailureDescription != null)
								parms.FailureDescription = item.FailureDescription;

							parms.FailureCount = item.FailureCount + 1;
							if (processorFailureType == WorkQueueProcessorFailureType.Fatal)
							{
								Platform.Log(LogLevel.Error,
											 "Failing {0} WorkQueue entry ({1}), fatal error: {2}",
											 item.WorkQueueTypeEnum, item.GetKey(), item.FailureDescription);

								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
								parms.ScheduledTime = Platform.Time;
								parms.ExpirationTime = Platform.Time; // expire now		

                                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Critical, Name, AlertTypeCodes.UnableToProcess,
                                                GetWorkQueueContextData(WorkQueueItem), TimeSpan.Zero,
                                               "Failing {0} WorkQueue entry ({1}), fatal error: {2}",
                                               item.WorkQueueTypeEnum, item.GetKey(), item.FailureDescription);
							}
							else if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
							{
								Platform.Log(LogLevel.Error,
                                             "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
											 item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, item.FailureDescription);
								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
								parms.ScheduledTime = Platform.Time;
								parms.ExpirationTime = Platform.Time; // expire now


                                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error, Name, AlertTypeCodes.UnableToProcess,
                                               GetWorkQueueContextData(WorkQueueItem), TimeSpan.Zero, "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
							                   item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, item.FailureDescription);
							}
							else
							{
								Platform.Log(LogLevel.Error,
                                             "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}.",
								             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
								parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
								parms.ScheduledTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);
								parms.ExpirationTime =
									Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount)*
									                         settings.WorkQueueFailureDelayMinutes);
							}


							if (false == update.Execute(parms))
							{
								Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}",
											 item.WorkQueueTypeEnum, item.GetKey().ToString());
							}
							else
								updateContext.Commit();
						}
					}
				);
		}

        

        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        /// <param name="failureDescription">The reason for the failure.</param>
        protected virtual void FailQueueItem(Model.WorkQueue item, string failureDescription)
        {
            DBUpdateTime.Add(
                delegate
                    {
                        using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                        {
                            IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                            UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters();
                            parms.ProcessorID = ServiceTools.ProcessorId;

                            parms.WorkQueueKey = item.GetKey();
                            parms.StudyStorageKey = item.StudyStorageKey;
                            parms.FailureCount = item.FailureCount + 1;
                            parms.FailureDescription = failureDescription;

                            WorkQueueSettings settings = WorkQueueSettings.Instance;
                            if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                            {
                                Platform.Log(LogLevel.Error,
                                             "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
                                             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, failureDescription);
                                parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Failed;
                                parms.ScheduledTime = Platform.Time;
                                parms.ExpirationTime = Platform.Time.AddDays(1);


                                ServerPlatform.Alert(AlertCategory.Application, AlertLevel.Error, Name, AlertTypeCodes.UnableToProcess,
                                                GetWorkQueueContextData(WorkQueueItem), TimeSpan.Zero, "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}. Failure Reason: {3}",
                                                item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1, failureDescription);
                            }
                            else
                            {
                                Platform.Log(LogLevel.Error,
                                             "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}",
                                             item.WorkQueueTypeEnum, item.GetKey(), item.FailureCount + 1);
                                parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                                parms.ScheduledTime = Platform.Time.AddMilliseconds(settings.WorkQueueQueryDelay);
                                parms.ExpirationTime =
                                    Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount) *
                                                             settings.WorkQueueFailureDelayMinutes);

                            }

                            if (false == update.Execute(parms))
                            {
                                Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum,
                                             item.GetKey().ToString());
                            }
                            else
                                updateContext.Commit();
                        }
                    }
                );
        }


    	/// <summary>
        /// Delete an entry in the <see cref="WorkQueueUid"/> table.
        /// </summary>
        /// <param name="sop">The <see cref="WorkQueueUid"/> entry to delete.</param>
        protected virtual void DeleteWorkQueueUid(WorkQueueUid sop)
        {
            DBUpdateTime.Add(
                TimeSpanStatisticsHelper.Measure(
                        delegate
                            {
                                using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                                {
                                    IWorkQueueUidEntityBroker delete = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                                    delete.Delete(sop.GetKey());

                                    updateContext.Commit();
                                }
                            }));
        }

        /// <summary>
        /// Update an entry in the <see cref="WorkQueueUid"/> table.
        /// </summary>
        /// <remarks>
        /// Note that just the Duplicate, Failed, FailureCount, and Extension columns are updated from the
        /// input parameter <paramref name="sop"/>.
        /// </remarks>
        /// <param name="sop">The <see cref="WorkQueueUid"/> entry to update.</param>
        protected virtual void UpdateWorkQueueUid(WorkQueueUid sop)
        {
            DBUpdateTime.Add(
                TimeSpanStatisticsHelper.Measure(
                delegate
                {
                     using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                     {
                         IWorkQueueUidEntityBroker update = updateContext.GetBroker<IWorkQueueUidEntityBroker>();

                         WorkQueueUidUpdateColumns columns = new WorkQueueUidUpdateColumns();

                         columns.Duplicate = sop.Duplicate;
                         columns.Failed = sop.Failed;
                         columns.FailureCount = sop.FailureCount;
                         if (sop.Extension != null)
                             columns.Extension = sop.Extension;

                         update.Update(sop.GetKey(), columns);

                         updateContext.Commit();
                     }
                }));

            
        }

        /// <summary>
        /// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="location">The location a study is stored.</param>
        /// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
        protected virtual StudyXml LoadStudyXml(StudyStorageLocation location)
        {
            StudyXml theXml = new StudyXml();

            StudyXmlLoadTime.Add(
                delegate
                    {
                        String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");
                        if (File.Exists(streamFile))
                        {
                            using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
                            {
                                XmlDocument theDoc = new XmlDocument();

                                StudyXmlIo.Read(theDoc, fileStream);

                                theXml.SetMemento(theDoc);

                                fileStream.Close();
                            }
                        }
                    }
                );

           return theXml;
        }

		protected virtual void PostponeItem(Model.WorkQueue item)
		{
			WorkQueueSettings settings = WorkQueueSettings.Instance;
			DateTime newScheduledTime = Platform.Time.AddMilliseconds(settings.WorkQueueQueryDelay);
			DateTime expireTime = newScheduledTime.Add(TimeSpan.FromSeconds(settings.WorkQueueExpireDelaySeconds));
			PostponeItem(item, newScheduledTime, expireTime);
		}

        protected virtual void PostponeItem(Model.WorkQueue item, DateTime newScheduledTime, DateTime expireTime)
        {
            DBUpdateTime.Add(
               delegate
               {
                   Platform.Log(LogLevel.Info, "Postpone {0} entry until {1}. [GUID={2}]", item.WorkQueueTypeEnum, newScheduledTime, item.GetKey());
                
                   using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                   {
                       IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                       UpdateWorkQueueParameters parms = new UpdateWorkQueueParameters();
                       parms.WorkQueueKey = item.GetKey();
                       parms.StudyStorageKey = item.StudyStorageKey;
                       parms.ProcessorID = ServiceTools.ProcessorId;
                       parms.WorkQueueStatusEnum = WorkQueueStatusEnum.Pending;
                       parms.ScheduledTime = newScheduledTime;
                       parms.ExpirationTime = expireTime;
                       parms.FailureCount = item.FailureCount;
                       
                       if (false == update.Execute(parms))
                       {
                           Platform.Log(LogLevel.Error, "Unable to reschedule {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum, item.GetKey().ToString());
                       }
                       else
                       {
                           updateContext.Commit();
                       }
                   }
               }
               );
        }


        protected List<Model.WorkQueue> FindRelatedWorkQueueItems(Model.WorkQueue item, WorkQueueSelectCriteria criteria)
        {
            IWorkQueueEntityBroker broker = ReadContext.GetBroker<IWorkQueueEntityBroker>();
            List<Model.WorkQueue> list = CollectionUtils.Cast<Model.WorkQueue>(broker.Find(criteria));
            return list.FindAll(delegate(Model.WorkQueue testItem)
                                    {
                                        return !testItem.GetKey().Equals(item.GetKey());
                                    });
        }


        protected static bool LockStudyState(Model.WorkQueue item, QueueStudyStateEnum state)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                ILockStudy lockStudyBroker = updateContext.GetBroker<ILockStudy>();
                LockStudyParameters lockStudyParams = new LockStudyParameters();
                lockStudyParams.StudyStorageKey = item.StudyStorageKey;
                lockStudyParams.QueueStudyStateEnum = state;

                if (!lockStudyBroker.Execute(lockStudyParams) || !lockStudyParams.Successful)
                    return false;

                else
                {
                    updateContext.Commit();
                    return true;
                }
            }
        }

        public IList<StudyIntegrityQueue> FindSIQEntries()
        {
            IStudyIntegrityQueueEntityBroker broker = ReadContext.GetBroker<IStudyIntegrityQueueEntityBroker>();
            StudyIntegrityQueueSelectCriteria criteria = new StudyIntegrityQueueSelectCriteria();
            criteria.StudyStorageKey.EqualTo(StorageLocation.GetKey());
            criteria.InsertTime.SortDesc(0);
            IList<StudyIntegrityQueue> list = broker.Find(criteria);
            return list;
        }


        /// <summary>
        /// Called by the base before <see cref="ProcessItem"/> is invoked to determine 
        /// if the process can begin.
        /// </summary>
        /// <returns>True if the processing can begin. False otherwise.</returns>
        /// <remarks>
        /// </remarks>
        protected abstract bool CanStart();

        /// <summary>
        /// Called by the base to initialize the processor.
        /// </summary>
        protected virtual void Initialize(Model.WorkQueue item)
        {
            
        }

        /// <summary>
        /// Called before the <see cref="WorkQueue"/> item is processed
        /// </summary>
        /// <param name="item">The work queue item to be processed.</param>
        protected virtual void OnProcessItemBegin(Model.WorkQueue item)
        {
            //NOOP
        }

        /// <summary>
        /// Called after the <see cref="WorkQueue"/> item has been processed
        /// </summary>
        /// <param name="item">The work queue item which has been processed.</param>
        protected virtual void OnProcessItemEnd(Model.WorkQueue item)
        {
            // NOOP
        }

        protected abstract void ProcessItem(Model.WorkQueue item);

        #endregion

		public void Cancel()
		{
			lock (_syncRoot)
				_cancelPending = true;
		}

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (_readContext!=null)
            {
                _readContext.Dispose();
                _readContext = null;
            }
        }

        public void Process(Model.WorkQueue item)
        {
            _workQueueItem = item;

            using (WorkQueueProcessorContext executionContext = new WorkQueueProcessorContext(item))
            {
                Initialize(item);

                if (!LoadStorageLocation(item))
            	{
            		Platform.Log(LogLevel.Warn,
            		             "Unable to find readable StorageLocation when processing {0} WorkQueue item, rescheduling",
            		             item.WorkQueueTypeEnum.Description);
					PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
					return;
            	}

                if (StorageLocation.QueueStudyStateEnum == QueueStudyStateEnum.ReprocessScheduled && !item.WorkQueueTypeEnum.Equals(WorkQueueTypeEnum.ReprocessStudy))
                {
                    Platform.Log(LogLevel.Info,
                                 "Study is being reprocessed. Postponing {0} entry", item.WorkQueueTypeEnum.Description);
                    PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
                    return;
                }

                if (CanStart())
                {
                    try
                    {
                        OnProcessItemBegin(item);

                        ProcessTime.Add(
                            delegate
                            {
                                ProcessItem(item);
                            }
                            );
                        OnProcessItemEnd(item);
                    }
                    catch(StudyIntegrityValidationFailure ex)
                    {
                        item.FailureDescription = ex.Message;
                        OnStudyIntegrityFailure(WorkQueueItem, ex.Message);
                    }
                    
                }
            }
        }

        protected void Log(LogLevel level, String header, string message)
        {
            Platform.Log(level, String.Format("{0} : {1}", header, message));
        }
        protected void Log(LogLevel level, String header, string message, params object[] args)
        {
            Platform.Log(level, String.Format("{0} : {1}", header, String.Format(message, args)));
        }

        protected void OnStudyIntegrityFailure(Model.WorkQueue item, string reason)
        {
            Platform.CheckMemberIsSet(Study, "Study"); 

            RecoveryModes mode = GetProcessorRecoveryMode();
            switch (mode)
            {
                case RecoveryModes.Automatic:
                    try
                    {
                        if (PerformAutoRecovery(item, reason))
                        {
                            Platform.Log(LogLevel.Info, "Auto-recovery was successful (some operations may still be pending). Current {0} entry will resume later.", item.WorkQueueTypeEnum);
                            PostponeItem(item);
                        }
                        else
                        {
                            // unable to recover.. fail it now
                            Platform.Log(LogLevel.Error, "Unable to recover from previous failure. Failing the entry.");
                            PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                        }
                    }
                    catch(Exception ex)
                    {
                        // unable to recover.. fail it now
                        Platform.Log(LogLevel.Error, ex, "Unable to recover from previous failure. Failing the entry.");
                        PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                    }
                    
                    break;
                default:
                    PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
                    break;
            }
            
        }

        protected static WorkQueueAlertContextData GetWorkQueueContextData(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            WorkQueueAlertContextData contextData = new WorkQueueAlertContextData();
            contextData.WorkQueueItemKey = item.GetKey().Key.ToString();

            StudyStorage storage = StudyStorage.Load(ExecutionContext.Current.PersistenceContext, item.StudyStorageKey);
            IList<StudyStorageLocation> locations = StudyStorageLocation.FindStorageLocations(storage);

            if (locations != null && locations.Count > 0)
            {
                StudyStorageLocation location = locations[0];
                if (location != null)
                {
                    contextData.StudyInfo = new StudyInfo();
                    contextData.StudyInfo.StudyInstaneUid = location.StudyInstanceUid;

                    // study info is not always available (eg, when all images failed to process)
                    if (location.Study != null)
                    {
                        contextData.StudyInfo.AccessionNumber = location.Study.AccessionNumber;
                        contextData.StudyInfo.PatientsId = location.Study.PatientId;
                        contextData.StudyInfo.PatientsName = location.Study.PatientsName;
                        contextData.StudyInfo.ServerAE = location.ServerPartition.AeTitle;
                        contextData.StudyInfo.StudyDate = location.Study.StudyDate;
                    }
                }

            }

            return contextData;
        }


        protected static void RaiseAlert(Model.WorkQueue item, AlertLevel level, string message, params object[] arguments)
        {
            WorkQueueAlertContextData alertContextData = GetWorkQueueContextData(item);
            StringBuilder alertMessage = new StringBuilder();
            if (alertContextData.StudyInfo != null)
            {
                alertMessage.AppendLine(String.Format("Study: {0}", alertContextData.StudyInfo.StudyInstaneUid));
                alertMessage.AppendLine(String.Format("A #: {0}", alertContextData.StudyInfo.AccessionNumber ?? "N/A"));
                alertMessage.AppendLine(String.Format("Patient: {0} , ID: {1}", alertContextData.StudyInfo.PatientsName ?? "N/A", alertContextData.StudyInfo.PatientsId ?? "N/A"));

            }
            else
            {
                alertMessage.AppendLine("Study: Unknown");
            }

            alertMessage.AppendLine(String.Format(message, arguments));


            ServerPlatform.Alert(AlertCategory.Application, level,
                item != null ? item.WorkQueueTypeEnum.ToString() : "WorkQueue",
                -1, alertContextData, TimeSpan.Zero, alertMessage.ToString());

        }

        protected void RemoveBadDicomFile(string file, string reason)
        {
            Platform.Log(LogLevel.Error, "Deleting unreadable dicom file: {0}. Reason={1}", file, reason);
            FileUtils.Delete(file);
            RaiseAlert(WorkQueueItem, AlertLevel.Critical, "Dicom file {0} is unreadable: {1}. It has been removed from the study.", file, reason);
        }

        protected virtual bool PerformAutoRecovery(Model.WorkQueue item, string reason)
        {
            
            Platform.Log(LogLevel.Info, "{4} failed. Reason:{5}. Attempting to perform auto-recovery for Study:{0}, A#:{1}, Patient:{2}, ID:{3}",
                         Study.StudyInstanceUid, Study.AccessionNumber, Study.PatientsName, Study.PatientId, item.WorkQueueTypeEnum.ToString(), reason);

            Platform.CheckForNullReference(StorageLocation, "StorageLocation");
            StudyXml studyXml = StorageLocation.LoadStudyXml();
            Platform.CheckForNullReference(studyXml, "studyXml");
            
            // Do a secondary check on the filesystem vs. the Study Xml.  
            // If these match, then the DB is just update to reflect the valid counts.  
            // If they don't match, a Reprocess entry is inserted into the WorkQueue.
            String studyFolder = StorageLocation.GetStudyPath();
            long fileCounter = DirectoryUtility.Count(studyFolder, "*.dcm", true, null);

            // cache these values to avoid looping again
            int numStudyRelatedSeriesInXml = studyXml.NumberOfStudyRelatedSeries;
            int numStudyRelatedInstancesInXml = studyXml.NumberOfStudyRelatedInstances;

            if (fileCounter != numStudyRelatedInstancesInXml)
            {
                // reprocess the study
                Log(LogLevel.Info, "AUTO-RECOVERY", "# of study related instances in study Xml ({0}) appears incorrect. Study needs to be reprocessed.", numStudyRelatedInstancesInXml);
                StudyReprocessor reprocessor = new StudyReprocessor();
                String reprocessReason = String.Format("Auto-recovery from {0}. {1}", item.WorkQueueTypeEnum, reason);
                reprocessor.ReprocessStudy(reprocessReason, StorageLocation, Platform.Time, WorkQueuePriorityEnum.High);

                return true; // 
            }
            else
            {
                Log(LogLevel.Info, "AUTO-RECOVERY", "# of study related instances in study Xml ({0}) appears correct. Update database based on study xml", numStudyRelatedInstancesInXml);
                // update the counts in db to match the study xml
                // Update count for each series 
                IList<Series> seriesList = StorageLocation.Study.Series;
               
                    
                foreach (Series series in seriesList)
                {
                    SeriesXml seriesXml = studyXml[series.SeriesInstanceUid];
                    if (seriesXml != null)
                    {
                        int numInstancesInSeriesXml = seriesXml.NumberOfSeriesRelatedInstances;
                        if (numInstancesInSeriesXml != series.NumberOfSeriesRelatedInstances)
                        {
                            // Update the count in the series table. Assuming the stored procedure
                            // will also update the count in the Study/Patient tables based on 
                            // the new value.
                            Log(LogLevel.Info, "AUTO-RECOVERY", "Update series in the db. UID:{0}, #Instance:{1}==>{2}", series.SeriesInstanceUid, series.NumberOfSeriesRelatedInstances, numInstancesInSeriesXml);
                            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                            {
                                ISetSeriesRelatedInstanceCount broker = updateContext.GetBroker<ISetSeriesRelatedInstanceCount>();
                                SetSeriesRelatedInstanceCountParameters criteria = new SetSeriesRelatedInstanceCountParameters(StorageLocation.GetKey(), series.SeriesInstanceUid);
                                criteria.SeriesRelatedInstanceCount = numInstancesInSeriesXml;
                                if (!broker.Execute(criteria))
                                {
                                    throw new ApplicationException("Unable to update series related instance count in db");
                                }
                                else
                                {
                                    updateContext.Commit();
                                }
                            }
                        }
                    }
                    else
                    {
                        // TODO: series in the db doesn't exist in the xml... the series was deleted from the study?
                        // For now we just reprocess the study. Can we delete the series? If so, we need to take care of the counts.
                        Log(LogLevel.Info, "AUTO-RECOVERY", "Found series in the db which does not exist in the study xml. Force to reprocess the study.");
                        StudyReprocessor reprocessor = new StudyReprocessor();
                        String reprocessReason = String.Format("Auto-recovery from {0}. {1}", item.WorkQueueTypeEnum, reason);
                        reprocessor.ReprocessStudy(reprocessReason, StorageLocation, Platform.Time, WorkQueuePriorityEnum.High);
                        
                    }
                }

                if (numStudyRelatedSeriesInXml!=StorageLocation.Study.NumberOfStudyRelatedSeries || 
                    numStudyRelatedInstancesInXml != StorageLocation.Study.NumberOfStudyRelatedInstances)
                {
                    Log(LogLevel.Info, "AUTO-RECOVERY", "Updating study related series and instance counts in the db. #Series: {0} ==> {1}.  #Instances: {2}==>{3}", 
                                    StorageLocation.Study.NumberOfStudyRelatedSeries, numStudyRelatedSeriesInXml,
                                    StorageLocation.Study.NumberOfStudyRelatedInstances, numStudyRelatedInstancesInXml);

                    using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                    {
                        ISetStudyRelatedInstanceCount broker = updateContext.GetBroker<ISetStudyRelatedInstanceCount>();
                        SetStudyRelatedInstanceCountParameters criteria = new SetStudyRelatedInstanceCountParameters(StorageLocation.GetKey());
                        criteria.StudyRelatedSeriesCount = numStudyRelatedSeriesInXml;
                        criteria.StudyRelatedInstanceCount = numStudyRelatedInstancesInXml;
                        if (!broker.Execute(criteria))
                        {
                            throw new ApplicationException("Unable to update study related instance count in db");
                        }
                        else
                        {
                            updateContext.Commit();
                        }
                    }
                }
                
                Log(LogLevel.Info, "AUTO-RECOVERY", "Object count in the db have been corrected.");
                return true;
            }

        }

        
        #endregion

        #region Private Methods

        private bool DeleteWorkQueueEntry(Model.WorkQueue item)
        {
            DateTime startTime = DateTime.Now;
            int retryCounter = 0;
            TimeSpan elapsed;
            do
            {
                Platform.Log(LogLevel.Info, "Deleting current work queue entry..");
                elapsed = DateTime.Now - startTime;

                using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
                {
                    try
                    {
                        item.Delete(updateContext);
                        updateContext.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        Platform.Log(LogLevel.Error, ex, "Error occured when trying to delete current work queue entry. Retry later.");
                        Random rand = new Random();
                        Thread.Sleep(rand.Next(100, 500));

                        if (elapsed > TimeSpan.FromMinutes(1))
                        {
                            if (retryCounter % 10 == 0) // reduce the frequency of the alerts
                                RaiseAlert(item, AlertLevel.Warning, "Server has failed to delete the work queue entry for the last {0}", TimeSpanFormatter.Format(elapsed, true));
                        }
                        retryCounter++;
                    }
                }


            }
            while (!CancelPending && elapsed < TimeSpan.FromMinutes(5));
            return false;
        }

        private RecoveryModes GetProcessorRecoveryMode()
        {
            RecoveryModes mode = RecoveryModes.Manual;

            if (!_processorsRecoverySettings.TryGetValue(GetType(), out mode))
            {
                lock(_processorsRecoverySettings)
                {
                    object[] attributes = GetType().GetCustomAttributes(typeof(StudyIntegrityValidationAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        StudyIntegrityValidationAttribute att = attributes[0] as StudyIntegrityValidationAttribute;
                        if (!_processorsRecoverySettings.ContainsKey(GetType()))
                        {
                            _processorsRecoverySettings.Add(GetType(), att.Recovery);
                        }
                        return att.Recovery;
                    }
                }
                
            }

            return mode;
        }

        private StudyIntegrityValidationModes GetValidationTypes()
        {
            if (!_processorsValidationSettings.TryGetValue(GetType(), out _ValidationModes))
            {
                object[] attributes = GetType().GetCustomAttributes(typeof(StudyIntegrityValidationAttribute), true);
                if (attributes != null && attributes.Length > 0)
                {
                    StudyIntegrityValidationAttribute att = attributes[0] as StudyIntegrityValidationAttribute;
                    _processorsValidationSettings.Add(GetType(), att.ValidationTypes);
                    return att.ValidationTypes;
                }
            }

            return _ValidationModes;
        }

        private void VerifyStudy()
        {
            if (!WorkQueueSettings.Instance.EnableStudyIntegrityValidation)
                return;

            if (ValidationModes == StudyIntegrityValidationModes.None)
                return;
            
            using(ExecutionContext scope = new ExecutionContext())
            {
                IList<StudyStorageLocation> locations = WorkQueueItem.LoadStudyLocations(scope.PersistenceContext);
                if (locations!=null)
                {
                    StudyStorageLocation location = locations[0];
                    StudyIntegrityValidator validator = new StudyIntegrityValidator();
                    validator.ValidateStudyState(WorkQueueItem.WorkQueueTypeEnum.ToString(), location, ValidationModes);
                }
            }
            
        }
        #endregion


    }
}
