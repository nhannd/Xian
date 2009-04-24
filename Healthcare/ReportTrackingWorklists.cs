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
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.IP });
			criteria.Report.Status.In(new ReportStatus[] { ReportStatus.D, ReportStatus.P });
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
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM }); 
			criteria.Report.Status.EqualTo(ReportStatus.F);
			criteria.ReportPart.Index.EqualTo(0);
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
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
			criteria.Report.Status.EqualTo(ReportStatus.C);
			criteria.ReportPart.Index.MoreThan(0);
			ApplyTimeCriteria(criteria, WorklistTimeField.ReportPartCompletedTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };

		}
	}
}
