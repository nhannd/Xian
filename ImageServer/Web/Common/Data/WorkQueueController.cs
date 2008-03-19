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
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Web.Common.Data;

namespace ClearCanvas.ImageServer.Web.Common.Data
{
    /// <summary>
    /// Provides methods to interact with the database to query/update work queue.
    /// </summary>
    public class WorkQueueController : BaseController
    {
        #region Private Members

        
        #endregion

        #region Protected Methods

        /// <summary>
        /// Deletes all <see cref="WorkQueueUid"/> associated with a specified <see cref="WorkQueue"/>
        /// </summary>
        /// <param name="uctx">The update context for current operation</param>
        /// <param name="item">The <see cref="WorkQueue"/> item whose <see cref="WorkQueueUid"/> to be deleted</param>
        /// <param name="uidList">The list of <see cref="WorkQueueUid"/> to be deleted</param>
        /// <param name="deleteFiles">A value indicating whether the DICOM files associated with the <see cref="WorkQueueUid"/> in the <paramref name="uidList"/> should be deleted as well</param>
        /// <returns>a value indicating whether all <see cref="WorkQueueUid"/> in <paramref name="uidList"/> are deleted from the database. If <paramref name="deleteFiles"/> is 
        /// <b>true</b> then the method returns a <b>true</b>only if all associated DICOM files are successfully deleted as well.</returns>
        static protected bool DeleteWorkQueueUids(IUpdateContext uctx, WorkQueue item, IList<WorkQueueUid> uidList, bool deleteFiles)
        {
            bool result = true;
            int fileCounter = 0;

            StudyStorageLocation storage = GetLoadStorageLocation(uctx, item);

            IWorkQueueUidEntityBroker workQueueUidBroker = uctx.GetBroker<IWorkQueueUidEntityBroker>();
            try
            {
                Platform.Log(LogLevel.Debug, "Deleting work queue uid {0}", item.GetKey().Key);

                foreach (WorkQueueUid uid in uidList)
                {
                    try
                    {
                        if (deleteFiles)
                        {
                            string path = Path.Combine(storage.GetStudyPath(), uid.SeriesInstanceUid);
                            path = Path.Combine(path, uid.SopInstanceUid + ".dcm");
                            File.Delete(path);

                            fileCounter++;
                        }


                        if (false == workQueueUidBroker.Delete(uid.GetKey()))
                        {
                            result = false;
                            break;
                        }
                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, "DeleteWorkQueueUids(): Error occurred while trying to delete the work queue entry: GUID= {0}, Uid GUID={1}, Sop UID={2}. Error= {4}",
                                     item.GetKey().Key, uid.GetKey().Key, uid.SopInstanceUid, e.StackTrace);
                        return false;
                    }
                }
            }
            finally
            {
                if (deleteFiles)
                    Platform.Log(LogLevel.Info, "{0} files deleted for work queue {0}", item.GetKey().Key);

            }


            return result;
        }


        /// <summary>
        /// Gets the <see cref="StudyStorageLocation"/> for the study associated with the specified <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="uctx">The update context for current operation</param>
        /// <param name="item"></param>
        /// <returns></returns>
        static protected StudyStorageLocation GetLoadStorageLocation(IUpdateContext uctx, WorkQueue item)
        {
            IQueryStudyStorageLocation select = uctx.GetBroker<IQueryStudyStorageLocation>();

            StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
            parms.StudyStorageKey = item.StudyStorageKey;

            IList<StudyStorageLocation> storages = select.Execute(parms);

            if (storages == null || storages.Count == 0)
            {
                Platform.Log(LogLevel.Error, "Unable to find storage location for WorkQueue item: {0}", item.GetKey().ToString());
                throw new ApplicationException("Unable to find storage location for WorkQueue item.");
            }

            if (storages.Count > 1)
            {
                Platform.Log(LogLevel.Warn,
                             "WorkQueueController:LoadStorageLocation: multiple study storage found for work queue item {0}",
                             item.GetKey().Key);
            }

            return storages[0];
        }

        #endregion Protected Methods

        #region Public Methods

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually rescheduled.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanReschedule(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                // it's pending
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("Pending")
                // it's idle
                || item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("Idle")
                /* somebody claimed it but hasn't updated it for quite a while */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress") &&
                    !String.IsNullOrEmpty(item.ProcessorID) &&
                    item.ScheduledTime < Platform.Time.Subtract(TimeSpan.FromHours(12)));
        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually reset.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanReset(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                /* failed item */
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("Failed")
                /* nobody claimed it */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress") && String.IsNullOrEmpty(item.ProcessorID))
                /* somebody claimed it but hasn't updated it for quite a while */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress") &&
                    !String.IsNullOrEmpty(item.ProcessorID) &&
                    item.ScheduledTime < Platform.Time.Subtract(TimeSpan.FromHours(12)));

        }

        /// <summary>
        /// Returns a value indicating whether the specified <see cref="WorkQueue"/> can be manually deleted from the queue.
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        static public bool CanDelete(WorkQueue item)
        {
            if (item == null)
                return false;

            return
                /* failed item */
                item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("Failed")
                /* completed item */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("Completed"))
                /* nobody claimed it */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress") && String.IsNullOrEmpty(item.ProcessorID))
                /* somebody claimed it but hasn't updated it for quite a while */
                || (item.WorkQueueStatusEnum == WorkQueueStatusEnum.GetEnum("In Progress") && !String.IsNullOrEmpty(item.ProcessorID) &&
                    item.ScheduledTime < Platform.Time.Subtract(TimeSpan.FromHours(12)));
        }




        

        /// <summary>
        /// Gets a list of <see cref="WorkQueue"/> items with specified criteria
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public IList<WorkQueue> FindWorkQueue(WebWorkQueueQueryParameters parameters)
        {
            try
            {
                IList<WorkQueue> list = null;
                
                IPersistentStore _store = PersistentStoreRegistry.GetDefaultStore();

                using (IReadContext ctx = _store.OpenReadContext())
                {
                    IWebQueryWorkQueue broker = ctx.GetBroker<IWebQueryWorkQueue>();
                    list = broker.Execute(parameters);
                }

                return list;
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Error, "FindWorkQueue failed", e);
            }

            return null;
            
        }

        /// <summary>
        /// Deletes a list of <see cref="WorkQueue"/> items from the system.
        /// </summary>
        /// <param name="items">The list of <see cref="WorkQueue"/> items to be deleted</param>
        /// <returns>A value indicating whether all items have been successfully deleted.</returns>
        /// 
        /// <remarkks>
        /// If one or more <see cref="WorkQueue"/> in <paramref name="items"/> cannot be deleted, the method will return <b>false</b>
        /// and the deletion will be undone (i.e., All of the <see cref="WorkQueue"/> items will remain in the database)
        /// </remarkks>
        public bool DeleteWorkQueueItems(IList<WorkQueue> items)
        {
            if (items == null || items.Count == 0)
                return false;

            bool result = true;

            WorkQueueTypeEnum StudyProcess = WorkQueueTypeEnum.GetEnum("StudyProcess");
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext uctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker workQueueUidBroker = uctx.GetBroker<IWorkQueueUidEntityBroker>();
                IWorkQueueEntityBroker workQueueBroker = uctx.GetBroker<IWorkQueueEntityBroker>();
                    
                foreach (WorkQueue item in items)
                {
                    WorkQueueUidSelectCriteria criteria = new WorkQueueUidSelectCriteria();
                    criteria.WorkQueueKey.EqualTo(item.GetKey());

                    IList<WorkQueueUid> unprocessedUids = workQueueUidBroker.Find(criteria);

                    if (item.WorkQueueTypeEnum == StudyProcess)
                    {
                        result = DeleteWorkQueueUids(uctx, item, unprocessedUids, true);
                    }
                    else
                    {
                        result = DeleteWorkQueueUids(uctx, item, unprocessedUids, false);
                    }

                    if (result)
                    {
                        // if the entry is deleted while images are coming in, we may not be able to delete 
                        // the work queue entry. Check to see if any new uids have been added before attempting to delete the entry.
                        // Note: there's still small chance the problem still occurs
                        unprocessedUids = workQueueUidBroker.Find(criteria);
                        if (unprocessedUids==null || unprocessedUids.Count==0)
                        {
                            if (false == workQueueBroker.Delete(item.GetKey()))
                            {
                                result = false;
                                break;
                            }
                        }
                        
                    }
                }

                if (result)
                    uctx.Commit();
            }

            return result;
        }

        /// <summary>
        /// Reschedule a list of <see cref="WorkQueue"/> items
        /// </summary>
        /// <param name="items">List of <see cref="WorkQueue"/> items to be rescheduled</param>
        /// <param name="newScheduledTime">New schedule start date/time</param>
        /// <param name="expirationTime">New expiration date/time</param>
        /// <param name="priority">New priority</param>
        /// <returns>A value indicating whether all <see cref="WorkQueue"/> items in <paramref name="items"/> are updated successfully.</returns>
        /// <remarks>
        /// If one or more <see cref="WorkQueue"/> in <paramref name="items"/> cannot be rescheduled, all changes will be 
        /// reverted and <b>false</b> will be returned.
        /// </remarks>
        public bool RescheduleWorkQueueItems(IList<WorkQueue> items, DateTime newScheduledTime, DateTime expirationTime, WorkQueuePriorityEnum priority)
        {
            if (items == null || items.Count == 0)
                return false;

            WorkQueueUpdateColumns updatedColumns = new WorkQueueUpdateColumns();
            updatedColumns.WorkQueuePriorityEnum = priority;
            updatedColumns.ScheduledTime = newScheduledTime;
            updatedColumns.ExpirationTime = expirationTime;

            bool result = false;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                foreach (WorkQueue item in items)
                {
                    result = workQueueBroker.Update(item.GetKey(), updatedColumns);
                    if (!result)
                    {
                        break;
                    }
                }
                if (result)
                    ctx.Commit();
            }

            return result;
        }


        /// <summary>
        /// Resets a list of <see cref="WorkQueue"/> items.
        /// </summary>
        /// <param name="items">List of <see cref="WorkQueue"/>  to be reset</param>
        /// <param name="newScheduledTime">The new scheduled start date/time for the entries</param>
        /// <param name="expirationTime">The new expiration start date/time for the entries</param>
        /// <returns>
        /// A value indicating whether all <see cref="WorkQueue"/> in <paramref name="items"/> have been reset successfully.
        /// If one or more item cannot be reset, <b>false</b> will be returned and all changes are not committed.
        /// </returns>
        public bool ResetWorkQueueItems(IList<WorkQueue> items, DateTime newScheduledTime, DateTime expirationTime)
        {
            if (items == null || items.Count==0)
                return false;

            
            WorkQueueUpdateColumns columns = new WorkQueueUpdateColumns();
            columns.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Pending");
            columns.ProcessorID = "";
            columns.FailureCount = 0;
            columns.ScheduledTime = newScheduledTime;
            columns.ExpirationTime = expirationTime;

            bool result = true;
            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();
            using (IUpdateContext ctx = store.OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueEntityBroker workQueueBroker = ctx.GetBroker<IWorkQueueEntityBroker>();
                
                foreach(WorkQueue item in items)
                {
                    if (!workQueueBroker.Update(item.GetKey(), columns))
                    {
                        result = false;
                        break;
                        
                    }
                }

                if (result)
                    ctx.Commit();
            }

            return result;
        }

        #endregion
    }
}
