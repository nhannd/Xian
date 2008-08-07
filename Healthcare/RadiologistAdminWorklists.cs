using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Abstract base class for protocoling worklists.
	/// </summary>
	[WorklistCategory("WorklistCategoryRadiologistAdmin")]
	public abstract class RadiologistAdminWorklist : ReportingWorklist
	{
	}

	/// <summary>
	/// ReportingAdminAssignedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ReportingAdminAssignedWorklistDescription")]
	public class ReportingAdminAssignedWorklist : RadiologistAdminWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ReportingProcedureStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP, ActivityStatus.SU });
			criteria.ProcedureStep.Performer.Staff.IsNotNull();
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("ProtocollingAdminAssignedWorklist")]
	public class ProtocollingAdminAssignedWorklist : RadiologistAdminWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(ProtocolAssignmentStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP, ActivityStatus.SU });
			criteria.ProcedureStep.Performer.Staff.IsNotNull();
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}
}
