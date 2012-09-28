#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Folders
{
	public abstract class RadiologistAdmin
	{
		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminUnreportedWorklist)]
		[FolderPath("Unreported Items", true)]
		[FolderDescription("ReportingAdminUnreportedFolderDescription")]
		public class ReportingAdminUnreportedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminAssignedWorklist)]
		[FolderPath("Active Reporting Items", true)]
		[FolderDescription("ReportingAdminAssignedFolderDescription")]
		public class ReportingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminToBeTranscribedWorklist)]
		[FolderPath("To Be Transcribed Items", true)]
		[FolderDescription("ReportingAdminToBeTranscribedWorklistDescription")]
		public class ReportingAdminToBeTranscribedWorklist : ReportingWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class RadiologistAdminSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItemSummary, IReportingWorkflowService>
		{
			public RadiologistAdminSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			protected override string ProcedureStepClassName
			{
				//TODO: having the client specify the class name isn't a terribly good idea, but
				//it is the only way to get things working right now
				//This class uses two different ProcedureStepClassNames for query.  So this property is actually not used.
				get { return "ReportingProcedureStep and ProtocolAssignmentStep"; }
			}

			protected override TextQueryResponse<ReportingWorklistItemSummary> DoQuery(WorklistSearchParams query, int specificityThreshold)
			{
				TextQueryResponse<ReportingWorklistItemSummary> response;

				WorklistItemTextQueryOptions options = WorklistItemTextQueryOptions.ProcedureStepStaff
					| (DowntimeRecovery.InDowntimeRecoveryMode ? WorklistItemTextQueryOptions.DowntimeRecovery : 0);

				response = DoQueryCore(query, specificityThreshold, options, "ReportingProcedureStep");
				if (response.TooManyMatches)
					return response;

				List<ReportingWorklistItemSummary> storeMatches = new List<ReportingWorklistItemSummary>(response.Matches);
				response = DoQueryCore(query, specificityThreshold, options, "ProtocolAssignmentStep");

				if (!response.TooManyMatches)
					response.Matches.AddRange(storeMatches);

				return response;
			}
		}
	}
}
