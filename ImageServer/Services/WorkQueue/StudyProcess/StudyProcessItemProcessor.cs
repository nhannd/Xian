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
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Common.Exceptions;
using ClearCanvas.ImageServer.Common.Utilities;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Reconcile;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess
{
    /// <summary>
    /// Processor for 'StudyProcess' <see cref="WorkQueue"/> entries.
    /// </summary>
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery= RecoveryModes.Automatic)]
    public class StudyProcessItemProcessor : BaseItemProcessor, ICancelable
    {
        #region Private Members
        private ServerRulesEngine _sopProcessedRulesEngine;

        protected StudyProcessStatistics Statistics;
    	protected StudyProcessorContext Context;
        #endregion

        #region Public Properties
       
        #endregion

        #region Constructors
        public StudyProcessItemProcessor()
        {
            Statistics = new StudyProcessStatistics();            
        }

        #endregion Constructors

        #region Private Methods
	
        private void ProcessDuplicate(DicomFile dupFile, WorkQueueUid uid)
        {
            string duplicateSopPath = GetDuplicateUidPath(uid);
            string basePath = StorageLocation.GetSopInstancePath(uid.SeriesInstanceUid, uid.SopInstanceUid);
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
					// If they're compressed, and we have a codec, lets decompress and still do the comparison
					if (dupFile.TransferSyntax.Encapsulated 
						&& !dupFile.TransferSyntax.LossyCompressed
						&& DicomCodecRegistry.GetCodec(dupFile.TransferSyntax)!=null)
					{
						dupFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
					}

					if (baseFile.TransferSyntax.Encapsulated
						&& !baseFile.TransferSyntax.LossyCompressed
						&& DicomCodecRegistry.GetCodec(baseFile.TransferSyntax) != null)
					{
						baseFile.ChangeTransferSyntax(TransferSyntax.ExplicitVrLittleEndian);
					}

					if (dupFile.TransferSyntax.Encapsulated || baseFile.TransferSyntax.Encapsulated)
					{
						string failure = String.Format("Base file transfer syntax is '{0}' while duplicate file has '{1}'",
						                               baseFile.TransferSyntax, dupFile.TransferSyntax);

						List<DicomAttributeComparisonResult> list = new List<DicomAttributeComparisonResult>();
						DicomAttributeComparisonResult result = new DicomAttributeComparisonResult
						                                        	{
						                                        		ResultType = ComparisonResultType.DifferentValues,
						                                        		TagName =
						                                        			DicomTagDictionary.GetDicomTag(DicomTags.TransferSyntaxUid).Name,
						                                        		Details = failure
						                                        	};
						list.Add(result);
						CreateDuplicateSIQEntry(uid, dupFile, list);
						return;
					}
				}

				List<DicomAttributeComparisonResult> failureReason = new List<DicomAttributeComparisonResult>();
				if (baseFile.DataSet.Equals(dupFile.DataSet, ref failureReason))
				{
					Platform.Log(LogLevel.Info,
					             "Duplicate SOP being processed is identical.  Removing SOP: {0}",
					             baseFile.MediaStorageSopInstanceUid);

                    RemoveWorkQueueUid(uid, duplicateSopPath);
                }
				else
				{
					CreateDuplicateSIQEntry(uid, dupFile, failureReason);
				}
			}
        }

        void CreateDuplicateSIQEntry(WorkQueueUid uid, DicomFile file, List<DicomAttributeComparisonResult> differences)
        {
            Platform.Log(LogLevel.Info, "Duplicate SOP is different from existing copy. Creating duplicate SIQ entry. SOP: {0}", uid.SopInstanceUid);

            using (ServerCommandProcessor processor = new ServerCommandProcessor("Create Duplicate SIQ Entry"))
            {
            	InsertOrUpdateEntryCommand insertCommand = new InsertOrUpdateEntryCommand(uid.GroupID, StorageLocation,
                                                                  file,
                                                                  string.IsNullOrEmpty(uid.RelativePath)
                                                                  	? Path.Combine(StorageLocation.StudyInstanceUid, uid.SopInstanceUid + "." + uid.Extension) 
																	: uid.RelativePath, 
																	differences);
                processor.AddCommand(insertCommand);

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
        /// <param name="compare">Indicates whether to compare the DICOM file against the study in the system.</param>
        protected virtual void ProcessFile(WorkQueueUid queueUid, DicomFile file, StudyXml stream, bool compare)
        {
            SopInstanceProcessor processor = new SopInstanceProcessor(Context) {EnforceNameRules = true};

        	FileInfo fileInfo = new FileInfo(file.Filename);
			long fileSize = fileInfo.Length;

			processor.InstanceStats.FileLoadTime.Start();
			processor.InstanceStats.FileLoadTime.End();
			processor.InstanceStats.FileSize = (ulong)fileSize;
			string sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "File:" + fileInfo.Name);
			processor.InstanceStats.Description = sopInstanceUid;

            string group = queueUid.GroupID ?? ServerHelper.GetUidGroup(file, ServerPartition, WorkQueueItem.InsertTime);

            ProcessingResult result = processor.ProcessFile(group, file, stream, compare, true, queueUid, null);

            if (result.Status == ProcessingStatus.Reconciled)
            {
                // file has been saved by SopInstanceProcessor in another place for reconcilation
                // Note: SopInstanceProcessor has removed the WorkQueueUid so we
                // only need to delete the file here.
                FileUtils.Delete(fileInfo.FullName);
            }
			
			Statistics.StudyInstanceUid = StorageLocation.StudyInstanceUid;
			if (String.IsNullOrEmpty(processor.Modality) == false)
				Statistics.Modality = processor.Modality;

			// Update the statistics
			Statistics.NumInstances++;
        	Statistics.AddSubStats(processor.InstanceStats);
        }
        

        /// <summary>
        /// Process all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
        /// </summary>
        /// <param name="item">The <see cref="WorkQueue"/> item.</param>
        /// <returns>Number of instances that have been processed successfully.</returns>
        private int ProcessUidList(Model.WorkQueue item)
        {
        	StudyXml studyXml = LoadStudyXml(StorageLocation);

            int successfulProcessCount = 0;

            foreach (WorkQueueUid sop in WorkQueueUidList)
            {
                if (sop.Failed)
                    continue;

				if (CancelPending)
				{
					Platform.Log(LogLevel.Info, "Processing of study canceled for shutdown: {0}", StorageLocation.StudyInstanceUid);
                    return successfulProcessCount;	
				}

                if (ProcessWorkQueueUid(item, sop, studyXml))
                    successfulProcessCount++;
            }

            return successfulProcessCount;
        }

		private String GetDuplicateUidPath(WorkQueueUid sop)
		{
			string dupPath = Path.Combine(StorageLocation.FilesystemPath, StorageLocation.PartitionFolder);
			dupPath = Path.Combine(dupPath, ServerPlatform.ReconcileStorageFolder);
			dupPath = Path.Combine(dupPath, sop.GroupID);
			dupPath = string.IsNullOrEmpty(sop.RelativePath)
			          	? Path.Combine(dupPath,
			          	               Path.Combine(StorageLocation.StudyInstanceUid, sop.SopInstanceUid + "." + sop.Extension))
			          	: Path.Combine(dupPath, sop.RelativePath);

			#region BACKWARD_COMPATIBILTY_CODE

			if (string.IsNullOrEmpty(sop.RelativePath) && !File.Exists(dupPath))
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
				groupFolderPath = Path.Combine(groupFolderPath, ServerPlatform.ReconcileStorageFolder);
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
            
            try
            {
                if (sop.Duplicate && sop.Extension != null)
                {
                    path = GetDuplicateUidPath(sop);
                    DicomFile file = new DicomFile(path);
                    file.Load();

                    InstancePreProcessingResult result = PreProcessFile(sop, file);

                    if (false ==file.DataSet[DicomTags.StudyInstanceUid].ToString().Equals(StorageLocation.StudyInstanceUid) 
                            || result.DiscardImage)
                    {
                        RemoveWorkQueueUid(sop, null);
                    }
                    else {
                    	ProcessDuplicate(file, sop);
                    }
                }
                else
                {
                    try
                    {
                        path = StorageLocation.GetSopInstancePath(sop.SeriesInstanceUid, sop.SopInstanceUid);
                        DicomFile file = new DicomFile(path);
                        file.Load();

                        InstancePreProcessingResult result = PreProcessFile(sop, file);

                        if (false == file.DataSet[DicomTags.StudyInstanceUid].ToString().Equals(StorageLocation.StudyInstanceUid) 
                            || result.DiscardImage)
                        {
                            RemoveWorkQueueUid(sop, path);
                        }
                        else
                        {
                            ProcessFile(sop, file, studyXml, result.AutoReconciled? false:true);
                        }
                    }
                    catch (DicomException ex)
                    {
                        // bad file. Remove it from the filesystem and the queue
                        RemoveBadDicomFile(path, ex.Message);
                        DeleteWorkQueueUid(sop);
                        return false;
                    }
                    
                }
                
                return true;
            }
            catch (StudyIsNearlineException)
            {
                // handled by caller
                throw;
            }
            catch (Exception e)
            {
                Platform.Log(LogLevel.Error, e, "Unexpected exception when processing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                item.FailureDescription = e.InnerException != null ? 
					String.Format("{0}:{1}", e.GetType().Name, e.InnerException.Message) : String.Format("{0}:{1}", e.GetType().Name, e.Message);

				//No longer needed.  Update was moved into the SopInstanceProcessor
                //sop.FailureCount++;
                //UpdateWorkQueueUid(sop);
                return false;
                
            }
            finally
            {
                OnProcessUidEnd(item, sop);
            }            
        }
        #endregion


        /// <summary>
        /// Apply changes to the file prior to processing it.
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="file"></param>
        protected virtual InstancePreProcessingResult PreProcessFile(WorkQueueUid uid, DicomFile file)
        {
            String contextID = uid.GroupID ?? String.Format("{0}_{1}",
                String.IsNullOrEmpty(file.SourceApplicationEntityTitle) ? ServerPartition.AeTitle : file.SourceApplicationEntityTitle, 
                WorkQueueItem.InsertTime.ToString("yyyyMMddHHmmss"));

            InstancePreProcessingResult result = new InstancePreProcessingResult();
            
            PatientNameRules patientNameRules = new PatientNameRules(Study);
            UpdateItem updateItem = patientNameRules.Apply(file);

            result.Modified = updateItem != null;

            AutoReconciler autoBaseReconciler = new AutoReconciler(contextID, StorageLocation);
            InstancePreProcessingResult reconcileResult = autoBaseReconciler.Process(file);
            result.AutoReconciled = reconcileResult != null;
            result.Modified |= reconcileResult != null;
            
            if (reconcileResult!=null && reconcileResult.DiscardImage)
            {
                result.DiscardImage = true;
            }

            // if the studyuid is modified, the file will be deleted by the caller.
            if (file.DataSet[DicomTags.StudyInstanceUid].ToString().Equals(StorageLocation.StudyInstanceUid))
            {
                if (result.Modified)
                    file.Save();
            }

            
            return result;
        }

        private static void RemoveWorkQueueUid(WorkQueueUid uid, string fileToDelete)
        {
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Remove Work Queue Uid"))
            {
                processor.AddCommand(new DeleteWorkQueueUidCommand(uid));
                if (String.IsNullOrEmpty(fileToDelete) == false)
                {
                    processor.AddCommand(new FileDeleteCommand(fileToDelete, true));

                }

                if (!processor.Execute())
                {
                    String error = String.Format("Unable to delete Work Queue Uid {0}: {1}", uid.Key, processor.FailureReason);
                    Platform.Log(LogLevel.Error, error);
                    throw new ApplicationException(error, processor.FailureException);
                }
            }

        }


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

                if (f.Directory!=null && DirectoryUtility.DeleteIfEmpty(f.Directory.FullName))
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
            base.OnProcessItemEnd(item);

            Statistics.UidsLoadTime.Add(UidsLoadTime);
            Statistics.StorageLocationLoadTime.Add(StorageLocationLoadTime);
            Statistics.StudyXmlLoadTime.Add(StudyXmlLoadTime);
            Statistics.DBUpdateTime.Add(DBUpdateTime);

            if (Statistics.NumInstances > 0)
            {
                Statistics.CalculateAverage();
                StatisticsLogger.Log(LogLevel.Info, false, Statistics);
            }

        }

        protected override  void OnProcessItemBegin(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");

            Statistics = new StudyProcessStatistics
                          	{
                          		Description = String.Format("{0}[Key={1}]", item.WorkQueueTypeEnum, item.Key.Key)
                          	};
        }
        
        protected override void ProcessItem(Model.WorkQueue item)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(StorageLocation, "StorageLocation");
            Statistics.TotalProcessTime.Start();
            
            bool successful;
        	bool idle = false;
            //Load the specific UIDs that need to be processed.
            LoadUids(item);

            int totalUidCount = WorkQueueUidList.Count;

            if (totalUidCount == 0)
            {
                successful = true;
                idle = true;
            }
            else
            {
                try
                {
                    Context = new StudyProcessorContext(StorageLocation);

                    // Load the rules engine
                    _sopProcessedRulesEngine = new ServerRulesEngine(ServerRuleApplyTimeEnum.SopProcessed, item.ServerPartitionKey);
                    _sopProcessedRulesEngine.AddOmittedType(ServerRuleTypeEnum.SopCompress);
                    _sopProcessedRulesEngine.Load();
                    Statistics.SopProcessedEngineLoadTime.Add(_sopProcessedRulesEngine.Statistics.LoadTime);
                    Context.SopProcessedRulesEngine = _sopProcessedRulesEngine;
                    
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
                    successful = ProcessUidList(item) > 0;
                }
                catch (StudyIsNearlineException ex)
                {
                    // delay until the target is restored
                    // NOTE: If the study could not be restored after certain period of time, this entry will be failed.
                    if (ex.RestoreRequested)
                    {
                        PostponeItem(string.Format("Unable to auto-reconcile at this time: the target study {0} is not online yet. Restore has been requested.", ex.StudyInstanceUid));
                        return;
                    }
                	// fail right away
                	FailQueueItem(item, string.Format("Unable to auto-reconcile at this time: the target study {0} is not nearline and could not be restored.", ex.StudyInstanceUid));
                	return;
                }
            }
            Statistics.TotalProcessTime.End();

			if (successful)
			{
				if (idle && item.ExpirationTime <= Platform.Time)
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
				bool allFailedDuplicate = CollectionUtils.TrueForAll(WorkQueueUidList, uid => uid.Duplicate && uid.Failed);

				if (allFailedDuplicate)
				{
					Platform.Log(LogLevel.Error, "All entries are duplicates");

					PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
					return;
				}
				PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal);
			}				
        }


        protected override bool CanStart()
        {
            // If the study is not in processing state, attempt to push it into this state
            // If it fails, postpone the processing instead of failing
            if (StorageLocation.QueueStudyStateEnum != QueueStudyStateEnum.ProcessingScheduled)
            {
				string failureReason;
				if (!ServerHelper.LockStudy(WorkQueueItem.StudyStorageKey, QueueStudyStateEnum.ProcessingScheduled, out failureReason))
				{
					Platform.Log(LogLevel.Debug,
								 "StudyProcess cannot start at this point. Study is being locked by another processor. WriteLock Failure reason={0}",
								 failureReason);
					PostponeItem(String.Format("Study is being locked by another processor: {0}", failureReason));
                    return false;
                }
            }

            return true;
        }
        #endregion        
    }
}
