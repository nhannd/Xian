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
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Registration
	{
		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationScheduledWorklist)]
		[FolderPath("Scheduled", true)]
		public class ScheduledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCheckedInWorklist)]
		[FolderPath("Checked In")]
		public class CheckedInFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationInProgressWorklist)]
		[FolderPath("In Progress")]
		public class InProgressFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationPerformedWorklist)]
		[FolderPath("Performed")]
		public class PerformedFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCancelledWorklist)]
		[FolderPath("Cancelled")]
		public class CancelledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationToBeScheduledWorklist)]
		[FolderPath("To Be Scheduled")]
		public class ToBeScheduledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationPendingProtocolWorklist)]
		[FolderPath("Pending Protocol")]
		public class PendingProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCompletedProtocolWorklist)]
		[FolderPath("Completed Protocol", true)]
		public class CompletedProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationRejectedProtocolWorklist)]
		[FolderPath("Rejected Protocol")]
		public class RejectedProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class RegistrationSearchFolder : WorklistSearchResultsFolder<RegistrationWorklistItem, IRegistrationWorkflowService>
		{
			public RegistrationSearchFolder()
				: base(new RegistrationWorklistTable())
			{
			}

			protected override string ProcedureStepClassName
			{
				get { return null; }
			}
		}

		[FolderPath("Search Results")]
		public class BookingSearchFolder : WorklistSearchResultsFolder<RegistrationWorklistItem, IRegistrationWorkflowService>
		{
			public BookingSearchFolder()
				: base(new RegistrationWorklistTable())
			{
			}


			//TODO: (JR may 2008) having the client specify the class name isn't a terribly good idea, but
			//it is the only way to get things working right now
			protected override string ProcedureStepClassName
			{
				get { return "ProtocolResolutionStep"; }
			}

		}
	}
}
