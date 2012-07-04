#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

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
		[FolderDescription("RegistrationScheduledFolderDescription")]
		public class ScheduledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCheckedInWorklist)]
		[FolderPath("Checked In")]
		[FolderDescription("RegistrationCheckedInFolderDescription")]
		public class CheckedInFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationInProgressWorklist)]
		[FolderPath("In Progress")]
		[FolderDescription("RegistrationInProgressFolderDescription")]
		public class InProgressFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationPerformedWorklist)]
		[FolderPath("Performed")]
		[FolderDescription("RegistrationPerformedFolderDescription")]
		public class PerformedFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RegistrationWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCancelledWorklist)]
		[FolderPath("Cancelled")]
		[FolderDescription("RegistrationCancelledFolderDescription")]
		public class CancelledFolder : RegistrationWorkflowFolder
		{
		}


		[FolderPath("Search Results")]
		public class RegistrationSearchFolder : WorklistSearchResultsFolder<RegistrationWorklistItemSummary, IRegistrationWorkflowService>
		{
			public RegistrationSearchFolder()
				: base(new RegistrationWorklistTable())
			{
			}

			protected override string ProcedureStepClassName
			{
				get { return "RegistrationProcedureStep"; }
			}
		}

		[FolderPath("Search Results")]
		public class BookingSearchFolder : WorklistSearchResultsFolder<RegistrationWorklistItemSummary, IRegistrationWorkflowService>
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
