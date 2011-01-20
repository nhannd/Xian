#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Core;
using ClearCanvas.ImageServer.Core.Validation;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudyPostProcessing
{
	/// <summary>
	/// ReconcilePostProcessing processor
	/// </summary>
	/// <remarks>
	/// This functionality was replaced in the 2.0 (Server - Performance milestone) release,
	/// and will be removed in a future relase.
	/// </remarks>
	[Obsolete("This functionality is now integrated in with the Reconcile processor")]
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    class ReconcilePostProcessingProcessor : StudyProcessItemProcessor
	{
        protected override InstancePreProcessingResult PreProcessFile(Model.WorkQueueUid uid, DicomFile file)
        {
            // Return a result indicating the file has been reconciled.
            InstancePreProcessingResult result = new InstancePreProcessingResult {AutoReconciled = true};

        	return result;
        }

		protected override void ProcessItem(Model.WorkQueue item)
		{
			LoadUids(item);

			if (WorkQueueUidList.Count == 0)
			{
				// Delete the queue entry.
				PostProcessing(item,
							   WorkQueueProcessorStatus.Complete,
							   WorkQueueProcessorDatabaseUpdate.ResetQueueState);
			}
			else
				base.ProcessItem(item);
		}

		protected override void ProcessFile(Model.WorkQueueUid queueUid, DicomFile file, ClearCanvas.Dicom.Utilities.Xml.StudyXml stream, bool compare)
		{
            Platform.CheckFalse(compare, "compare");

			SopInstanceProcessor processor = new SopInstanceProcessor(Context);

			FileInfo fileInfo = new FileInfo(file.Filename);
			long fileSize = fileInfo.Length;
			processor.InstanceStats.FileSize = (ulong)fileSize;
			string sopInstanceUid = file.DataSet[DicomTags.SopInstanceUid].GetString(0, "File:" + fileInfo.Name);
			processor.InstanceStats.Description = sopInstanceUid;

			if (Study != null)
			{
				StudyComparer comparer = new StudyComparer();
				DifferenceCollection list = comparer.Compare(file, Study, ServerPartition.GetComparisonOptions());
				if (list != null && list.Count > 0)
				{
					Platform.Log(LogLevel.Warn, "Dicom file contains information inconsistent with the study in the system");
				}
			}

		    string groupID = ServerHelper.GetUidGroup(file, StorageLocation.ServerPartition, WorkQueueItem.InsertTime);
			processor.ProcessFile(groupID, file, stream, false, false, null, null);

			Statistics.StudyInstanceUid = StorageLocation.StudyInstanceUid;
			if (String.IsNullOrEmpty(processor.Modality) == false)
				Statistics.Modality = processor.Modality;

			// Update the statistics
			Statistics.NumInstances++;
		}
	}
}
