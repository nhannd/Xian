#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
 
    /// <summary>
    /// Processor for 'StudyProcess' <see cref="WorkQueue"/> entries.
    /// </summary>
    public class StudyProcessItemProcessor : BaseItemProcessor, ICancelable
    {
        #region Private Members
        private ServerRulesEngine _sopProcessedRulesEngine;

        protected StudyProcessStatistics _statistics;
    	protected StudyProcessorContext _context;
    	private const string RECONCILE_STORAGE_FOLDER = "Reconcile";

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
	
        private void ProcessDuplicate(WorkQueueUid uid, string basePath, string duplicatePath)
        {
            DicomFile dupFile = new DicomFile(duplicatePath);
            dupFile.Load();
            if (!File.Exists(basePath))
            {
                // NOTE: This is special case. The file which caused dicom service to think this sop is a duplicate
                // no longer exists in the study folder. Perhaps it has been moved to another folder during auto reconciliation.
                // We have nothing to compare against so let's just throw it into the SIQ queue.
                CreateDuplicateSIQEntry(uid, dupFile, null);
            }
            else
            {
                DicomFile baseFile = new DicomFile(basePath);
                baseFile.Load();

                if (!dupFile.TransferSyntax.Equals(baseFile.TransferSyntax))
                {
                    string failure = String.Format("Base file transfer syntax is '{0}' while duplicate file has '{1}'",
                                                   baseFile.TransferSyntax, dupFile.TransferSyntax);

                    List<DicomAttributeComparisonResult> list = new List<DicomAttributeComparisonResult>();
                    DicomAttributeComparisonResult result = new DicomAttributeComparisonResult();
                    result.ResultType = ComparisonResultType.DifferentValues;
                    result.TagName = DicomTagDictionary.GetDicomTag(DicomTags.TransferSyntaxUid).Name;
                    result.Details = failure;
                    list.Add(result);
                    CreateDuplicateSIQEntry(uid, dupFile, list);
                }
                else
                {
                    List<DicomAttributeComparisonResult> failureReason = new List<DicomAttributeComparisonResult>();
                    if (baseFile.DataSet.Equals(dupFile.DataSet, ref failureReason))
                    {
                        Platform.Log(LogLevel.Info,
                                     "Duplicate SOP being processed is identical.  Removing SOP: {0}",
                                     baseFile.MediaStorageSopInstanceUid);

                        FileInfo file = new FileInfo(duplicatePath);
                        file.Delete();
                    }
                    else
                    {
                        CreateDuplicateSIQEntry(uid, dupFile, failureReason);
                    }
                }
                
            }
            
        }

        void CreateDuplicateSIQEntry(WorkQueueUid uid, DicomFile file, List<DicomAttributeComparisonResult> differences)
        {
            Platform.Log(LogLevel.Info, "Duplicate SOP is different from existing copy. Creating duplicate SIQ entry. SOP: {0}", uid.SopInstanceUid);

            using (ServerCommandProcessor processor = new ServerCommandProcessor("Create Duplicate SIQ Entry"))
            {
                InsertDuplicateQueueEntryCommand insertCommand = new InsertDuplicateQueueEntryCommand(uid.GroupID, StorageLocation, file, uid.RelativePath, differences);
                processor.AddCommand(insertCommand);
                processor.AddCommand(new UpdateDuplicateQueueEntryCommand(
                        delegate() { return insertCommand.QueueEntry; }, file));

                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));

                processor.Execute();
            }
            
        }



        /// <summary>
        /// Process a specific DICOM file related to a <see cref="WorkQueue"/> request.
        /// </summary>
        /// <param name="queueUid"></param>
        /// <param name="stream">The <see cref="StudyXml"/> file to update with information from the file.</param>
        /// <param name="file">The file being processed.</param>
        protected virtual void ProcessFile(WorkQueueUid queueUid, DicomFile file, StudyXml stream)
        {
            SopInstanceProcessor processor = new SopInstanceProcessor( _context);
 
			long fileSize;
			FileInfo fileInfo = new FileInfo(file.Filename);
			fileSize = fileInfo.Length;

			processor.InstanceStats.FileLoadTime.Start();
			processor.InstanceStats.FileLoadTime.End();
			processor.InstanceStats.FileSize = (ulong)fileSize;
			string sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "File:" + fileInfo.Name);
			processor.InstanceStats.Description = sopInstanceUid;
        	processor.ProcessFile(file, stream, queueUid.Duplicate, true);
			
			_statistics.StudyInstanceUid = StorageLocation.StudyInstanceUid;
			if (String.IsNullOrEmpty(processor.Modality) == false)
				_statistics.Modality = processor.Modality;

			// Update the statistics
			_statistics.NumInstances++;
        	_statistics.AddSubStats(processor.InstanceStats);
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

				if (CancelPending)
				{
					Platform.Log(LogLevel.Info, "Processing of study canceled for shutdown: {0}", StorageLocation.StudyInstanceUid);
					return true;	
				}

                if (ProcessWorkQueueUid(item, sop, studyXml))
                    successfulProcessCount++;
            }

            //TODO: Should we return true only if ALL uids have been processed instead?
            return successfulProcessCount > 0;

        }

        private String GetDuplicateUidPath(WorkQueueUid sop)
        {
            String dupPath;
            if (!String.IsNullOrEmpty(sop.RelativePath))
            {
                dupPath = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
                dupPath = Path.Combine(dupPath, RECONCILE_STORAGE_FOLDER);
                dupPath = Path.Combine(dupPath, sop.GroupID);
                dupPath = Path.Combine(dupPath, sop.RelativePath);
            }
                
            #region BACKWARD_COMPATIBILTY_CODE
            else
            {
                string basePath = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
                basePath = Path.Combine(basePath, sop.SopInstanceUid);
                if (sop.Extension != null)
                    dupPath = basePath + "." + sop.Extension;
                else
                    dupPath = basePath + ".dcm";
            }
            #endregion

            return dupPath;
        }

        private String GetDuplicateGroupPath(WorkQueueUid sop)
        {
            String groupFolderPath = null;
            if (!String.IsNullOrEmpty(sop.RelativePath))
            {
                groupFolderPath = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
                groupFolderPath = Path.Combine(groupFolderPath, RECONCILE_STORAGE_FOLDER);
                groupFolderPath = Path.Combine(groupFolderPath, sop.GroupID);
            }

            return groupFolderPath;
        }

        /// <summary>
        /// Process a specified <see cref="WorkQueueUid"/>
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item being processed</param>
        /// <param name="sop">The <see cref="WorkQueueUid"/> being processed</param>
        /// <param name="studyXml">The <see cref="StudyXml"/> object for the study being processed</param>
        /// <returns>true if the <see cref="WorkQueueUid"/> is successfully processed. false otherwise</returns>
        protected virtual bool ProcessWorkQueueUid(Model.WorkQueue item, WorkQueueUid sop, StudyXml studyXml)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(sop, "sop");
            Platform.CheckForNullReference(studyXml, "studyXml");

            OnProcessUidBegin(item, sop);

            string path = null;
            string basePath = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
            basePath = Path.Combine(basePath, sop.SopInstanceUid);
            
            try
            {  
                if (sop.Duplicate && sop.Extension != null)
                {
                    path = GetDuplicateUidPath(sop);
                    ProcessDuplicate(sop, basePath + ".dcm", path);
                }
                else
                {
                    path = StorageLocation.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid);
                    DicomFile file = new DicomFile(path);
                    file.Load(DicomReadOptions.StorePixelDataReferences);
                    ProcessFile(sop, file, studyXml);
                }
                
                // Delete it out of the queue
                DeleteWorkQueueUid(sop);
                return true;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                if (e.InnerException != null)
                    item.FailureDescription = String.Format("{0}:{1}", e.GetType().Name, e.InnerException.Message);
                else
                    item.FailureDescription = String.Format("{0}:{1}", e.GetType().Name, e.Message);

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

            if (uid.Duplicate)
            {
                String dupPath = GetDuplicateUidPath(uid);
                // Delete the container if it's empty
                FileInfo f = new FileInfo(dupPath);

                if (DirectoryUtility.DeleteIfEmpty(f.Directory.FullName))
                {
                    DirectoryUtility.DeleteIfEmpty(GetDuplicateGroupPath(uid));
                }

            }
            
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
            Platform.CheckForNullReference(StorageLocation, "StorageLocation");
            
            bool successful = false;
        	bool idle = false;
            _statistics.TotalProcessTime.Add(
                    delegate
                    	{
                            //Load the specific UIDs that need to be processed.
                            LoadUids(item);

                            if (WorkQueueUidList.Count == 0)
                            {
                                successful = true;
                            	idle = true;
                            }
                            else
                            {
                                _context = new StudyProcessorContext(StorageLocation);

                                // Load the rules engine
                                _sopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, item.ServerPartitionKey);
                                _sopProcessedRulesEngine.Load();
                                _statistics.SopProcessedEngineLoadTime.Add(_sopProcessedRulesEngine.Statistics.LoadTime);
                            	_context.SopProcessedRulesEngine = _sopProcessedRulesEngine;
                            	_context.ReadContext = ReadContext;

								if (Study != null)
								{
									Platform.Log(LogLevel.Info, "Processing study {0} for Patient {1} (PatientId:{2} A#:{3}), {4} objects",
									             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
									             Study.AccessionNumber, WorkQueueUidList.Count);
								}
								else
								{
									Platform.Log(LogLevel.Info, "Processing study {0}, {1} objects",
												 StorageLocation.StudyInstanceUid, WorkQueueUidList.Count);
								}
                            	// Process the images in the list
                                successful = ProcessUidList(item);
                            }
                        }
                );

			if (successful)
			{
				if (idle
					&& item.ExpirationTime <= Platform.Time)
				{
					// Run Study / Series Rules Engine.
					StudyRulesEngine engine = new StudyRulesEngine(StorageLocation, ServerPartition);
					engine.Apply(ServerRuleApplyTimeEnum.StudyProcessed);

					// Log the FilesystemQueue related entries
					StorageLocation.LogFilesystemQueue();

					// Delete the queue entry.
					PostProcessing(item,
					               WorkQueueProcessorStatus.Complete,
					               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				}
				else if (idle)
					PostProcessing(item,
								   WorkQueueProcessorStatus.IdleNoDelete, // Don't delete, so we ensure the rules engine is run later.
								   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				else
					PostProcessing(item,
								   WorkQueueProcessorStatus.Pending,
								   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			}
			else
			{
				bool allFailedDuplicate = CollectionUtils.TrueForAll(WorkQueueUidList, delegate(WorkQueueUid uid)
																			   {
																				   return uid.Duplicate && uid.Failed;
																			   });

				if (allFailedDuplicate)
				{
					Platform.Log(LogLevel.Error, "All entries are duplicates");

					PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
					return;
				}
				else
				{
					PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal);
				}
			}				
        }


        protected override bool CanStart()
        {
            // If the study is not in processing state, attempt to push it into this state
            // If it fails, postpone the processing instead of failing
            if (StorageLocation.QueueStudyStateEnum != QueueStudyStateEnum.ProcessingScheduled)
            {
                if (!LockStudyState(WorkQueueItem, QueueStudyStateEnum.ProcessingScheduled))
                {
                    Platform.Log(LogLevel.Debug,
                                 "StudyProcess cannot start at this point. Study is being locked by another processor. Current state={0}",
                                 StorageLocation.QueueStudyStateEnum);
                	PostponeItem(WorkQueueItem);
                    return false;
                }
            }

            return true;
        }
        #endregion        
    }
}
