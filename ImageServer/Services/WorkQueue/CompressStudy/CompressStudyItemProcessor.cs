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
using System.Xml;
using ClearCanvas.Common;
using ClearCanvas.Common.Statistics;
using ClearCanvas.Dicom;
using ClearCanvas.Dicom.Codec;
using ClearCanvas.Dicom.Iod.Modules;
using ClearCanvas.Dicom.Utilities.Xml;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.ImageServer.Common;
using ClearCanvas.ImageServer.Common.CommandProcessor;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Model;
using ClearCanvas.ImageServer.Model.Brokers;
using ClearCanvas.ImageServer.Model.EntityBrokers;
using ClearCanvas.ImageServer.Model.Parameters;
using ClearCanvas.ImageServer.Rules;

namespace ClearCanvas.ImageServer.Services.WorkQueue.CompressStudy
{

    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    public class CompressStudyItemProcessor : BaseItemProcessor, ICancelable
	{
		#region Private Members
		private CompressInstanceStatistics _instanceStats;
		private CompressStudyStatistics _studyStats;
		#endregion

        #region Protected Properties
        /// <summary>
        /// Gets the new transfer syntax of the study after the compression.
        /// </summary>
        protected TransferSyntax CompressTransferSyntax { get; private set; }
        
        #endregion
        protected override void Initialize(Model.WorkQueue item)
        {
            base.Initialize(item);

            XmlElement element = WorkQueueItem.Data.DocumentElement;
            string syntax = element.Attributes["syntax"].Value;
            CompressTransferSyntax = TransferSyntax.GetTransferSyntax(syntax);   
        }

        #region Private Methods
        private bool ProcessWorkQueueUid(Model.WorkQueue item, WorkQueueUid sop, StudyXml studyXml, IDicomCodecFactory theCodecFactory)
        {
            Platform.CheckForNullReference(item, "item");
            Platform.CheckForNullReference(sop, "sop");
            Platform.CheckForNullReference(studyXml, "studyXml");

            if (!studyXml.Contains(sop.SeriesInstanceUid, sop.SopInstanceUid))
            {
                // Uid was inserted but not in the study xml.
                // Auto-recovery might have detect problem with that file and remove it from the study.
                // Assume the study xml has been corrected and ignore the uid.
                Platform.Log(LogLevel.Warn, "Skipping SOP {0} in series {1}. It is no longer part of the study.", sop.SopInstanceUid, sop.SeriesInstanceUid);

                // Delete it out of the queue
                DeleteWorkQueueUid(sop);
                return true;
            }

            string basePath = Path.Combine(StorageLocation.GetStudyPath(), sop.SeriesInstanceUid);
            basePath = Path.Combine(basePath, sop.SopInstanceUid);
            string path;
            if (sop.Extension != null)
                path = basePath + "." + sop.Extension;
            else
                path = basePath + ServerPlatform.DicomFileExtension;

            try
            {
                ProcessFile(item, sop, path, studyXml, theCodecFactory);

                // WorkQueueUid has been deleted out by the processor

                return true;
            }
            catch (Exception e)
            {
                if (e.InnerException != null && e.InnerException is DicomCodecUnsupportedSopException)
                {
                    Platform.Log(LogLevel.Warn, e, "Instance not supported for compressor: {0}.  Deleting WorkQueue entry for SOP {1}", e.Message, sop.SopInstanceUid);

                    item.FailureDescription = e.InnerException != null ? e.InnerException.Message : e.Message;

                    // Delete it out of the queue
                    DeleteWorkQueueUid(sop);

                    return false;
                }
                Platform.Log(LogLevel.Error, e, "Unexpected exception when compressing file: {0} SOP Instance: {1}", path, sop.SopInstanceUid);
                item.FailureDescription = e.InnerException != null ? e.InnerException.Message : e.Message;

                sop.FailureCount++;

                UpdateWorkQueueUid(sop);

                return false;
            }
        }

        private void ReinsertFilesystemQueue(TimeSpan delay)
        {
            using (IUpdateContext updateContext = PersistentStoreRegistry.GetDefaultStore().OpenUpdateContext(UpdateContextSyncMode.Flush))
            {
                IWorkQueueUidEntityBroker broker = updateContext.GetBroker<IWorkQueueUidEntityBroker>();
                WorkQueueUidSelectCriteria workQueueUidCriteria = new WorkQueueUidSelectCriteria();
                workQueueUidCriteria.WorkQueueKey.EqualTo(WorkQueueItem.Key);
                broker.Delete(workQueueUidCriteria);

                FilesystemQueueInsertParameters parms = new FilesystemQueueInsertParameters();
                parms.FilesystemQueueTypeEnum = CompressTransferSyntax.LosslessCompressed 
					? FilesystemQueueTypeEnum.LosslessCompress 
					: FilesystemQueueTypeEnum.LossyCompress;
                parms.ScheduledTime = Platform.Time + delay;
                parms.StudyStorageKey = WorkQueueItem.StudyStorageKey;
                parms.FilesystemKey = StorageLocation.FilesystemKey;

                parms.QueueXml = WorkQueueItem.Data;

                IInsertFilesystemQueue insertQueue = updateContext.GetBroker<IInsertFilesystemQueue>();

                if (false == insertQueue.Execute(parms))
                {
                    Platform.Log(LogLevel.Error, "Unexpected failure inserting FilesystemQueue entry");
                }
                else
                    updateContext.Commit();
            }
        } 
        #endregion
		/// <summary>
		/// Process all of the SOP Instances associated with a <see cref="WorkQueue"/> item.
		/// </summary>
		/// <param name="item">The <see cref="WorkQueue"/> item.</param>
		/// <returns>A value indicating whether the uid list has been successfully processed</returns>
		/// <param name="theCodecFactory">The factor for doing the compression</param>
		protected bool ProcessUidList(Model.WorkQueue item, IDicomCodecFactory theCodecFactory)
		{
			StudyXml studyXml = LoadStudyXml(StorageLocation);

            int successfulProcessCount = 0;
			int totalCount = WorkQueueUidList.Count;
			foreach (WorkQueueUid sop in WorkQueueUidList)
			{
				if (sop.Failed)
					continue;

				if (CancelPending)
				{
					Platform.Log(LogLevel.Info,
					             "Received cancel request while compressing study {0}.  {1} instances successfully processed.",
					             StorageLocation.StudyInstanceUid, successfulProcessCount);

					return successfulProcessCount > 0;
				}

				if (ProcessWorkQueueUid(item, sop, studyXml, theCodecFactory))
					successfulProcessCount++;
			}

			if (successfulProcessCount > 0)
				Platform.Log(LogLevel.Info,"Completed compression of study {0}. {1} instances successfully processed.",
                    StorageLocation.StudyInstanceUid, successfulProcessCount);

				return successfulProcessCount > 0 && totalCount == successfulProcessCount;
		}

		protected void ProcessFile(Model.WorkQueue item, WorkQueueUid sop, string path, StudyXml studyXml, IDicomCodecFactory theCodecFactory)
		{
			DicomFile file;

			_instanceStats = new CompressInstanceStatistics();

			_instanceStats.ProcessTime.Start();

			// Use the command processor for rollback capabilities.
            using (ServerCommandProcessor processor = new ServerCommandProcessor("Processing WorkQueue Compress DICOM File"))
            {
                string modality = String.Empty;

                try
                {
                    file = new DicomFile(path);

                    _instanceStats.FileLoadTime.Start();
                    file.Load(DicomReadOptions.StorePixelDataReferences | DicomReadOptions.Default);
                    _instanceStats.FileLoadTime.End();

					if (!file.TransferSyntax.Encapsulated)
					{
						//TODO: just move this right into ChangeTransferSyntax?

						// Check if Overlay is embedded in pixels
						OverlayPlaneModuleIod overlayIod = new OverlayPlaneModuleIod(file.DataSet);
						for (int i = 0; i < 16; i++)
						{
							if (overlayIod.HasOverlayPlane(i))
							{
								Platform.Log(ServerPlatform.InstanceLogLevel, "SOP Instance {0} has embedded overlay in pixel data, extracting", file.MediaStorageSopInstanceUid);
								OverlayPlane overlay = overlayIod[i];
								if (overlay.OverlayData == null)
								{
									DicomUncompressedPixelData pd = new DicomUncompressedPixelData(file);
									overlay.ConvertEmbeddedOverlay(pd);
								}
							}
						}
					}

                	modality = file.DataSet[DicomTags.Modality].GetString(0, String.Empty);

                    FileInfo fileInfo = new FileInfo(path);
                    _instanceStats.FileSize = (ulong)fileInfo.Length;

                    // Get the Patients Name for processing purposes.
                    String patientsName = file.DataSet[DicomTags.PatientsName].GetString(0, "");

                    if (file.TransferSyntax.Equals(theCodecFactory.CodecTransferSyntax))
                    {
						// Delete the WorkQueueUid item
						processor.AddCommand(new DeleteWorkQueueUidCommand(sop));

						// Do the actual processing
						if (!processor.Execute())
						{
							Platform.Log(LogLevel.Warn, "Failure deleteing WorkQueueUid: {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
							Platform.Log(LogLevel.Warn, "Compression file that failed: {0}", file.Filename);
						}
						else
						{
							Platform.Log(LogLevel.Warn, "Skip compressing SOP {0}. Its current transfer syntax is {1}",
							             file.MediaStorageSopInstanceUid, file.TransferSyntax.Name);
						}
                    }
                    else
                    {
                        IDicomCodec codec = theCodecFactory.GetDicomCodec();

                        // Create a context for applying actions from the rules engine
                        ServerActionContext context = new ServerActionContext(file, StorageLocation.FilesystemKey, ServerPartition, item.StudyStorageKey);
                        context.CommandProcessor = processor;
                        
                        DicomCodecParameters parms = theCodecFactory.GetCodecParameters(item.Data);
                        DicomCompressCommand compressCommand =
                            new DicomCompressCommand(context.Message, theCodecFactory.CodecTransferSyntax, codec, parms);
                        processor.AddCommand(compressCommand);

                        SaveDicomFileCommand save = new SaveDicomFileCommand(file.Filename, file, false);
                        processor.AddCommand(save);

                        // Update the StudyStream object, must be done after compression
                        // and after the compressed image has been successfully saved
                        UpdateStudyXmlCommand insertStudyXmlCommand = new UpdateStudyXmlCommand(file, studyXml, StorageLocation);
                        processor.AddCommand(insertStudyXmlCommand);

						// Delete the WorkQueueUid item
                    	processor.AddCommand(new DeleteWorkQueueUidCommand(sop));

                        // Do the actual processing
                        if (!processor.Execute())
                        {
                            _instanceStats.CompressTime.Add(compressCommand.CompressTime);
                            Platform.Log(LogLevel.Error, "Failure compressing command {0} for SOP: {1}", processor.Description, file.MediaStorageSopInstanceUid);
                            Platform.Log(LogLevel.Error, "Compression file that failed: {0}", file.Filename);
                            throw new ApplicationException("Unexpected failure (" + processor.FailureReason + ") executing command for SOP: " + file.MediaStorageSopInstanceUid,processor.FailureException);
                        }
                    	_instanceStats.CompressTime.Add(compressCommand.CompressTime);
                    	Platform.Log(ServerPlatform.InstanceLogLevel, "Compress SOP: {0} for Patient {1}", file.MediaStorageSopInstanceUid,
                    	             patientsName);
                    }
                    
                }
                catch (Exception e)
                {
                    Platform.Log(LogLevel.Error, e, "Unexpected exception when {0}.  Rolling back operation.",
                                 processor.Description);
                    processor.Rollback();

                	throw;
                }
                finally
                {
                    _instanceStats.ProcessTime.End();
                    _studyStats.AddSubStats(_instanceStats);

                    _studyStats.StudyInstanceUid = StorageLocation.StudyInstanceUid;
                    if (String.IsNullOrEmpty(modality) == false)
                        _studyStats.Modality = modality;

                    // Update the statistics
                    _studyStats.NumInstances++;
                }
            }
		}

		protected override void ProcessItem(Model.WorkQueue item)
		{
			LoadUids(item);

			if (WorkQueueUidList.Count == 0)
			{
				// No UIDs associated with the WorkQueue item.  Set the status back to idle
				PostProcessing(item,
				               WorkQueueProcessorStatus.Idle,
				               WorkQueueProcessorDatabaseUpdate.ResetQueueState);
				return;
			}


			XmlElement element = item.Data.DocumentElement;

			string syntax = element.Attributes["syntax"].Value;

			TransferSyntax compressSyntax = TransferSyntax.GetTransferSyntax(syntax);
			if (compressSyntax == null)
			{
				item.FailureDescription =
					String.Format("Invalid transfer syntax in compression WorkQueue item: {0}", element.Attributes["syntax"].Value);
				Platform.Log(LogLevel.Error, "Error with work queue item {0}: {1}", item.GetKey(), item.FailureDescription);
				base.PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
				return;
			}

			if (Study == null)
			{
				item.FailureDescription =
					String.Format("Compression item does not have a linked Study record");
				Platform.Log(LogLevel.Error, "Error with work queue item {0}: {1}", item.GetKey(), item.FailureDescription);
				base.PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
				return;
			}

			Platform.Log(LogLevel.Info,
			             "Compressing study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4} to {5}",
			             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
			             Study.AccessionNumber, ServerPartition.Description, compressSyntax.Name);

			IDicomCodecFactory[] codecs = DicomCodecRegistry.GetCodecFactories();
			IDicomCodecFactory theCodecFactory = null;
			foreach (IDicomCodecFactory codec in codecs)
				if (codec.CodecTransferSyntax.Equals(compressSyntax))
				{
					theCodecFactory = codec;
					break;
				}

			if (theCodecFactory == null)
			{
				item.FailureDescription = String.Format("Unable to find codec for compression: {0}", compressSyntax.Name);
				Platform.Log(LogLevel.Error, "Error with work queue item {0}: {1}", item.GetKey(), item.FailureDescription);
				base.PostProcessingFailure(item, WorkQueueProcessorFailureType.Fatal);
				return;
			}

			if (!ProcessUidList(item, theCodecFactory))
				PostProcessingFailure(item, WorkQueueProcessorFailureType.NonFatal);
			else
			{
				Platform.Log(LogLevel.Info,
				             "Completed Compressing study {0} for Patient {1} (PatientId:{2} A#:{3}) on partition {4} to {5}",
				             Study.StudyInstanceUid, Study.PatientsName, Study.PatientId,
				             Study.AccessionNumber, ServerPartition.Description, compressSyntax.Name);


				if (compressSyntax.LossyCompressed)
					UpdateStudyStatus(StorageLocation, StudyStatusEnum.OnlineLossy, compressSyntax);
				else
					UpdateStudyStatus(StorageLocation, StudyStatusEnum.OnlineLossless, compressSyntax);

				PostProcessing(item,
				               WorkQueueProcessorStatus.Pending,
				               WorkQueueProcessorDatabaseUpdate.None); // batch processed, not complete
			}
		}

		protected override void OnProcessItemEnd(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");
            base.OnProcessItemEnd(item);
            
            _studyStats.UidsLoadTime.Add(UidsLoadTime);
			_studyStats.StorageLocationLoadTime.Add(StorageLocationLoadTime);
			_studyStats.StudyXmlLoadTime.Add(StudyXmlLoadTime);
			_studyStats.DBUpdateTime.Add(DBUpdateTime);

			if (_studyStats.NumInstances > 0)
			{
				_studyStats.CalculateAverage();
				StatisticsLogger.Log(LogLevel.Info, false, _studyStats);
			}
		}

		protected override void OnProcessItemBegin(Model.WorkQueue item)
		{
			Platform.CheckForNullReference(item, "item");

			_studyStats = new CompressStudyStatistics
			              	{
			              		Description = String.Format("{0}", item.WorkQueueTypeEnum)
			              	};
		}

        protected override bool CanStart()
        {
            
			IList<Model.WorkQueue> relatedItems = FindRelatedWorkQueueItems(WorkQueueItem, 
                new[]
			    {
			        WorkQueueTypeEnum.StudyProcess, 
                    WorkQueueTypeEnum.ReconcileStudy
                }, null);

        	TimeSpan delay;

			if (relatedItems == null || relatedItems.Count == 0)
			{
				// Don't compress lossy if the study needs to be reconciled.
                // It's time wasting if we go ahead with the compression because later on
                // users will have to restore the study in order to reconcile the images in SIQ.
                if (CompressTransferSyntax.LossyCompressed && StorageLocation.IsReconcileRequired)
                {
                    Platform.Log(LogLevel.Info, "Study {0} cannot be compressed to lossy at this time because of pending reconciliation. Reinserting into FilesystemQueue", StorageLocation.StudyInstanceUid);
                    delay = TimeSpan.FromMinutes(60);
                    ReinsertFilesystemQueue(delay);
                    PostProcessing(WorkQueueItem, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
                    return false;
                }
				return true;
			}
        	Platform.Log(LogLevel.Info, "Compression entry for study {0} has existing WorkQueue entry, reinserting into FilesystemQueue", StorageLocation.StudyInstanceUid);
        	delay = TimeSpan.FromMinutes(60);
        	ReinsertFilesystemQueue(delay);
        	PostProcessing(WorkQueueItem, WorkQueueProcessorStatus.Complete, WorkQueueProcessorDatabaseUpdate.ResetQueueState);
        	return false;
        }

	}
}