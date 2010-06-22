#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System;
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
			return (IList)wqc.GetBroker<IModalityWorklistItemBroker>().GetWorklistItems<WorklistItem>(this, wqc);
        }

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IModalityWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

        public override int GetWorklistItemCount(IWorklistQueryContext wqc)
        {
            return wqc.GetBroker<IModalityWorklistItemBroker>().CountWorklistItems(this, wqc);
        }

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			return WorklistItemProjection.GetDefaultProjection(timeField);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] {typeof (ModalityProcedureStep)};
		}
    }
	
    /// <summary>
	/// PerformingScheduledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingScheduledWorklistDescription")]
	public class PerformingScheduledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureCheckIn.CheckInTime.IsNull(); // not checked in
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepScheduledStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

    /// <summary>
	/// PerformingWorklistCheckedInWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingWorklistCheckedInWorklistDescription")]
	public class PerformingCheckedInWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureCheckIn.CheckInTime.IsNotNull(); // checked-in
            criteria.ProcedureCheckIn.CheckOutTime.IsNull(); // but not checked-out
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);    // and not started
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureCheckInTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

    /// <summary>
	/// PerformingWorklistInProgessWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingWorklistInProgressWorklistDescription")]
	public class PerformingInProgressWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

    /// <summary>
	/// PerformingWorklistPerformedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingWorklistPerformingWorklistDescription")]
	public class PerformingPerformedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.CM);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepEndTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}

    /// <summary>
	/// PerformingCancelledWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingCancelledWorklistDescription")]
	public class PerformingCancelledWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.DC);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepEndTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}

    /// <summary>
	/// PerformingUndocumentedWorklist entity
    /// </summary>
    [ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistClassDescription("PerformingUndocumentedWorklistDescription")]
	public class PerformingUndocumentedWorklist : PerformingWorklist
    {
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
        {
            ModalityWorklistItemSearchCriteria criteria = new ModalityWorklistItemSearchCriteria();
            criteria.ProcedureStep.State.EqualTo(ActivityStatus.IP);
            return new WorklistItemSearchCriteria[] { criteria };
        }

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new [] { typeof(DocumentationProcedureStep) };
		}
	}
}
