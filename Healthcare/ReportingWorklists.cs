using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Enterprise.Core;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Workflow.Reporting;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
    [WorklistCategory("WorklistCategoryReporting")]
    public abstract class ReportingWorklist : Worklist
    {
        public override IList GetWorklistItems(IWorklistQueryContext wqc)
        {
            return (IList)wqc.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems(this, wqc);
        }

        public override int GetWorklistItemCount(IWorklistQueryContext wqc)
        {
            return wqc.GetBroker<IReportingWorklistItemBroker>().CountWorklistItems(this, wqc);
        }
    }

    /// <summary>
    /// ReportingToBeReportedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistClassDescription("ReportingToBeReportedWorklistDescription")]
    public class ReportingToBeReportedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof (InterpretationStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
            criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new ReportingWorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
    /// ReportingAssignedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingAssignedWorklistDescription")]
    public class ReportingAssignedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(InterpretationStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
            criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new ReportingWorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingDraftWorklistDescription")]
    public class ReportingDraftWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof (InterpretationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.IP });
            criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingInTranscriptionWorklistDescription")]
    public class ReportingInTranscriptionWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(TranscriptionStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingRadiologistToBeVerifiedWorklistDescription")]
    public class ReportingRadiologistToBeVerifiedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(VerificationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });

			// things that I'm assigned to verify
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);

            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingResdientToBeVerifiedWorklistDescription")]
    public class ReportingResidentToBeVerifiedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(VerificationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });

			// things that were interpreted by me, even though I'm not assigned to verify them
            criteria.ReportPart.Interpreter.EqualTo(wqc.Staff);

            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    public abstract class ReportingVerifiedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(PublicationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
            GetStaffSearchCriteria(criteria).EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeNewestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }

        public override IList GetWorklistItems(IWorklistQueryContext wqc)
        {
            IList results = base.GetWorklistItems(wqc);

            // Addendum appears as a separate item - should only be one item
            // It was decided to filter the result collection instead of rewriting the query
            // Filter out duplicate WorklistItems that have the same procedure and keep the newest item
            Dictionary<EntityRef, WorklistItem> filter = new Dictionary<EntityRef, WorklistItem>();
            foreach (WorklistItem item in results)
            {
                if (!filter.ContainsKey(item.ProcedureRef))
                {
                    filter.Add(item.ProcedureRef, item);
                }
                else
                {
                    WorklistItem existingItem = filter[item.ProcedureRef];
                    if (item.Time > existingItem.Time)
                        filter[item.ProcedureRef] = item;
                }
            }

            return new List<WorklistItem>(filter.Values);
        }

        protected abstract StaffSearchCriteria GetStaffSearchCriteria(ReportingWorklistItemSearchCriteria criteria);
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingRadiologistVerifiedWorklistDescription")]
    public class ReportingRadiologistVerifiedWorklist : ReportingVerifiedWorklist
    {
        protected override StaffSearchCriteria GetStaffSearchCriteria(ReportingWorklistItemSearchCriteria criteria)
        {
            return criteria.ReportPart.Verifier;
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingResidentVerifiedWorklistDescription")]
    public class ReportingResidentVerifiedWorklist : ReportingVerifiedWorklist
    {
        protected override StaffSearchCriteria GetStaffSearchCriteria(ReportingWorklistItemSearchCriteria criteria)
        {
            return criteria.ReportPart.Interpreter;
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [StaticWorklist(true)]
    [WorklistClassDescription("ReportingReviewResidentReportWorklistDescription")]
    public class ReportingReviewResidentReportWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(VerificationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ReportPart.Supervisor.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
