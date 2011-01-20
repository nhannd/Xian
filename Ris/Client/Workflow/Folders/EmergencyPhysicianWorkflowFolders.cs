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

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Emergency
	{
		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyScheduledWorklist)]
		[FolderPath("Scheduled", true)]
		[FolderDescription("EmergencyScheduledFolderDescription")]
		public class ScheduledFolder : EmergencyWorkflowFolder
		{
		}

		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyInProgressWorklist)]
		[FolderPath("In Progress", true)]
		[FolderDescription("EmergencyInProgressFolderDescription")]
		public class InProgressFolder : EmergencyWorkflowFolder
		{
		}

		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyPerformedWorklist)]
		[FolderPath("Performed", true)]
		[FolderDescription("EmergencyPerformedFolderDescription")]
		public class PerformedFolder : EmergencyWorkflowFolder
		{
		}

		[ExtensionOf(typeof(EmergencyWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.EmergencyCancelledWorklist)]
		[FolderPath("Cancelled", true)]
		[FolderDescription("EmergencyCancelledFolderDescription")]
		public class CancelledFolder : EmergencyWorkflowFolder
		{
		}
	}
}