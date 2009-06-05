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
using ClearCanvas.Common;
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;

namespace ClearCanvas.Healthcare
{
	[WorklistProcedureTypeGroupClass(typeof(ReadingGroup))]
	[WorklistCategory("WorklistCategoryReporting")]
	public abstract class ReportingWorklist : Worklist
	{
		private WorklistStaffFilter _interpretedByStaffFilter;
		private WorklistStaffFilter _transcribedByStaffFilter;
		private WorklistStaffFilter _verifiedByStaffFilter;
		private WorklistStaffFilter _supervisedByStaffFilter;

		public ReportingWorklist()
		{
			_interpretedByStaffFilter = new WorklistStaffFilter();
			_transcribedByStaffFilter = new WorklistStaffFilter();
			_verifiedByStaffFilter = new WorklistStaffFilter();
			_supervisedByStaffFilter = new WorklistStaffFilter();
		}

		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			return (IList)wqc.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IReportingWorklistItemBroker>().CountWorklistItems(this, wqc);
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Interpreted By staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
        public virtual WorklistStaffFilter InterpretedByStaffFilter
		{
			get { return _interpretedByStaffFilter; }
            protected set { _interpretedByStaffFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Transcribed By staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
        public virtual WorklistStaffFilter TranscribedByStaffFilter
		{
			get { return _transcribedByStaffFilter; }
            protected set { _transcribedByStaffFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Verified By staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
        public virtual WorklistStaffFilter VerifiedByStaffFilter
		{
			get { return _verifiedByStaffFilter; }
            protected set { _verifiedByStaffFilter = value; }
		}

		/// <summary>
		/// Gets or sets the <see cref="WorklistStaffFilter"/> for Supervisor staff.
		/// </summary>
		[PersistentProperty]
		[EmbeddedValue]
        public virtual WorklistStaffFilter SupervisedByStaffFilter
		{
			get { return _supervisedByStaffFilter; }
            protected set { _supervisedByStaffFilter = value; }
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
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
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
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepScheduledStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new ReportingWorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingToBeReviewedReportWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[WorklistClassDescription("ReportingToBeReviewedReportWorklistDescription")]
	public class ReportingToBeReviewedReportWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(VerificationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });

			criteria.ReportPart.Interpreter.NotEqualTo(wqc.Staff);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ReportPart.Supervisor.IsNull();

			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	/// <summary>
	/// ReportingAssignedReviewWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAssignedReviewWorklistDescription")]
	public class ReportingAssignedReviewWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria assignedToMe = BaseCriteria();
			assignedToMe.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(assignedToMe, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);

			ReportingWorklistItemSearchCriteria bySupervisor = BaseCriteria();
			bySupervisor.ReportPart.Supervisor.EqualTo(wqc.Staff);
			ApplyTimeCriteria(bySupervisor, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);

			return new ReportingWorklistItemSearchCriteria[] { assignedToMe, bySupervisor };
		}

		private ReportingWorklistItemSearchCriteria BaseCriteria()
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(VerificationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			return criteria;
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingDraftWorklistDescription")]
	public class ReportingDraftWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(InterpretationStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingInTranscriptionWorklistDescription")]
	public class ReportingInTranscriptionWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ReportPart.Interpreter.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingReviewTranscriptionWorklistDescription")]
	public class ReportingReviewTranscriptionWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStepClass = typeof(TranscriptionReviewStep);
			criteria.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			ApplyTimeCriteria(criteria, WorklistTimeField.ProcedureStepCreationTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);
			return new WorklistItemSearchCriteria[] { criteria };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(false)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingAwaitingReviewWorklistDescription")]
	public class ReportingAwaitingReviewWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria criteriaNotEqual = new ReportingWorklistItemSearchCriteria();
			criteriaNotEqual.ProcedureStepClass = typeof(VerificationStep);
			criteriaNotEqual.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNotEqual.ReportPart.Interpreter.EqualTo(wqc.Staff);
			criteriaNotEqual.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.Staff);
			ApplyTimeCriteria(criteriaNotEqual, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);

			ReportingWorklistItemSearchCriteria criteriaNull = new ReportingWorklistItemSearchCriteria();
			criteriaNull.ProcedureStepClass = typeof(VerificationStep);
			criteriaNull.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNull.ReportPart.Interpreter.EqualTo(wqc.Staff);
			criteriaNull.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			ApplyTimeCriteria(criteriaNull, WorklistTimeField.ProcedureStepStartTime, null, WorklistOrdering.PrioritizeOldestItems, wqc);

			return new WorklistItemSearchCriteria[] { criteriaNotEqual, criteriaNull };
		}
	}

	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistSupportsTimeFilter(true)]
	[StaticWorklist(true)]
	[WorklistClassDescription("ReportingVerifiedWorklistDescription")]
	public class ReportingVerifiedWorklist : ReportingWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			ReportingWorklistItemSearchCriteria unsupervised = new ReportingWorklistItemSearchCriteria();
			unsupervised.ProcedureStepClass = typeof(PublicationStep);
			unsupervised.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
			unsupervised.ReportPart.Verifier.EqualTo(wqc.Staff);
			ApplyTimeCriteria(unsupervised, WorklistTimeField.ProcedureStepCreationTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);

			ReportingWorklistItemSearchCriteria supervised = new ReportingWorklistItemSearchCriteria();
			supervised.ProcedureStepClass = typeof(PublicationStep);
			supervised.ProcedureStep.State.In(new ActivityStatus[] { ActivityStatus.SC, ActivityStatus.CM });
			supervised.ReportPart.Verifier.NotEqualTo(wqc.Staff);
			supervised.ReportPart.Interpreter.EqualTo(wqc.Staff);
			ApplyTimeCriteria(supervised, WorklistTimeField.ProcedureStepCreationTime, WorklistTimeRange.Today, WorklistOrdering.PrioritizeNewestItems, wqc);

			return new WorklistItemSearchCriteria[] { unsupervised, supervised };
		}
	}
}
