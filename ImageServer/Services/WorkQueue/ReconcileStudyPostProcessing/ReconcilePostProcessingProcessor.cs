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
	/// This functionality was replaced in the 1.6 (Server - Performance milestone) release,
	/// and will be removed in a future relase.
	/// </remarks>
	[Obsolete("This functionality is now integrated in with the Reconcile processor")]
    [StudyIntegrityValidation(ValidationTypes = StudyIntegrityValidationModes.Default, Recovery = RecoveryModes.Automatic)]
    class ReconcilePostProcessingProcessor : StudyProcessItemProcessor
	{
        protected override PreProcessingResult PreProcessFile(Model.WorkQueueUid uid, DicomFile file)
        {
            // Return a result indicating the file has been reconciled.
            PreProcessingResult result = new PreProcessingResult();
            result.AutoReconciled = true;

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

			SopInstanceProcessor processor = new SopInstanceProcessor(_context);

			long fileSize;
			FileInfo fileInfo = new FileInfo(file.Filename);
			fileSize = fileInfo.Length;
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
            processor.ProcessFile(groupID, file, stream, false, null, null);

			_statistics.StudyInstanceUid = StorageLocation.StudyInstanceUid;
			if (String.IsNullOrEmpty(processor.Modality) == false)
				_statistics.Modality = processor.Modality;

			// Update the statistics
			_statistics.NumInstances++;
		}
	}
}
