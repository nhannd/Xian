using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	[WorklistCategory("WorklistCategoryReportingTracking")]
	public abstract class ReportingTrackingWorklist : ReportingWorklist
	{
		public override bool SupportsStaffRoleFilters
		{
			get { return true; }
		}
	}

	/// <summary>
	/// ReportingTrackingReportDraftWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingReportDraftWorklistDescription")]
	public class ReportingTrackingReportDraftWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ReportingProcedureStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ReportPart.Report.Status.In(new ReportStatus[] { ReportStatus.D, ReportStatus.P });
			ApplyTimeCriteria(criteria, WorklistTimeField.ReportPartPreliminaryTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingTrackingReportFinalWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingReportFinalWorklistDescription")]
	public class ReportingTrackingReportFinalWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(PublicationStep);
			criteria.ReportPart.Report.Status.EqualTo(ReportStatus.F);
			ApplyTimeCriteria(criteria, WorklistTimeField.ReportPartCompletedTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingTrackingReportCorrectedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingTrackingReportCorrectedWorklistDescription")]
	public class ReportingTrackingReportCorrectedWorklist : ReportingTrackingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(PublicationStep);
			criteria.ReportPart.Report.Status.EqualTo(ReportStatus.C);
			ApplyTimeCriteria(criteria, WorklistTimeField.ReportPartCompletedTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };

		}
	}
}
