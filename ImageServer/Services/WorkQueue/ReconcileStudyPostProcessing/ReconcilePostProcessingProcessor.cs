#region License

// Copyright (c) 2006-2009, ClearCanvas Inc.
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

using ClearCanvas.Common;
using ClearCanvas.Dicom;
using ClearCanvas.ImageServer.Common.Helpers;
using ClearCanvas.ImageServer.Services.WorkQueue.StudyProcess;

namespace ClearCanvas.ImageServer.Services.WorkQueue.ReconcileStudyPostProcessing
{
	class ReconcilePostProcessingProcessor : StudyProcessItemProcessor
	{
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

		protected override void ProcessFile(Model.WorkQueueUid queueUid, string path, ClearCanvas.Dicom.Utilities.Xml.StudyXml stream)
		{
			DicomFile file = LoadDicomFile(path);
			if (Study != null)
			{
                DifferenceCollection list = StudyHelper.Compare(file, Study, ServerPartition);
				if (list != null && list.Count > 0)
				{
					Platform.Log(LogLevel.Warn, "Dicom file contains information inconsistent with the study in the system");
				}

			}

			InsertInstance(file, stream, queueUid);
		}
	}
}
