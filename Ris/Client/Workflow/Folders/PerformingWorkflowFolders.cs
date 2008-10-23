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

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public abstract class Performing
	{
		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingScheduledWorklist)]
		[FolderPath("Scheduled")]
		public class ScheduledPerformingWorkflowFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingCheckedInWorklist)]
		[FolderPath("Checked In", true)]
		public class CheckedInPerformingWorkflowFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingInProgressWorklist)]
		[FolderPath("In Progress")]
		public class InProgressPerformingWorkflowFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingCompletedWorklist)]
		[FolderPath("Completed")]
		public class CompletedPerformingWorkflowFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingUndocumentedWorklist)]
		[FolderPath("Incomplete Documentation")]
		public class UndocumentedPerformingWorkflowFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingCancelledWorklist)]
		[FolderPath("Cancelled")]
		public class CancelledPerformingWorkflowFolder : PerformingWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class SearchFolder : WorklistSearchResultsFolder<ModalityWorklistItem, IModalityWorkflowService>
		{
			public SearchFolder()
				: base(new PerformingWorklistTable())
			{
			}

			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ModalityProcedureStep"; }
			}
		}
	}
}
