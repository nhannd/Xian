using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using ClearCanvas.Dicom;
using ClearCanvas.Common;
using ClearCanvas.Dicom.Network;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Queue;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Streaming;

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
            IQueryStudyStorageLocation select = _readContext.GetBroker<IQueryStudyStorageLocation>();

            StudyStorageLocationQueryParameters parms = new StudyStorageLocationQueryParameters();
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
            IQueryWorkQueueUids select = _readContext.GetBroker<IQueryWorkQueueUids>();

            WorkQueueUidQueryParameters parms = new WorkQueueUidQueryParameters();

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

        private StudyStream LoadStudyStream(string streamFile)
        {
            StudyStream theStream = new StudyStream();

            if (File.Exists(streamFile))
            {
                Stream fileStream = new FileStream(streamFile, FileMode.Open);

                XmlDocument theDoc = new XmlDocument();

                StreamingIo.Read(theDoc, fileStream);

                fileStream.Close();

                theStream.SetMemento(theDoc);
            }

            return theStream;
        }

        private void WriteStudyStream(string streamFile, StudyStream theStream)
        {

            XmlDocument doc = theStream.GetMomento();

            if (File.Exists(streamFile))
                File.Delete(streamFile);

            Stream fileStream = new FileStream(streamFile, FileMode.CreateNew);

            StreamingIo.Write(doc, fileStream);

            fileStream.Close();
        }

        private void ProcessFile(WorkQueue item, string path, StudyStream stream)
        {
            // Use the command processor for rollback capabilities.
            ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM File");
            try
            {
                DicomFile file = new DicomFile(path);

                file.Load();

                // Get the Patients Name for processing purposes.
                String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

                // Insert into the database
                processor.ExecuteCommand(new InsertInstanceCommand(_readContext,file,_storageLocation));

                // Update the StudyStream object
                processor.ExecuteCommand(new InsertStreamCommand(file,stream));

                Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", processor.Description);
                processor.Rollback();
            }
        }

        private void ProcessUidList(WorkQueue item)
        {
            

            string studyStreamPath =
                Path.Combine(_storageLocation.GetStudyPath(), _storageLocation.StudyInstanceUid + ".xml");

            StudyStream stream = LoadStudyStream(studyStreamPath);

            foreach (WorkQueueUid sop in _uidList)
            {
                string path = Path.Combine(_storageLocation.GetStudyPath(), sop.SeriesInstanceUid);
                path = Path.Combine(path, sop.SopInstanceUid + ".dcm");
                try
                {
                    ProcessFile(item, path,stream);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                    continue;
                }

                // Delete it out of the queue
                DeleteWorkQueueUid(sop);                
            }

            // Write it back out.
            WriteStudyStream(studyStreamPath, stream);

            // Update the WorkQueue item status and times.
            IUpdateWorkQueue update = _readContext.GetBroker<IUpdateWorkQueue>();
            
            WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
            parms.StatusEnum = StatusEnum.GetEnum("Pending");
            parms.WorkQueueKey = item.GetKey();
            parms.StudyStorageKey = item.StudyStorageKey;
            parms.ScheduledTime = Platform.Time.AddSeconds(15.0);
            parms.ExpirationTime = Platform.Time.AddMinutes(5.0);

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

                if (item.ExpirationTime < Platform.Time)
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
                    parms.ScheduledTime = Platform.Time.AddSeconds(60.0); // 30 second delay to recheck
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
