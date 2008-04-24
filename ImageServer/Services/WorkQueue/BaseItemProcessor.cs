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
using System.Diagnostics;
using System.IO;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.Parameters;

namespace ClearCanvas.ImageServer.Services.WorkQueue
{
    /// <summary>
    /// Base class used when implementing WorkQueue item processors.
    /// </summary>
    public abstract class BaseItemProcessor : IWorkQueueItemProcessor
    {
        #region Private Members

        private IList<StudyStorageLocation> _storageLocationList;
        private IReadContext _readContext;
        private IList<WorkQueueUid> _uidList;

        #endregion

        #region Protected Properties

        protected IReadContext ReadContext
        {
            get { return _readContext; }
        }

        protected StudyStorageLocation StorageLocation
        {
            get { return _storageLocationList[0]; }
        }

        protected IList<StudyStorageLocation> StorageLocationList
        {
            get { return _storageLocationList; }
        }

        protected IList<WorkQueueUid> WorkQueueUidList
        {
            get { return _uidList; }
        }

        #endregion

        #region Contructors

        public BaseItemProcessor()
        {
            _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Load the storage location for the WorkQueue item.
        /// </summary>
        /// <param name="item">The item to load the location for.</param>
        protected void LoadStorageLocation(Model.WorkQueue item)
        {
            IQueryStudyStorageLocation select = _readContext.GetBroker<IQueryStudyStorageLocation>();

            StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
            parms.StudyStorageKey = item.StudyStorageKey;

            _storageLocationList = select.Execute(parms);

            if (_storageLocationList.Count == 0)
            {
                Platform.Log(LogLevel.Error, "Unable to find storage location for WorkQueue item: {0}",
                             item.GetKey().ToString());
                throw new ApplicationException("Unable to find storage location for WorkQueue item.");
            }

            Debug.Assert(_storageLocationList.Count>0);
        }

        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        /// <param name="item">The WorkQueue item.</param>
        protected void LoadUids(Model.WorkQueue item)
        {
            IQueryWorkQueueUids select = _readContext.GetBroker<IQueryWorkQueueUids>();

            WorkQueueUidQueryParameters parms = new WorkQueueUidQueryParameters();

            parms.WorkQueueKey = item.GetKey();
            _uidList = select.Execute(parms);

            _uidList = TruncateList(item, _uidList);
        }

        /// <summary>
        /// Returns the max batch size for a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item to be processed</param>
        /// <returns>The maximum batch size for the <see cref="WorkQueue"/> item</returns>
        protected static int GetMaxBatchSize(Model.WorkQueue item)
        {
            int maxSize = 0;

            WorkQueueSettings settings = WorkQueueSettings.Default;

            if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.GetEnum("Low"))
            {
                maxSize = settings.LowPriorityMaxBatchSize;
            }
            else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.GetEnum("Medium"))
            {
                maxSize = settings.MedPriorityMaxBatchSize;
            }
            else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.GetEnum("High"))
            {
                maxSize = settings.HighPriorityMaxBatchSize;
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
            if (item!=null && list!=null)
            {
                int maxSize = GetMaxBatchSize(item);
                if (list.Count > maxSize)
                {
                    IList<WorkQueueUid> newList = new List<WorkQueueUid>();
                    for (int i = 0; i < maxSize; i++)
                        newList.Add(list[i]);
                    return newList;
                }
            }

            return list;
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
        /// <param name="batchSize">Number of workqueue uids associated with the workqueue item (process batch size).</param>
        /// <param name="failed">Indicates if the current batch was not processed successfully.</param>
        protected virtual void PostProcessing(Model.WorkQueue item, int batchSize, bool failed)
        {
            using (
                IUpdateContext updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
                parms.WorkQueueKey = item.GetKey();
                parms.StudyStorageKey = item.StudyStorageKey;
                parms.ProcessorID = item.ProcessorID;

                WorkQueueSettings settings = WorkQueueSettings.Default;

                if (failed)
                {
                    parms.FailureCount = item.FailureCount + 1;
                    if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                    {
                        Platform.Log(LogLevel.Error,
                                     "Failing StudyProcess WorkQueue entry ({0}), reached max retry count of {1}",
                                     item.GetKey(), item.FailureCount + 1);
                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Failed");
                        parms.ScheduledTime = Platform.Time;
                        parms.ExpirationTime = Platform.Time; // expire now
                    }
                    else
                    {
                        Platform.Log(LogLevel.Error,
                                     "Resetting StudyProcess WorkQueue entry ({0}) to Pending, current retry count {1}",
                                     item.GetKey(), item.FailureCount + 1);
                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Pending");
                        parms.ScheduledTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);
                        parms.ExpirationTime =
                            Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount)*
                                                     settings.WorkQueueFailureDelayMinutes);
                    }
                }
                else
                {
                    DateTime scheduledTime = Platform.Time;

                    if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.GetEnum("Low"))
                    {
                        scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayLowPrioritySeconds);
                    }
                    else if (item.WorkQueuePriorityEnum == WorkQueuePriorityEnum.GetEnum("High"))
                    {
                        scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayHighPrioritySeconds);
                    }
                    else 
                    {
                        scheduledTime = Platform.Time.AddSeconds(settings.WorkQueueProcessDelayMedPrioritySeconds);
                    }


                    if (scheduledTime > item.ExpirationTime)
                        scheduledTime = item.ExpirationTime;


                    if (batchSize == 0 && item.ExpirationTime < Platform.Time)
                    {
                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Completed");
                        parms.FailureCount = item.FailureCount;
                        parms.ScheduledTime = scheduledTime;
                        parms.ExpirationTime = item.ExpirationTime; // Keep the same
                    }
                    else
                    {
                        // If the batch size is 0, switch to idle state.
                        if (batchSize == 0)
                        {
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Idle");
                            parms.ScheduledTime = scheduledTime;
                            parms.ExpirationTime = item.ExpirationTime; // keep the same
                            parms.FailureCount = item.FailureCount;
                        }
                        else
                        {
                            parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Pending");

                            parms.ExpirationTime = scheduledTime.AddSeconds(settings.WorkQueueExpireDelaySeconds);
                            parms.ScheduledTime = scheduledTime;
                            parms.FailureCount = item.FailureCount;
                        }
                    }
                }


                if (false == update.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unable to update StudyProcess WorkQueue GUID: {0}",
                                 item.GetKey().ToString());
                }
                else
                    updateContext.Commit();
            }

        }

        /// <summary>
        /// Delete an entry in the <see cref="WorkQueueUid"/> table.
        /// </summary>
        /// <param name="sop">The <see cref="WorkQueueUid"/> entry to delete.</param>
        protected static void DeleteWorkQueueUid(WorkQueueUid sop)
        {
            using (
                IUpdateContext updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IDeleteWorkQueueUid delete = updateContext.GetBroker<IDeleteWorkQueueUid>();

                WorkQueueUidDeleteParameters parms = new WorkQueueUidDeleteParameters();
                parms.WorkQueueUidKey = sop.GetKey();

                delete.Execute(parms);

                updateContext.Commit();
            }
        }

        /// <summary>
        /// Simple routine for failing a work queue item.
        /// </summary>
        /// <param name="item">The item to fail.</param>
        protected static void FailQueueItem(Model.WorkQueue item)
        {
            using (
                IUpdateContext updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateWorkQueue update = updateContext.GetBroker<IUpdateWorkQueue>();
                WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
                parms.ProcessorID = ServiceTools.ProcessorId;

                parms.WorkQueueKey = item.GetKey();
                parms.StudyStorageKey = item.StudyStorageKey;
                parms.FailureCount = item.FailureCount + 1;

                WorkQueueSettings settings = WorkQueueSettings.Default;
                if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                {
                    Platform.Log(LogLevel.Error,
                                 "Failing {0} WorkQueue entry ({1}), reached max retry count of {2}",
                                 item.WorkQueueTypeEnum.Description, item.GetKey(), item.FailureCount + 1);
                    parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Failed");
                    parms.ScheduledTime = Platform.Time;
                    parms.ExpirationTime = Platform.Time.AddDays(1);
                }
                else
                {
                    Platform.Log(LogLevel.Error,
                                 "Resetting {0} WorkQueue entry ({1}) to Pending, current retry count {2}",
                                 item.WorkQueueTypeEnum.Description, item.GetKey(), item.FailureCount + 1);
                    parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Pending");
                    parms.ScheduledTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);
                    parms.ExpirationTime =
                        Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount)*
                                                 settings.WorkQueueFailureDelayMinutes);
                }

                if (false == update.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unable to update {0} WorkQueue GUID: {1}", item.WorkQueueTypeEnum.Name,
                                 item.GetKey().ToString());
                }

                updateContext.Commit();
            }
        }

        /// <summary>
        /// Load a <see cref="StudyXml"/> file for a given <see cref="StudyStorageLocation"/>
        /// </summary>
        /// <param name="location">The location a study is stored.</param>
        /// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
        protected static StudyXml LoadStudyXml(StudyStorageLocation location)
        {
            String streamFile = Path.Combine(location.GetStudyPath(), location.StudyInstanceUid + ".xml");

            StudyXml theXml = new StudyXml();

            if (File.Exists(streamFile))
            {
                using (Stream fileStream = new FileStream(streamFile, FileMode.Open))
                {
                    XmlDocument theDoc = new XmlDocument();

                    StudyXmlIo.Read(theDoc, fileStream);

                    theXml.SetMemento(theDoc);

                    fileStream.Close();
                }
            }

            return theXml;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public void Dispose()
        {
            if (_readContext != null)
            {
                _readContext.Dispose();
                _readContext = null;
            }
        }

        #endregion

        #region IWorkQueueItemProcessor members

        public abstract void Process(Model.WorkQueue item);

        #endregion IWorkQueueItemProcessor members
    }
}
