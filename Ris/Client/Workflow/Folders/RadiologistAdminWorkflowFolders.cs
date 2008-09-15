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
		public class RadiologistAdminSearchFolder : SearchResultsFolder<ReportingWorklistItem>
		{
			public RadiologistAdminSearchFolder()
				: base(new ReportingWorklistTable())
			{
			}

			protected override TextQueryResponse<ReportingWorklistItem> DoQuery(string query, int specificityThreshold)
			{
				TextQueryResponse<ReportingWorklistItem> response;

				//TODO: having the client specify the class name isn't a terribly good idea, but
				//it is the only way to get things working right now
				response = DoQuery(query, specificityThreshold, "ReportingProcedureStep");
				if (response.TooManyMatches)
					return response;

				List<ReportingWorklistItem> storeMatches = new List<ReportingWorklistItem>(response.Matches);
				response = DoQuery(query, specificityThreshold, "ProtocolAssignmentStep");

				if (!response.TooManyMatches)
					response.Matches.AddRange(storeMatches);

				return response;
			}

			private static TextQueryResponse<ReportingWorklistItem> DoQuery(string query, int specificityThreshold, string procedureStepClassName)
			{
				TextQueryResponse<ReportingWorklistItem> response = null;
				Platform.GetService<IReportingWorkflowService>(
					delegate(IReportingWorkflowService service)
					{
						WorklistItemTextQueryOptions options = WorklistItemTextQueryOptions.ProcedureStepStaff
							| (DowntimeRecovery.InDowntimeRecoveryMode ? WorklistItemTextQueryOptions.DowntimeRecovery : 0);

						response = service.SearchWorklists(
							new WorklistItemTextQueryRequest(
								query, specificityThreshold, procedureStepClassName, options));
					});

				return response;
			}
		}
	}
}
