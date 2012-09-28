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

namespace ClearCanvas.Ris.Client.Workflow.Extended.Folders
{
	public class Booking
	{
		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationToBeScheduledWorklist)]
		[FolderPath("To Be Scheduled")]
		[FolderDescription("BookingToBeScheduledFolderDescription")]
		public class ToBeScheduledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationPendingProtocolWorklist)]
		[FolderPath("Pending Protocol")]
		[FolderDescription("BookingPendingProtocolFolderDescription")]
		public class PendingProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationCompletedProtocolWorklist)]
		[FolderPath("Completed Protocol", true)]
		[FolderDescription("BookingCompletedProtocolFolderDescription")]
		public class CompletedProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(Application.Extended.Common.WorklistClassNames.RegistrationRejectedProtocolWorklist)]
		[FolderPath("Rejected Protocol")]
		[FolderDescription("BookingRejectedProtocolFolderDescription")]
		public class RejectedProtocolFolder : RegistrationWorkflowFolder
		{
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
