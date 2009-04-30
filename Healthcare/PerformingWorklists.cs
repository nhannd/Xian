#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System.Collections;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Common;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
    [WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
	[WorklistCategory("WorklistCategoryPerforming")]
	public abstract class PerformingWorklist : Worklist
    {
        public override IList GetWorklistItems(IWorklistQueryContext wqc)
        {
            return (IList)wqc.GetBroker<IModalityWorklistItemBroker>().GetWorklistItems(this, wqc);
        }

        public override int GetWorklistItemCount(IWorklistQueryContext wqc)
        {
            return wqc.GetBroker<IModalityWorklistItemBroker>().CountWorklistItems(this, wqc);
        }
    }
	
    /// <summary>
	/// PerformingScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingScheduledWorklistDescription")]
	public class PerformingScheduledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureCheckIn.CheckInTime.IsNull(); // not checked in
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingWorklistCheckedInWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingWorklistCheckedInWorklistDescription")]
	public class PerformingCheckedInWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureCheckIn.CheckInTime.IsNotNull(); // checked-in
            criteria.ProcedureCheckIn.CheckOutTime.IsNull(); // but not checked-out
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);    // and not started
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureCheckInTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingWorklistInProgessWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingWorklistInProgressWorklistDescription")]
	public class PerformingInProgressWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingWorklistPerformedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingWorklistPerformingWorklistDescription")]
	public class PerformingPerformedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingCancelledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingCancelledWorklistDescription")]
	public class PerformingCancelledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(ModalityProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepEndTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }

    /// <summary>
	/// PerformingUndocumentedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
    [WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("PerformingUndocumentedWorklistDescription")]
	public class PerformingUndocumentedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStepClass = typeof(DocumentationProcedureStep);
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
            return new WorklistItemSearchCriteria[] { criteria };
        }
    }
}
