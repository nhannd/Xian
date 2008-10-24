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
using ClearCanvas.Common.Statistics;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Encapsulates the context of the 'StudyProcess' operation.
    /// </summary>
    internal class StudyProcessorContext
    {
        #region Private Members
        private Model.WorkQueue _item;
        private IReadContext _readContext;
        private IUpdateContext _updateContext;
        private ServerPartition _partition;
        private StudyStorageLocation _storageLocation;
        private Study _study;
        #endregion

        #region Constructors

        public StudyProcessorContext(Model.WorkQueue item)
        {
            _item = item;
            _partition = ServerPartition.Load(item.ServerPartitionKey);
        }

        #endregion

        #region Public Properties

        public ServerPartition Partition
        {
            get { return _partition; }
            set { _partition = value; }
        }

        public Model.WorkQueue WorkQueueItem
        {
            get { return _item; }
            set { _item = value; }
        }

        public IReadContext ReadContext
        {
            get
            {
                if (_readContext == null)
                    _readContext = PersistentStoreRegistry.GetDefaultStore().OpenReadContext();
                return _readContext;
            }
            set { _readContext = value; }
        }


        public IUpdateContext UpdateContext
        {
            get
            {
                if (_updateContext == null)
                    _updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush);
                return _updateContext;
            }
            set { _updateContext = value; }
        }

        public Study Study
        {
            get { return _study; }
            set { _study = value; }
        }

        public StudyStorageLocation StorageLocation
        {
            get { return _storageLocation; }
            set { _storageLocation = value; }
        }

        #endregion


    }


    /// <summary>
    /// Processor for 'StudyProcess' <see cref="WorkQueue"/> entries.
    /// </summary>
    public class StudyProcessItemProcessor : BaseItemProcessor
    {
        #region Private Members
        private ServerRulesEngine _sopProcessedRulesEngine;
        private ServerRulesEngine _studyProcessedRulesEngine;
        private ServerRulesEngine _seriesProcessedRulesEngine;

        private StudyProcessStatistics _statistics;
        private InstanceStatistics _instanceStats;
        private StudyProcessorContext _context;
        
        #endregion

        #region Public Properties
       
        #endregion

        #region Constructors
        public StudyProcessItemProcessor()
        {
            _statistics = new StudyProcessStatistics();
            
        }

        #endregion Constructors

        #region Private Methods
        /// <summary>
        /// Method for applying rules when a new study has been inserted.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed.</param>
        /// <param name="file">The DICOM file being processed.</param>
        private void ProcessStudyRules(Model.WorkQueue item, DicomFile file)
        {
            if (_studyProcessedRulesEngine == null)
            {
                _studyProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.StudyProcessed, item.ServerPartitionKey);
                _studyProcessedRulesEngine.Load();
            }
            else
            {
                _studyProcessedRulesEngine.Statistics.LoadTime.Reset();
                _studyProcessedRulesEngine.Statistics.ExecutionTime.Reset();
            }
            ServerActionContext context = new ServerActionContext(file,StorageLocation.FilesystemKey, item.ServerPartitionKey,item.StudyStorageKey);

            context.CommandProcessor = new ServerCommandProcessor("Study Rule Processor");

			_studyProcessedRulesEngine.Execute(context);

			// This is a bit kludgy, but we had a problem with studies with only 1 image incorectlly
			// having archive requests inserted when they were scheduled for deletion.  Calling
			// this command here so that if a delete is inserted at the study level, we will remove
			// the previously inserted archive request for the study.  Note also this has to be done
			// after the rules engine is executed.
			context.CommandProcessor.AddCommand(new InsertArchiveQueueCommand(item.ServerPartitionKey, item.StudyStorageKey));

            if (false == context.CommandProcessor.Execute())
            {
                Platform.Log(LogLevel.Error, "Unexpeected failure processing Study level rules");
            }
        }


        /// <summary>
        /// Method for applying rules when a new series has been inserted.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed.</param>
        /// <param name="file">The DICOM file being processed.</param>
        private void ProcessSeriesRules(Model.WorkQueue item, DicomFile file)
        {
            if (_seriesProcessedRulesEngine == null)
            {
                _seriesProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SeriesProcessed, item.ServerPartitionKey);
                _seriesProcessedRulesEngine.Load();
            }
            else
            {
                _seriesProcessedRulesEngine.Statistics.LoadTime.Reset();
                _seriesProcessedRulesEngine.Statistics.ExecutionTime.Reset();
            }
           
            ServerActionContext context = new ServerActionContext(file, StorageLocation.FilesystemKey, item.ServerPartitionKey, item.StudyStorageKey);

            context.CommandProcessor = new ServerCommandProcessor("Series Rule Processor");

            _seriesProcessedRulesEngine.Execute(context);

            if (false == context.CommandProcessor.Execute())
            {
                Platform.Log(LogLevel.Error,"Error processing Series level rules StudyStorage {0}",item.StudyStorageKey);
            }
        }

        private static void ProcessDuplicate(string basePath, string duplicatePath)
        {
            DicomFile dupFile = new DicomFile(duplicatePath);
            DicomFile baseFile = new DicomFile(basePath);

            dupFile.Load(DicomReadOptions.StorePixelDataReferences);
            baseFile.Load(DicomReadOptions.StorePixelDataReferences);

            if (!dupFile.TransferSyntax.Equals(baseFile.TransferSyntax))
            {
                string failure = String.Format("Base file transfer syntax '{0}' not equal to duplicate file '{1}'",
                                               baseFile.TransferSyntax, dupFile.TransferSyntax);
                throw new ApplicationException(failure);
            }
            string failureReason;
            if (baseFile.DataSet.Equals(dupFile.DataSet, out failureReason))
            {
                Platform.Log(LogLevel.Info,
                             "Duplicate SOP being processed is identical.  Removing SOP: {0}",
                             baseFile.MediaStorageSopInstanceUid);

                File.Delete(duplicatePath);
                
                return;
            }
            else throw new ApplicationException(failureReason);
        }

        /// <summary>
        /// Returns a value indicating whether the Dicom image must be reconciled.
        /// </summary>
        /// <param name="message">The Dicom message</param>
        /// <returns></returns>
        private bool ShouldReconcile(DicomMessageBase message)
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(message, "message");

            string studyInstanceUid = message.DataSet[DicomTags.StudyInstanceUid].GetString(0, String.Empty);
            if (_context.Study == null || _context.Study.StudyInstanceUid != studyInstanceUid)
            {
                _context.Study = Study.Find(studyInstanceUid, _context.Partition);
                // Note: if this is the first image in the study, _study will still be null at point
            }

            if (_context.Study == null)
            {
                // the study doesn't exist in the database
                return false;
            }
            else
            {
                StudyComparer comparer = new StudyComparer();
                IList<DicomAttribute> list = comparer.Compare(message, _context.Study, GetComparisonOptions());
                return list != null && list.Count > 0;
            }
        }

        /// <summary>
        /// Gets the <see cref="StudyCompareOptions"/> based on the server partition settings.
        /// </summary>
        /// <returns></returns>
        private StudyCompareOptions GetComparisonOptions()
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.Partition, "_context.Partition");

            StudyCompareOptions options = new StudyCompareOptions();
            options.MatchAccessionNumber = _context.Partition.MatchAccessionNumber;
            options.MatchIssuerOfPatientId = _context.Partition.MatchIssuerOfPatientId;
            options.MatchPatientId = _context.Partition.MatchPatientId;
            options.MatchPatientsBirthDate = _context.Partition.MatchPatientsBirthDate;
            options.MatchPatientsName = _context.Partition.MatchPatientsName;
            options.MatchPatientsSex = _context.Partition.MatchPatientsSex;

            return options;
        }

        /// <summary>
        /// Process a specific DICOM file related to a <see cref="WorkQueue"/> request.
        /// </summary>
        /// <param name="item">The WorkQueue item to process</param>
        /// <param name="queueUid"></param>
        /// <param name="path">The path of the file to process.</param>
        /// <param name="stream">The <see cref="StudyXml"/> file to update with information from the file.</param>
        private void ProcessFile(Model.WorkQueue item, WorkQueueUid queueUid, string path, StudyXml stream)
        {
            DicomFile file;

            long fileSize;
            FileInfo fileInfo = new FileInfo(path);
            fileSize = fileInfo.Length;
            
            _instanceStats.FileLoadTime.Start();
            file = new DicomFile(path);
            file.Load(DicomReadOptions.StorePixelDataReferences);
            _instanceStats.FileLoadTime.End();
            _instanceStats.FileSize = (ulong) fileSize;

            string sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "File:"+fileInfo.Name);
            
            _instanceStats.Description = sopInstanceUid;

            if (ShouldReconcile(file))
            {
                ScheduleReconcile(file);
            }
            else
            {
                InsertInstance(file, stream, queueUid);
            }
        }

        /// <summary>
        /// Inserts an instance into the system
        /// </summary>
        /// <param name="file"></param>
        /// <param name="stream"></param>
        /// <param name="queueUid"></param>
        private void InsertInstance(DicomFile file, StudyXml stream, WorkQueueUid queueUid)
        {
            Platform.CheckForNullReference(_context, "_context");
            Platform.CheckForNullReference(_context.WorkQueueItem, "_context.WorkQueueItem");

            ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue DICOM file");
            InsertInstanceCommand insertInstanceCommand = null;
            InsertStudyXmlCommand insertStudyXmlCommand = null;
            InstanceKeys keys = null;

            String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, String.Empty);
            String modality = file.DataSet[DicomTags.Modality].GetString(0, String.Empty);
            
            try
            {
                
                // Update the StudyStream object
                insertStudyXmlCommand = new InsertStudyXmlCommand(file, stream, StorageLocation);
                processor.AddCommand(insertStudyXmlCommand);

                // Insert into the database, but only if its not a duplicate so the counts don't get off
                if (!queueUid.Duplicate)
                {
                    insertInstanceCommand = new InsertInstanceCommand(file, StorageLocation);
                    processor.AddCommand(insertInstanceCommand);
                }

                // Create a context for applying actions from the rules engine
                ServerActionContext context =
                    new ServerActionContext(file, StorageLocation.FilesystemKey, _context.WorkQueueItem.ServerPartitionKey, _context.WorkQueueItem.StudyStorageKey);
                context.CommandProcessor = processor;

                // Run the rules engine against the object.
                _sopProcessedRulesEngine.Execute(context);

                // Do insert into the archival queue.  Note that we re-run this with each object processed
                // so that the scheduled time is pushed back each time.  Note, however, if the study only 
                // has one image, we could incorrectly insert an ArchiveQueue request, since the 
                // study rules haven't been run.  We re-run the command when the study processed
                // rules are run to remove out the archivequeue request again, if it isn't needed.
                context.CommandProcessor.AddCommand(
                    new InsertArchiveQueueCommand(_context.WorkQueueItem.ServerPartitionKey, StorageLocation.GetKey()));

                // Do the actual processing
                if (!processor.Execute())
                {
                    Platform.Log(LogLevel.Error, "Failure processing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
                    Platform.Log(LogLevel.Error, "File that failed processing: {0}", file.Filename);
                    throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid);
                }
                else
                {
                    Platform.Log(LogLevel.Info, "Processed SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid, patientsName);

                    if (insertInstanceCommand != null)
                        keys = insertInstanceCommand.InsertKeys;

                    if (keys != null)
                    {
                        // We've inserted a new Study, process Study Rules
                        if (keys.InsertStudy)
                        {
                            ProcessStudyRules(_context.WorkQueueItem, file);
                        }

                        // We've inserted a new Series, process Series Rules
                        if (keys.InsertSeries)
                        {
                            ProcessSeriesRules(_context.WorkQueueItem, file);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
                             processor.Description);
                processor.Rollback();
                throw new ApplicationException("Unexpected exception when processing file.", e);
            }
            finally
            {
                if (insertInstanceCommand != null && insertInstanceCommand.Statistics.IsSet)
                    _instanceStats.InsertDBTime.Add(insertInstanceCommand.Statistics);
                if (insertStudyXmlCommand != null && insertStudyXmlCommand.Statistics.IsSet)
                    _instanceStats.InsertStreamTime.Add(insertStudyXmlCommand.Statistics);

                        
                _statistics.StudyInstanceUid = StorageLocation.StudyInstanceUid;
                if (String.IsNullOrEmpty(modality) == false)
                    _statistics.Modality = modality;

                // Update the statistics
                _statistics.NumInstances++;

            }
        }
        
        /// <summary>
        /// Schedules a reconciliation for the specified <see cref="DicomFile"/>
        /// </summary>
        /// <param name="file"></param>
        private void ScheduleReconcile(DicomFile file)
        {
            ImageReconciler reconciler = new ImageReconciler();
            reconciler.ExistingStudy = _context.Study;
            reconciler.ExistingStudyLocation = StorageLocation;
            reconciler.Partition = _context.Partition;
            reconciler.ReconcileImage(file);
        }


        /// <summary>
        /// Process all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item.</param>
        /// <returns>A value indicating whether the uid list has been successfully processed</returns>
        private bool ProcessUidList(Model.WorkQueue item)
        {
            StudyXml studyXml;

            studyXml = LoadStudyXml(StorageLocation);

            int successfulProcessCount = 0;

            foreach (WorkQueueUid sop in WorkQueueUidList)
            {
                if (sop.Failed)
                    continue;

                if (ProcessWorkQueueUid(item, sop, studyXml))
                    successfulProcessCount++;
            }

            //TODO: Should we return true only if ALL uids have been processed instead?
            return successfulProcessCount > 0;

        }
        /// <summary>
        /// Process a specified <see cref="WorkQueueUid"/>
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="sop">The <see cref="WorkQueueUid"/> being processed</param>
        /// <param name="studyXml">The <see cref="StudyXml"/> object for the study being processed</param>
        /// <returns>true if the <see cref="WorkQueueUid"/> is successfully processed. false otherwise</returns>
        private bool ProcessWorkQueueUid(Model.WorkQueue item, WorkQueueUid sop, StudyXml studyXml)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(sop, "sop");
            Platform.CheckForNullReference(studyXml, "studyXml");

            OnProcessUidBegin(item, sop);
            
            string basePath = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
            basePath = Path.Combine(basePath, sop.SopInstanceUid);
            string path;
            if (sop.Extension != null)
                path = basePath + "." + sop.Extension;
            else
                path = basePath + ".dcm";

            try
            {
                
                // This is a bit trickly.  If the partition has the "AcceptLatest" policy configured, the 
                // duplicate bit is set, and no extension is set.  In this case we want to process the 
                // file as normal.  When the "CompareDuplicates" policy is set, we don't want to process,
                // we just want to compare to the original object.  In this case, we have an extension
                // set along with the duplicate flag.
                if (sop.Duplicate && sop.Extension != null)
                {
                    ProcessDuplicate(basePath + ".dcm", path);
                }
                else

                    ProcessFile(item, sop, path, studyXml);
                
                // Delete it out of the queue
                DeleteWorkQueueUid(sop);
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                if (e.InnerException != null)
                    item.FailureDescription = e.InnerException.Message;
                else
                    item.FailureDescription = e.Message;

                sop.FailureCount++;
                if ((sop.FailureCount > WorkQueueSettings.Instance.WorkQueueMaxFailureCount) || sop.Duplicate)
                {
                    sop.Failed = true;
                    if (sop.Extension != null)
                        for (int i = 1; i < 999; i++)
                        {
                            string extension = String.Format("bad{0}.dcm", i);
                            string newPath = basePath + "." + extension;
                            if (!File.Exists(newPath))
                            {
								if (File.Exists(path))
								{
									sop.Extension = extension;
									File.Move(path, newPath);
								}
                            	break;
                            }
                        }
                }
                UpdateWorkQueueUid(sop);
                return false;
                
            }
            finally
            {
                OnProcessUidEnd(item, sop);
            }

            
        }


        #endregion

        #region Protected Methods

        /// <summary>
        /// Called before the specified <see cref="WorkQueueUid"/> is processed
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="uid">The <see cref="WorkQueueUid"/> being processed</param>
        protected virtual void OnProcessUidBegin(Model.WorkQueue item, WorkQueueUid uid)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(uid, "uid");

            _instanceStats = new InstanceStatistics();
            _instanceStats.ProcessTime.Start();
        }

        /// <summary>
        /// Called after the specified <see cref="WorkQueueUid"/> has been processed
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="uid">The <see cref="WorkQueueUid"/> being processed</param>
        protected virtual void OnProcessUidEnd(Model.WorkQueue item, WorkQueueUid uid)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(uid, "uid");

            if (_sopProcessedRulesEngine != null)
            {
                if (_sopProcessedRulesEngine.Statistics.LoadTime.IsSet)
                    _instanceStats.SopRulesLoadTime.Add(_sopProcessedRulesEngine.Statistics.LoadTime);

                if (_sopProcessedRulesEngine.Statistics.ExecutionTime.IsSet)
                    _instanceStats.SopEngineExecutionTime.Add(_sopProcessedRulesEngine.Statistics.ExecutionTime);


                _sopProcessedRulesEngine.Statistics.Reset();
            }

            if (_studyProcessedRulesEngine != null)
            {
                if (_studyProcessedRulesEngine.Statistics.LoadTime.IsSet)
                    _instanceStats.StudyRulesLoadTime.Add(_studyProcessedRulesEngine.Statistics.LoadTime);

                if (_studyProcessedRulesEngine.Statistics.ExecutionTime.IsSet)
                    _instanceStats.StudyEngineExecutionTime.Add(_studyProcessedRulesEngine.Statistics.ExecutionTime);

                _studyProcessedRulesEngine.Statistics.Reset();
            }

            if (_seriesProcessedRulesEngine != null)
            {

                if (_seriesProcessedRulesEngine.Statistics.LoadTime.IsSet)
                    _instanceStats.SeriesRulesLoadTime.Add(_seriesProcessedRulesEngine.Statistics.LoadTime);
                if (_seriesProcessedRulesEngine.Statistics.ExecutionTime.IsSet)
                    _instanceStats.SeriesEngineExecutionTime.Add(_seriesProcessedRulesEngine.Statistics.ExecutionTime);

                _seriesProcessedRulesEngine.Statistics.Reset();
            }

            _instanceStats.ProcessTime.End();

            _statistics.AddSubStats(_instanceStats);
        }

        #endregion

        #region Overridden Protected Method


        protected override void OnProcessItemEnd(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            _statistics.UidsLoadTime.Add(UidsLoadTime);
            _statistics.StorageLocationLoadTime.Add(StorageLocationLoadTime);
            _statistics.StudyXmlLoadTime.Add(StudyXmlLoadTime);
            _statistics.DBUpdateTime.Add(DBUpdateTime);

            if (_statistics.NumInstances > 0)
            {
                _statistics.CalculateAverage();
                StatisticsLogger.Log(LogLevel.Info, false, _statistics);
            }
        }

        protected override  void OnProcessItemBegin(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            _statistics = new StudyProcessStatistics();
            _statistics.Description = String.Format("{0}[Key={1}]", item.WorkQueueTypeEnum, item.Key.Key);
        }
        
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            
            bool successful = false;
        	bool batchProcessed = true;
            _statistics.TotalProcessTime.Add(
                    delegate
                    	{
                            //Load the specific UIDs that need to be processed.
                            LoadUids(item);

                            if (WorkQueueUidList.Count == 0)
                            {
                                successful = true;
                            	batchProcessed = false;
                            }
                            else
                            {
                                _context = new StudyProcessorContext(item);

                                // Load the rules engine
                                _sopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, item.ServerPartitionKey);
                                _sopProcessedRulesEngine.Load();
                                _statistics.SopProcessedEngineLoadTime.Add(_sopProcessedRulesEngine.Statistics.LoadTime);
            
                                //Load the storage location.
								if (!LoadStorageLocation(item))
								{
									Platform.Log(LogLevel.Warn, "Unable to find readable location when processing StudyProcess WorkQueue item, rescheduling");
									PostponeItem(item, item.ScheduledTime.AddMinutes(2), item.ExpirationTime.AddMinutes(2));
									return;
								}

                                _context.StorageLocation = StorageLocation;

                                // If the study is not in processing state, attempt to push it into this state
                                if (StorageLocation.QueueStudyStateEnum != QueueStudyStateEnum.ProcessingScheduled)
                                    LockStudyState(item, QueueStudyStateEnum.ProcessingScheduled);
                                
                                // Process the images in the list
                                successful = ProcessUidList(item);

                            }
                        }
                );

			if (successful)
				PostProcessing(item, batchProcessed, false, true);
			else
			{
                bool allFailedDuplicate = CollectionUtils.TrueForAll(WorkQueueUidList, delegate(WorkQueueUid uid)
                                                                               {
                                                                                   return uid.Duplicate && uid.Failed;
                                                                               });

                if (allFailedDuplicate)
                {
                    Platform.Log(LogLevel.Error, "All entries are duplicates");
                }

                bool failNow = allFailedDuplicate;

                if (failNow)
                {
                    PostProcessingFailure(item, true);
                    return;
                }
                else
                {
                    PostProcessingFailure(item, false);
                }
			}
				
        }


        protected override bool CanStart()
        {
            return true; // can start anytime
        }
        #endregion

        
    }


}
