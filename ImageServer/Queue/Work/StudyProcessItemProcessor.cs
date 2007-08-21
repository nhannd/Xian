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
    public class StudyProcessItemProcessor : IWorkQueueItemProcessor
    {
        private IReadContext _readContext;
        private StudyStorageLocation _storageLocation;
        private IList<WorkQueueUid> _uidList;

        public StudyProcessItemProcessor()
        {
            _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
        }

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

        private void LoadUids(WorkQueue item)
        {
            ISelectWorkQueueUids select = _readContext.GetBroker<ISelectWorkQueueUids>();

            WorkQueueUidSelectParameters parms = new WorkQueueUidSelectParameters();

            parms.WorkQueueKey = item.GetKey();

            _uidList = select.Execute(parms);

        }

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

            String patientName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

            InstanceInsertParameters parms = new InstanceInsertParameters();

            file.DataSet.LoadDicomFields(parms);

            parms.ServerPartitionKey = _storageLocation.ServerPartitionKey;
            parms.StatusEnum = StatusEnum.GetEnum("Online");

            IInsertInstance insert = _readContext.GetBroker<IInsertInstance>();

            IList<InstanceKeys> keys = insert.Execute(parms);

            Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientName);
        }

        private void ProcessList(WorkQueue item)
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

        public void Process(WorkQueue item)
        {
            LoadStorageLocation(item);

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
                ProcessList(item);
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
