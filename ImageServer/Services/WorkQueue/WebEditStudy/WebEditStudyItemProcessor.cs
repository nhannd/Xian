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
using ClearCanvas.Common.Utilities;
using ClearCanvas.DicomServices.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Services.WorkQueue.EditStudy;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    public class WebEditStudyItemProcessor : BaseItemProcessor
    {
        private ServerPartition _partition;
        private Study _study;
        private Patient _patient;

        public Study Study
        {
            get {  
                return _study;

            }
            set { _study = value; }
        }

        public Patient Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        private List<FileInfo> GetDicomFileList(string studyPath)
        {
            List<FileInfo> fileList = new List<FileInfo>();
            
            FileProcessor.Process(studyPath, "*.dcm", delegate(string filePath)
                                                          {
                                                              fileList.Add(new FileInfo(filePath));
                                                          }, true);
            return fileList;
        }

        
        public override void Process(ClearCanvas.ImageServer.Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item.Data, "item.Data");

            LoadStorageLocation(item);

            IPersistentStore store = PersistentStoreRegistry.GetDefaultStore();

            string studyInstanceUid = StorageLocation.StudyInstanceUid;
            ServerEntityKey partitionKey = item.ServerPartitionKey;

            IServerPartitionEntityBroker serverPartitionBroker = ReadContext.GetBroker<IServerPartitionEntityBroker>();
            _partition = serverPartitionBroker.Load(partitionKey);

            IStudyEntityBroker studyBroker = ReadContext.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partitionKey);
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            IList<Study> studies = studyBroker.Find(criteria);

            if (studies == null || studies.Count==0)
            {
                // no stuyy found
                Platform.Log(LogLevel.Debug, "No study entity found for work queue item {0}", item.GetKey().Key);
                return;
            }

            _study = studies[0];
            IPatientEntityBroker patientBroker = ReadContext.GetBroker<IPatientEntityBroker>();
            _patient = patientBroker.Load(_study.PatientKey);

            Platform.CheckForNullReference(Partition, "Partition");
            Platform.CheckForNullReference(Patient, "Patient");
            Platform.CheckForNullReference(Study, "Study");


            ServerCommandProcessor processor = new ServerCommandProcessor("Process EditStudy");
            bool successful = false;
            try
            {
                    
                    EditStudyContext ctx = new EditStudyContext();
                    ctx.WorkQueueItem = item;
                    ctx.StudyKey = Study.GetKey();
                    ctx.PatientKey = Patient.GetKey();
                    ctx.Partition = Partition;
                    ctx.StorageLocation = StorageLocation;
                    ctx.NewStudyXml = new ClearCanvas.DicomServices.Xml.StudyXml();

                    string destFolder = Path.GetTempPath();
                    destFolder = Path.Combine(destFolder, "ImageServer");
                    destFolder = Path.Combine(destFolder, "EditStudy");
                    destFolder = Path.Combine(destFolder, Path.GetRandomFileName());
                    ctx.DestinationFolder = destFolder;
                    
                    EditStudyCommand editCommand = new EditStudyCommand("Edit Study Command");
                    editCommand.Context = ctx;
                    editCommand.Compile(item.Data.DocumentElement);

                    processor.AddCommand(editCommand);

                    successful = processor.Execute();


                
            }
            catch(Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception occured while processing the EditStudy work queue item");
            }
            finally
            {
                PostProcessing(item, 0, !successful);
                if (!successful)
                    processor.Rollback();
                
            }
            
        }

        protected override void PostProcessing(ClearCanvas.ImageServer.Model.WorkQueue item, int batchSize, bool failed)
        {
            using (
                IUpdateContext updateContext =
                    PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IUpdateWorkQueue broker = updateContext.GetBroker<IUpdateWorkQueue>();
                WorkQueueUpdateParameters parms = new WorkQueueUpdateParameters();
                parms.WorkQueueKey = item.GetKey();
                parms.StudyStorageKey = item.StudyStorageKey;
                parms.ProcessorID = item.ProcessorID;

                WorkQueueSettings settings = WorkQueueSettings.Default;
                
                if (!failed)
                {
                    // it is completed
                    parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Completed");
                    parms.FailureCount = item.FailureCount;
                    parms.ScheduledTime = item.ScheduledTime;
                    parms.ExpirationTime = item.ExpirationTime; // Keep the same
                }
                else
                {
                    parms.FailureCount = item.FailureCount + 1;
                    if ((item.FailureCount + 1) > settings.WorkQueueMaxFailureCount)
                    {
                        Platform.Log(LogLevel.Error,
                                     "Failing EditStudy WorkQueue entry ({0}), reached max retry count of {1}",
                                     item.GetKey(), item.FailureCount + 1);
                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Failed");
                        parms.ScheduledTime = Platform.Time;
                        parms.ExpirationTime = Platform.Time; // expire now
                    }
                    else
                    {
                        Platform.Log(LogLevel.Error,
                                     "Resetting EditStudy WorkQueue entry ({0}) to Pending, current retry count {1}",
                                     item.GetKey(), item.FailureCount + 1);
                        parms.WorkQueueStatusEnum = WorkQueueStatusEnum.GetEnum("Pending");
                        parms.ScheduledTime = Platform.Time.AddMinutes(settings.WorkQueueFailureDelayMinutes);
                        parms.ExpirationTime = Platform.Time.AddMinutes((settings.WorkQueueMaxFailureCount - item.FailureCount) *
                                                     settings.WorkQueueFailureDelayMinutes);
                    }
                }

                if (false == broker.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unable to update EditStudy WorkQueue GUID: {0}", item.GetKey().ToString());
                }
                else
                    updateContext.Commit();
            }
        }
    }
}
