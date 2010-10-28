#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public class Performing
	{
		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingScheduledWorklist)]
		[FolderPath("Scheduled")]
		[FolderDescription("PerformingScheduledFolderDescription")]
		public class ScheduledFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingCheckedInWorklist)]
		[FolderPath("Checked In", true)]
		[FolderDescription("PerformingCheckedInFolderDescription")]
		public class CheckedInFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingInProgressWorklist)]
		[FolderPath("In Progress")]
		[FolderDescription("PerformingInProgressFolderDescription")]
		public class InProgressFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingPerformedWorklist)]
		[FolderPath("Performed")]
		[FolderDescription("PerformingPerformedFolderDescription")]
		public class PerformedFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingUndocumentedWorklist)]
		[FolderPath("Incomplete Documentation")]
		[FolderDescription("PerformingUndocumentedFolderDescription")]
		public class UndocumentedFolder : PerformingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(PerformingWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.PerformingCancelledWorklist)]
		[FolderPath("Cancelled")]
		[FolderDescription("PerformingCancelledFolderDescription")]
		public class CancelledFolder : PerformingWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class SearchFolder : WorklistSearchResultsFolder<ModalityWorklistItemSummary, IModalityWorkflowService>
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
