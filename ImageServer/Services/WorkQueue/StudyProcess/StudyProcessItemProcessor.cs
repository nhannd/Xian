#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Rules;
using ClearCanvas.ImageServer.Services.WorkQueue;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Processor for 'StudyProcess' WorkQueue entries.
    /// </summary>
    public class StudyProcessItemProcessor : IWorkQueueItemProcessor
    {
        private IReadContext _readContext;
        private StudyStorageLocation _storageLocation;
        private IList<WorkQueueUid> _uidList;
        private ServerRulesEngine _sopProcessedRulesEngine;

        public StudyProcessItemProcessor()
        {
            _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
        }

        ~StudyProcessItemProcessor()
        {
            if (_readContext != null)
                _readContext.Dispose();
        }

        /// <summary>
        /// Load the storage location for the WorkQueue item.
        /// </summary>
        /// <param name="item">The item to load the location for.</param>
        private void LoadStorageLocation(Model.WorkQueue item)
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
        private void LoadUids(Model.WorkQueue item)
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

        private StudyXml LoadStudyStream(string streamFile)
        {
            StudyXml theStream = new StudyXml();

            if (File.Exists(streamFile))
            {
                Stream fileStream = new FileStream(streamFile, FileMode.Open);

                XmlDocument theDoc = new XmlDocument();

                StudyXmlIo.Read(theDoc, fileStream);

                fileStream.Close();

                theStream.SetMemento(theDoc);
            }

            return theStream;
        }



        private void ProcessFile(Model.WorkQueue item, string path, StudyXml stream, string studyStreamFile)
        {
            // Use the command processor for rollback capabilities.
            ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM File");
            try
            {
                DicomFile file = new DicomFile(path);

                file.Load();

                // Get the Patients Name for processing purposes.
                String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

                // Update the StudyStream object
                processor.ExecuteCommand(new InsertStreamCommand(file, stream, studyStreamFile));

                // Insert into the database
                processor.ExecuteCommand(new InsertInstanceCommand(_readContext,file,_storageLocation));

                ServerActionContext context = new ServerActionContext(file);
                _sopProcessedRulesEngine.Execute(context);

                Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.", processor.Description);
                processor.Rollback();
                throw new ApplicationException("Unexpected exception when processing file.",e);
            }

            
        }

        private void ProcessUidList(Model.WorkQueue item)
        {
            string studyStreamPath =
                Path.Combine(_storageLocation.GetStudyPath(), _storageLocation.StudyInstanceUid + ".xml");
            
            StudyXml stream;

            try
            {
                stream = LoadStudyStream(studyStreamPath);
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e,
                             "Unexpected exception when loading study stream to process StudyProcess WorkQueue item for Study Instance UID: {0} File: {1}", _storageLocation.StudyInstanceUid, studyStreamPath);
                throw new ApplicationException("Unexpected exception when loading study stream", e);
            }

            int successfulProcessCount = 0;

            foreach (WorkQueueUid sop in _uidList)
            {
                string path = Path.Combine(_storageLocation.GetStudyPath(), sop.SeriesInstanceUid);
                path = Path.Combine(path, sop.SopInstanceUid + ".dcm");
                try
                {
                    ProcessFile(item, path,stream, studyStreamPath);
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                    continue;
                }

                successfulProcessCount++;

                // Delete it out of the queue
                DeleteWorkQueueUid(sop);                
            }

            // Update the WorkQueue item status and times.
            IUpdateWorkQueue update = _readContext.GetBroker<IUpdateWorkQueue>();
            
            WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
            parms.WorkQueueKey = item.GetKey();
            parms.StudyStorageKey = item.StudyStorageKey;

            if (successfulProcessCount == 0)
            {
                parms.FailureCount = item.FailureCount + 1;
                ImageServerServicesWorkQueueSettings settings = ImageServerServicesWorkQueueSettings.Default;
                if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                {
                    Platform.Log(LogLevel.Error,"Failing StudyProcess WorkQueue entry ({0}), reached max retry count of {1}",item.GetKey(),item.FailureCount + 1);
                    parms.StatusEnum = StatusEnum.GetEnum("Failed");
                    parms.ScheduledTime = Platform.Time;
                    parms.ExpirationTime = Platform.Time;
                }
                else
                {
                    Platform.Log(LogLevel.Error, "Resetting StudyProcess WorkQueue entry ({0}) to Pending, current retry count {1}", item.GetKey(), item.FailureCount + 1);
                    parms.StatusEnum = StatusEnum.GetEnum("Pending");
                    parms.ScheduledTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);
                    parms.ExpirationTime = Platform.Time.AddMinutes(settings.WorkQueueMaxFailureCount * settings.WorkQueueFailureDelayMinutes);
                }
            }
            else
            {
                parms.StatusEnum = StatusEnum.GetEnum("Pending");
                parms.FailureCount = item.FailureCount;
                parms.ScheduledTime = Platform.Time.AddSeconds(15.0);
                parms.ExpirationTime = Platform.Time.AddMinutes(5.0);
            }

            if (false == update.Execute(parms))
            {
                Platform.Log(LogLevel.Error, "Unable to update StudyProcess WorkQueue GUID Status: {0}", item.GetKey().ToString()); 
            }
        }

        #region IWorkQueueItemProcessor Members

        /// <summary>
        /// Process a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The item to process.</param>
        public void Process(Model.WorkQueue item)
        {
            // Load the rules engine
            _sopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.GetEnum("SopProcessed"));
            _sopProcessedRulesEngine.Load();

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
                    parms.FailureCount = item.FailureCount;
                }
                else
                {
                    parms.StatusEnum = StatusEnum.GetEnum("Pending");
                    parms.WorkQueueKey = item.GetKey();
                    parms.StudyStorageKey = item.StudyStorageKey;
                    parms.ScheduledTime = Platform.Time.AddSeconds(90.0); // 60 second delay to recheck
                    parms.ExpirationTime = item.ExpirationTime; // Keep the same
                    parms.FailureCount = item.FailureCount;
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