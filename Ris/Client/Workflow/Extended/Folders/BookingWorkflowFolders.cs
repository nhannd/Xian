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

namespace ClearCanvas.Ris.Client.Workflow.Extended.Folders
{
	public class Booking
	{
		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationToBeScheduledWorklist)]
		[FolderPath("To Be Scheduled")]
		[FolderDescription("BookingToBeScheduledFolderDescription")]
		public class ToBeScheduledFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationPendingProtocolWorklist)]
		[FolderPath("Pending Protocol")]
		[FolderDescription("BookingPendingProtocolFolderDescription")]
		public class PendingProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationCompletedProtocolWorklist)]
		[FolderPath("Completed Protocol", true)]
		[FolderDescription("BookingCompletedProtocolFolderDescription")]
		public class CompletedProtocolFolder : RegistrationWorkflowFolder
		{
		}

		[ExtensionOf(typeof(BookingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.RegistrationRejectedProtocolWorklist)]
		[FolderPath("Rejected Protocol")]
		[FolderDescription("BookingRejectedProtocolFolderDescription")]
		public class RejectedProtocolFolder : RegistrationWorkflowFolder
		{
		}
	}
}
