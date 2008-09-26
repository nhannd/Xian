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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Enterprise;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.EntityBrokers;

namespace ClearCanvas.ImageServer.Services.WorkQueue.WebEditStudy
{
    /// <summary>
    /// 'WebEditStudy' processor.
    /// </summary>
    public class WebEditStudyItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ServerPartition _partition;
        private Study _study;
        private Patient _patient;
        private string _tempPath;
        #endregion Private Members

        #region Public Properties

        /// <summary>
        ///  The <see cref="Study"/> associated  with the 'WebEditStudy' work queue item 
        /// </summary>
        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        /// <summary>
        ///  The <see cref="Patient"/> associated  with the 'WebEditStudy' work queue item 
        /// </summary>
        public Patient Patient
        {
            get { return _patient; }
            set { _patient = value; }
        }

        /// <summary>
        ///  The <see cref="ServerPartition"/> associated  with the 'WebEditStudy' work queue item 
        /// </summary>
        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        #endregion Public Properties

        #region Overridden Protected Method

        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(item.Data, "item.Data");

			if (!LoadStorageLocation(item))
			{
				Platform.Log(LogLevel.Warn, "Unable to find readable location when processing WebEditStudy WorkQueue item, rescheduling");
				PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
				return;
			}

            WorkQueueSelectCriteria workQueueCriteria = new WorkQueueSelectCriteria();
            workQueueCriteria.StudyStorageKey.EqualTo(item.StudyStorageKey);
            workQueueCriteria.WorkQueueTypeEnum.In(new WorkQueueTypeEnum[] {WorkQueueTypeEnum.StudyProcess});
            workQueueCriteria.WorkQueueStatusEnum.In(new WorkQueueStatusEnum[] { WorkQueueStatusEnum.Idle, WorkQueueStatusEnum.InProgress, WorkQueueStatusEnum.Pending});

            List<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(item, workQueueCriteria);
            if (relatedItems != null && relatedItems.Count > 0)
            {
                // can't do it now. Reschedule it for future
                relatedItems.Sort(delegate(Model.WorkQueue item1, Model.WorkQueue item2)
                                      {
                                          return item1.ScheduledTime.CompareTo(item2.ScheduledTime);
                                      });

                DateTime newScheduledTime = relatedItems[0].ScheduledTime.AddMinutes(1);
                if (newScheduledTime < Platform.Time.AddMinutes(1))
                    newScheduledTime = Platform.Time.AddMinutes(1);

                PostponeItem(item, newScheduledTime, newScheduledTime.AddDays(1));
                Platform.Log(LogLevel.Info, "{0} postponed to {1}. Study UID={2}", item.WorkQueueTypeEnum, newScheduledTime, StorageLocation.StudyInstanceUid);
            }
            else
            {
                EditStudyContext ctx = new EditStudyContext();
                using (ServerCommandProcessor processor = new ServerCommandProcessor("Process EditStudy"))
                {
                    bool successful = false;
                    string failure = null;
                    try
                    {
                        
                        LoadEntities(item);

                        if (Study != null && Patient != null)
                        {
                            ctx.UpdateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);

                            ctx.WorkQueueItem = item;
                            ctx.Study = Study;
                            ctx.Patient = Patient;
                            ctx.Partition = Partition;
                            ctx.StorageLocation = StorageLocation;

                        	StudyStorageLocation originalLocation;
                        	FilesystemMonitor.Instance.GetStudyStorageLocation(item.StudyStorageKey,
																			   out originalLocation);
							ctx.OriginalStorageLocation = originalLocation; // copy
							ctx.NewStudyXml = new ClearCanvas.Dicom.Utilities.Xml.StudyXml();

                            // output folder = temp\ImageServer\EditStudy\.....
                            string outFolder = GetTempPath();
                            outFolder = Path.Combine(outFolder, "ImageServer");
                            outFolder = Path.Combine(outFolder, "EditStudy");
                            outFolder = Path.Combine(outFolder, Path.GetRandomFileName());
                            ctx.TempOutRootFolder = outFolder;


                            string itemDesc = String.Format("WebEditStudy for {0} study={1} UID={2}", Patient.Name, Study.StudyDescription, Study.StudyInstanceUid);
                            EditStudyCommand editCommand = new EditStudyCommand(itemDesc, ctx, item.Data.DocumentElement);
                            processor.AddCommand(editCommand);

                            successful = processor.Execute();

                            failure = processor.FailureReason;
                        }

                    }
                    catch (Exception e)
                    {
                        Platform.Log(LogLevel.Error, e, "Unexpected exception occured while processing the WebEditStudy work queue item");
                        failure = e.Message;
                        processor.Rollback();
                        successful = false;

                    }
                    finally
                    {
                        if (successful)
                        {
                            ctx.UpdateContext.Commit();
                            PostProcessing(item, false, true);
                        }
                        else
                        {
                            ctx.UpdateContext.Dispose();
                            FailQueueItem(item, failure);
                        }

                        // delete the temp folder
                        DirectoryUtility.DeleteIfExists(ctx.TempOutRootFolder);
                    }
                }
            }
            
            
        }
        
        #endregion Public Methods

        #region Private Methods

        private string GetTempPath()
        {
            if (String.IsNullOrEmpty(_tempPath))
            {
                string root = Path.GetPathRoot(Path.GetTempPath());
                _tempPath = Path.Combine(root, "temp");    
            }
            if (!Directory.Exists(_tempPath))
                Directory.CreateDirectory(_tempPath);
            
            return _tempPath;
        }

        private void LoadEntities(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            string studyInstanceUid = StorageLocation.StudyInstanceUid;
            ServerEntityKey partitionKey = item.ServerPartitionKey;

            IServerPartitionEntityBroker serverPartitionBroker = ReadContext.GetBroker<IServerPartitionEntityBroker>();
            _partition = serverPartitionBroker.Load(partitionKey);

            IStudyEntityBroker studyBroker = ReadContext.GetBroker<IStudyEntityBroker>();
            StudySelectCriteria criteria = new StudySelectCriteria();
            criteria.ServerPartitionKey.EqualTo(partitionKey);
            criteria.StudyInstanceUid.EqualTo(studyInstanceUid);
            Study study = studyBroker.FindOne(criteria);

            if (study == null )
            {
                // no study found. One possiblity this could happen is EditStudy entry was scheduled to happend after another StudyDelete.
                Platform.Log(LogLevel.Error, "No study entity found for work queue item {0}", item.GetKey().Key);
                return;
            }

            _study = study;
            IPatientEntityBroker patientBroker = ReadContext.GetBroker<IPatientEntityBroker>();
            _patient = patientBroker.Load(_study.PatientKey);

            Debug.Assert(Partition != null);
            Debug.Assert(Patient != null);
            Debug.Assert(Study != null);
        }

        #endregion
    }
}
