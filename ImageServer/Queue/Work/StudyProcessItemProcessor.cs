using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Dicom;
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Queue;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.Brokers;

namespace ClearCanvas.ImageServer.Queue.Work
{
    /// <summary>
    /// Processor for 'StudyProcess' WorkQueue entries.
    /// </summary>
    public class StudyProcessItemProcessor : IWorkQueueItemProcessor
    {
        private IReadContext _readContext;
        private StudyStorageLocation _storageLocation;
        private IList<WorkQueueUid> _uidList;

        public StudyProcessItemProcessor()
        {
            _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
        }

        /// <summary>
        /// Load the storage location for the WorkQueue item.
        /// </summary>
        /// <param name="item">The item to load the location for.</param>
        private void LoadStorageLocation(WorkQueue item)
        {
            ISelectStudyStorageLocation select = _readContext.GetBroker<ISelectStudyStorageLocation>();

            StudyStorageLocationSelectParameters parms = new StudyStorageLocationSelectParameters();
            parms.StudyStorageKey = item.StudyStorageKey;

            IList<StudyStorageLocation> list = select.Execute(parms);

            if (list.Count == 0)
            {
                Platform.Log(LogLevel.Error, "Unable to find storage location for WorkQueue item: {0}", item.GetKey().ToString());
                throw new ApplicationException("Unable to find storage location for WorkQueue item.");
            }

            _storageLocation = list[0];
        }

        /// <summary>
        /// Load the specific SOP Instance Uids in the database for the WorkQueue item.
        /// </summary>
        /// <param name="item">The WorkQueue item.</param>
        private void LoadUids(WorkQueue item)
        {
            ISelectWorkQueueUids select = _readContext.GetBroker<ISelectWorkQueueUids>();

            WorkQueueUidSelectParameters parms = new WorkQueueUidSelectParameters();

            parms.WorkQueueKey = item.GetKey();

            _uidList = select.Execute(parms);

        }

        /// <summary>
        /// Delete an entry in the WorkQueueUid table.
        /// </summary>
        /// <param name="sop">The WorkQueueUid entry to delete.</param>
        private void DeleteWorkQueueUid(WorkQueueUid sop)
        {
            IDeleteWorkQueueUid delete = _readContext.GetBroker<IDeleteWorkQueueUid>();

            WorkQueueUidDeleteParameters parms = new WorkQueueUidDeleteParameters();
            parms.WorkQueueUidKey = sop.GetKey();

            delete.Execute(parms);
        }

        private void ProcessFile(WorkQueue item, string path)
        {
            DicomFile file = new DicomFile(path);

            file.Load();

            // Get the Patients Name for processing purposes.
            String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

            // Setup the insert parameters
            InstanceInsertParameters parms = new InstanceInsertParameters();
            file.DataSet.LoadDicomFields(parms);
            parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
            parms.StatusEnum = StatusEnum.GetEnum("Online");

            // Get the Insert Instance broker and do the insert
            IInsertInstance insert = _readContext.GetBroker<IInsertInstance>();
            IList<InstanceKeys> keys = insert.Execute(parms);

            // If the Request Attributes Sequence is in the dataset, do an insert.
            if (file.DataSet.Contains(DicomTags.RequestAttributesSequence))
            {
                DicomAttributeSQ attribute = file.DataSet[DicomTags.RequestAttributesSequence] as DicomAttributeSQ;
                if (!attribute.IsEmpty)
                {
                    foreach (DicomSequenceItem sequenceItem in (DicomSequenceItem[])attribute.Values)
                    {
                        RequestAttributesInsertParameters requestParms = new RequestAttributesInsertParameters();
                        sequenceItem.LoadDicomFields(requestParms);
                        requestParms.SeriesKey = keys[0].SeriesKey;

                        IInsertRequestAttributes insertRequest = _readContext.GetBroker<IInsertRequestAttributes>();
                        insertRequest.Execute(requestParms);
                    }
                }
            }

            Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);
        }

        private void ProcessUidList(WorkQueue item)
        {
            string path = "";
            foreach (WorkQueueUid sop in _uidList)
            {
                path = Path.Combine(_storageLocation.GetStudyPath(), sop.SeriesInstanceUid);
                path = Path.Combine(path, sop.SopInstanceUid + ".dcm");
                try
                {
                    ProcessFile(item, path);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                    continue;
                }

                // Delete it out of the queue
                DeleteWorkQueueUid(sop);                
            }

            // Update the WorkQueue item status and times.
            IUpdateWorkQueue update = _readContext.GetBroker<IUpdateWorkQueue>();
            
            WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
            parms.StatusEnum = StatusEnum.GetEnum("Pending");
            parms.WorkQueueKey = item.GetKey();
            parms.StudyStorageKey = item.StudyStorageKey;
            parms.ScheduledTime = DateTime.Now.AddSeconds(15.0);
            parms.ExpirationTime = DateTime.Now.AddMinutes(5.0);

            if (false == update.Execute(parms))
            {
                Platform.Log(LogLevel.Error, "Unable to update StudyProcess WorkQueue GUID to Pending: {0}", item.GetKey().ToString()); 
            }
        }

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Process a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The item to process.</param>
        public void Process(WorkQueue item)
        {
            //Load the storage location.
            LoadStorageLocation(item);

            //Load the specific UIDs that need to be processed.
            LoadUids(item);

            if (_uidList.Count == 0)
            {
                IUpdateWorkQueue update = _readContext.GetBroker<IUpdateWorkQueue>();
                WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();

                if (item.ExpirationTime < DateTime.Now)
                {
                    parms.StatusEnum = StatusEnum.GetEnum("Completed");
                    parms.WorkQueueKey = item.GetKey();
                    parms.StudyStorageKey = item.StudyStorageKey;
                }
                else
                {
                    parms.StatusEnum = StatusEnum.GetEnum("Pending");
                    parms.WorkQueueKey = item.GetKey();
                    parms.StudyStorageKey = item.StudyStorageKey;
                    parms.ScheduledTime = DateTime.Now.AddSeconds(60.0); // 30 second delay to recheck
                    parms.ExpirationTime = item.ExpirationTime; // Keep the same
                }

                if (false == update.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unable to update StudyProcess WorkQueue GUID: {0}", item.GetKey().ToString());
                }
            }
            else
                ProcessUidList(item);
        }

        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (_readContext != null)
            {
                _readContext.Dispose();
                _readContext = null;
            }
        }
        #endregion
    }
}
