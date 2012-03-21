using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Threading;
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageViewer.Common.WorkItem;
using ClearCanvas.ImageViewer.Dicom.Core;
using ClearCanvas.ImageViewer.StudyManagement.Storage;

namespace ClearCanvas.ImageViewer.Shreds.WorkItem
{


   public abstract class BaseItemProcessor: IWorkItemProcessor
    {
        #region Private Fields
        private const int MAX_DB_RETRY = 5;
        private string _name = "Work Item";
        private IList<WorkItemUid> _uidList;
     	private bool _cancelPending;
    	private readonly object _syncRoot = new object();

        #endregion

        #region Constructors

        #endregion

        #region Properties

        protected IList<WorkItemUid> WorkQueueUidList
        {
            get
            {
                if (_uidList==null)
                {
                    LoadUids(Proxy.Item);
                }
                return _uidList;
            }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public WorkItemStatusProxy Proxy { get; set; }

        protected bool CancelPending
        {
            get { lock (_syncRoot) return _cancelPending; }
        }

        /// <summary>
        /// Gets or sets a boolean value indicating whether the Work Queue Item is completed.
        /// </summary>
        protected bool Completed
        {
            get;
            set;
        }
 
        #endregion

        #region Protected Methods

        public virtual bool Intialize(WorkItemStatusProxy proxy)
        {
            Proxy = proxy;
            return true;
        }

        public virtual bool CanStart(WorkItemStatusProxy proxy, out string reason)
        {
            reason = string.Empty;
            return true;
        }

        public abstract void Process(WorkItemStatusProxy proxy);


        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        /// <param name="item">The WorkQueue item.</param>
        protected void LoadUids(StudyManagement.Storage.WorkItem item)
        {
            if (_uidList == null)
            {
                using (var context = new DataAccessContext())
                {
                    var broker = context.GetWorkItemUidBroker();

                    _uidList = broker.GetWorkItemUidsForWorkItem(item.Oid);
                }
            }
        }

        /// <summary>
        /// Routine for failing a work queue uid record.
        /// </summary>
        /// <param name="sop">The WorkQueueUid record to fail.</param>
        /// <param name="retry">A boolean value indicating whether a retry will be attempted later.</param>
        protected void FailWorkItemUid(WorkItemUid sop, bool retry)
        {
            using (var context = new DataAccessContext())
            {
                sop.Complete = true;
                if (!sop.FailureCount.HasValue)
                    sop.FailureCount = 1;
                else
                    sop.FailureCount = (byte) (sop.FailureCount.Value + 1);

                if (sop.FailureCount > WorkItemServiceSettings.Instance.RetryCount)
                    sop.Complete = true; // TODO

                var broker = context.GetWorkItemUidBroker();
                broker.Update(sop);
                context.Commit();
            }
        }

        /// <summary>
        /// Delete an entry in the <see cref="WorkItemUid"/> table.
        /// </summary>
        /// <param name="sop">The <see cref="WorkItemUid"/> entry to delete.</param>
        protected virtual void CompleteWorkItemUid(WorkItemUid sop)
        {
            // Must retry in case of db error.
            // Failure to do so may lead to orphaned WorkQueueUid and FileNotFoundException 
            // when the work queue is reset.
            int retryCount = 0;
            while (true)
            {
                try
                {
                    using (var context = new DataAccessContext())
                    {
                        sop.Complete = true;
                        var broker = context.GetWorkItemUidBroker();
                        broker.Update(sop);
                        context.Commit();
                    }
                    break; // done
                }
                catch (Exception ex)
                {
                    if (ex is SqlException)
                    {
                        if (retryCount > MAX_DB_RETRY)
                        {
                            Platform.Log(LogLevel.Error, ex,
                                         "Error occurred when calling DeleteWorkQueueUid. Max db retry count has been reached.");
                            Proxy.Fail(
                                String.Format(
                                    "Error occurred when deleting WorkItemUid. Max db retry count has been reached."),
                                WorkItemFailureType.Fatal);
                            return;
                        }

                        Platform.Log(LogLevel.Error, ex,
                                     "Error occurred when calling DeleteWorkQueueUid(). Retry later. OID={0}", sop.Oid);
                        SleepForRetry();


                        // Service is stoping
                        if (CancelPending)
                        {
                            Platform.Log(LogLevel.Warn, "Termination Requested. DeleteWorkQueueUid() is now terminated.");
                            break;
                        }
                        retryCount++;
                    }
                    else
                        throw;
                }
            }

        }


        /// <summary>
        /// Put the workqueue thread to sleep for 2-3 minutes.
        /// </summary>
        /// <remarks>
        /// This method does not return until 2-3 minutes later or if the service is stoppping.
        /// </remarks>
        private void SleepForRetry()
        {
            var start = Platform.Time;
            var rand = new Random();
            while (!CancelPending)
            {
                // sleep, wake up every 1-3 sec and check if the service is stopping
                Thread.Sleep(rand.Next(1000, 3000));
                if (CancelPending)
                {
                    break;
                }

                // Sleep for 2-3 minutes
                DateTime now = Platform.Time;
                if (now - start > TimeSpan.FromMinutes(rand.Next(2, 3)))
                    break;
            }
        }

       /// <summary>
        /// Load a <see cref="StudyXml"/> file for a given <see cref="StudyLocation"/>
        /// </summary>
        /// <param name="location">The location a study is stored.</param>
        /// <returns>The <see cref="StudyXml"/> instance for <paramref name="location"/></returns>
        protected virtual StudyXml LoadStudyXml(StudyLocation location)
        {
            var theXml = new StudyXml();


            String streamFile = Path.Combine(location.StudyFolder, location.Study.StudyInstanceUid + ".xml");
            if (File.Exists(streamFile))
            {
                using (Stream fileStream = FileStreamOpener.OpenForRead(streamFile, FileMode.Open))
                {
                    var theDoc = new XmlDocument();

                    StudyXmlIo.Read(theDoc, fileStream);

                    theXml.SetMemento(theDoc);

                    fileStream.Close();
                }
            }
            return theXml;
        }
   

        /// <summary>
        /// Returns a list of related <see cref="StudyManagement.Storage.WorkItem"/> with specified types and status (both are optional).
        /// and related to the given <see cref="StudyManagement.Storage.WorkItem"/> 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        protected IList<StudyManagement.Storage.WorkItem> FindRelatedWorkItems(IEnumerable<WorkItemTypeEnum> types, IEnumerable<WorkItemStatusEnum> status)
        {
            IList<StudyManagement.Storage.WorkItem> list;

            using (var context = new DataAccessContext())
            {
                var broker = context.GetWorkItemBroker();

                list = broker.GetWorkItems(null, null, Proxy.Item.StudyInstanceUid);


            }

            if (list==null)
                return null;

            // remove the current item 
            CollectionUtils.Remove(list, item => item.Oid.Equals(Proxy.Item.Oid));

            // Remove items if the type is in not the list
            if (types != null)
            {
                list = CollectionUtils.Select(list,
                                              item =>
                                              CollectionUtils.Contains(types, t => t.Equals(item.Type)));
            }

            // Remove items if the type is in the list
            if (status != null)
            {
                list = CollectionUtils.Select(list,
                                              item =>
                                              CollectionUtils.Contains(status,
                                                                       s => s.Equals(item.Status)));
            }

            return list;
        }

        
        /// <summary>
        /// Called by the base before <see cref="Process"/> is invoked to determine 
        /// if the process can begin.
        /// </summary>
        /// <returns>True if the processing can begin. False otherwise.</returns>
        /// <remarks>
        /// </remarks>
        protected abstract bool CanStart();

        /// <summary>
        /// Called by the base to initialize the processor.
        /// </summary>
        protected virtual bool Initialize(StudyManagement.Storage.WorkItem item, out string failureDescription)
        {
        	failureDescription = string.Empty;
        	return true;
        }

        public void Cancel()
        {
            lock (_syncRoot)
                _cancelPending = true;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Dispose of any native resources.
        /// </summary>
        public virtual void Dispose()
        {

        }

        #endregion

    }
}
