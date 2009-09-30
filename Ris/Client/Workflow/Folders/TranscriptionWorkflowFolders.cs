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

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Transcription
	{
		[ExtensionOf(typeof(TranscriptionWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.TranscriptionToBeTranscribedWorklist)]
		[FolderPath("To Be Transcribed", true)]
		public class ToBeTranscribedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionDraftWorklist)]
		[FolderPath("My Items/Draft")]
		public class DraftFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionToBeReviewedWorklist)]
		[FolderPath("My Items/To Be Reviewed")]
		public class ToBeReviewedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionAwaitingReviewWorklist)]
		[FolderPath("My Items/Awaiting Review")]
		public class AwaitingReviewFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderForWorklistClass(WorklistClassNames.TranscriptionCompletedWorklist)]
		[FolderPath("My Items/Completed")]
		public class CompletedFolder : TranscriptionWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class TranscriptionSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItem, ITranscriptionWorkflowService>
		{
			public TranscriptionSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ReportingProcedureStep"; }
			}

		}
	}
}