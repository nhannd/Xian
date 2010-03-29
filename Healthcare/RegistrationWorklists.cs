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
using ClearCanvas.Common;
using ClearCanvas.Healthcare.Brokers;

namespace ClearCanvas.Healthcare
{
	[WorklistProcedureTypeGroupClass(typeof(PerformingGroup))]
	[WorklistCategory("WorklistCategoryRegistration")]
	public abstract class RegistrationWorklist : Worklist
	{
		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			return (IList)wqc.GetBroker<IRegistrationWorklistItemBroker>().GetWorklistItems<WorklistItem>(this, wqc);
		}

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IRegistrationWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IRegistrationWorklistItemBroker>().CountWorklistItems(this, wqc);
		}

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			return WorklistItemProjection.GetProcedureStepProjection(timeField);
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(RegistrationProcedureStep) };
		}
	}

	/// <summary>
	/// RegistrationToBeScheduledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistCategory("WorklistCategoryBooking")]
	[WorklistClassDescription("RegistrationToBeScheduledWorklistDescription")]
	public class RegistrationToBeScheduledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);

			// only unscheduled items should appear in this list
			criteria.Procedure.ScheduledStartTime.IsNull();

			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.OrderSchedulingRequestTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// RegistrationScheduledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("RegistrationScheduledWorklistDescription")]
	public class RegistrationScheduledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);
			criteria.ProcedureCheckIn.CheckInTime.IsNull();     // exclude anything already checked-in
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureScheduledStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// RegistrationCheckedInWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("RegistrationCheckedInWorklistDescription")]
	public class RegistrationCheckedInWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);
			criteria.ProcedureCheckIn.CheckInTime.IsNotNull();  // include only items that have been checked-in
			criteria.ProcedureCheckIn.CheckOutTime.IsNull();
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
	/// RegistrationInProgessWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("RegistrationInProgressWorklistDescription")]
	public class RegistrationInProgressWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.IP);
			criteria.ProcedureCheckIn.CheckOutTime.IsNull();    // exclude any item already checked-out
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStartTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeOldestItems);
		}
	}

	/// <summary>
	/// RegistrationPerformedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("RegistrationPerformedWorklistDescription")]
	public class RegistrationPerformedWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			// "completed" in this context just means the patient has checked-out
			// the order may still be in progress
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.In(new[] { ProcedureStatus.IP, ProcedureStatus.CM });
			criteria.ProcedureCheckIn.CheckOutTime.IsNotNull();
			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureCheckOutTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}

	/// <summary>
	/// RegistrationCancelledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[WorklistClassDescription("RegistrationCancelledWorklistDescription")]
	public class RegistrationCancelledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.In(new[] { ProcedureStatus.DC, ProcedureStatus.CA });

			return new WorklistItemSearchCriteria[] { criteria };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureEndTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}
}