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
    public class ReportingToBeReportedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof (InterpretationStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.OldestItemsFirst);
            return new ReportingWorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistSingleton(true)]
    public class ReportingDraftWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof (InterpretationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.OldestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistSingleton(true)]
    public class ReportingInTranscriptionWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(TranscriptionStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.OldestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistSingleton(true)]
    public class ReportingToBeVerifiedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(VerificationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            if (wqc.Staff.Type == StaffType.PRAR)
            {
                criteria.ReportPart.Interpreter.EqualTo(wqc.Staff);
            }
            else
            {
                criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
                criteria.ReportPart.Interpreter.EqualTo(wqc.Staff);
                criteria.ReportPart.Supervisor.IsNull();
            }
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.OldestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
    [WorklistSingleton(true)]
    public class ReportingVerifiedWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(PublicationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
            if (wqc.Staff.Type == StaffType.PRAR)
            {
                criteria.ReportPart.Interpreter.EqualTo(wqc.Staff);
            }
            else
            {
                criteria.ReportPart.Verifier.EqualTo(wqc.Staff);
            }
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.NewestItemsFirst);
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
    }

    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(false)]
    [WorklistSingleton(true)]
    public class ReportingReviewResidentReportWorklist : ReportingWorklist
    {
        public override WorklistItemSearchCriteria[] GetInvariantCriteria(IWorklistQueryContext wqc)
        {
            ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(VerificationStep);
            criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
            criteria.ReportPart.Supervisor.EqualTo(wqc.Staff);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.OldestItemsFirst);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
