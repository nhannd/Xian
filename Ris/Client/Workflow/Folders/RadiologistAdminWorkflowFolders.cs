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
		public class ReportingAdminUnreportedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ReportingAdminAssignedWorklist)]
		[FolderPath("Active Reporting Items", true)]
		public class ReportingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[ExtensionOf(typeof(RadiologistAdminWorkflowFolderExtensionPoint))]
		[FolderForWorklistClass(WorklistClassNames.ProtocollingAdminAssignedWorklist)]
		[FolderPath("Active Protocolling Items", true)]
		public class ProtocollingAdminAssignedFolder : ReportingWorkflowFolder
		{
		}

		[FolderPath("Search Results")]
		public class RadiologistAdminSearchFolder : WorklistSearchResultsFolder<ReportingWorklistItem, IReportingWorkflowService>
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

			protected override TextQueryResponse<ReportingWorklistItem> DoQuery(SearchParams query, int specificityThreshold)
			{
				TextQueryResponse<ReportingWorklistItem> response;

				WorklistItemTextQueryOptions options = WorklistItemTextQueryOptions.ProcedureStepStaff
					| (DowntimeRecovery.InDowntimeRecoveryMode ? WorklistItemTextQueryOptions.DowntimeRecovery : 0);

				response = DoQueryCore(query, specificityThreshold, options, "ReportingProcedureStep");
				if (response.TooManyMatches)
					return response;

				List<ReportingWorklistItem> storeMatches = new List<ReportingWorklistItem>(response.Matches);
				response = DoQueryCore(query, specificityThreshold, options, "ProtocolAssignmentStep");

				if (!response.TooManyMatches)
					response.Matches.AddRange(storeMatches);

				return response;
			}
		}
	}
}
