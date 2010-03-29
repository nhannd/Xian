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
using ClearCanvas.Enterprise.Core.Modelling;
using ClearCanvas.Healthcare.Brokers;
using ClearCanvas.Workflow;
using ClearCanvas.Healthcare.Workflow.Reporting;
using System.Collections.Generic;

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

		protected ReportingWorklist()
		{
			_interpretedByStaffFilter = new WorklistStaffFilter();
			_transcribedByStaffFilter = new WorklistStaffFilter();
			_verifiedByStaffFilter = new WorklistStaffFilter();
			_supervisedByStaffFilter = new WorklistStaffFilter();
		}

		#region Public Properties

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

		#endregion

		public override IList GetWorklistItems(IWorklistQueryContext wqc)
		{
			return (IList)wqc.GetBroker<IReportingWorklistItemBroker>().GetWorklistItems<ReportingWorklistItem>(this, wqc);
		}

		public override string GetWorklistItemsHql(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IReportingWorklistItemBroker>().GetWorklistItemsHql(this, wqc);
		}

		public override int GetWorklistItemCount(IWorklistQueryContext wqc)
		{
			return wqc.GetBroker<IReportingWorklistItemBroker>().CountWorklistItems(this, wqc);
		}

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			return WorklistItemProjection.GetReportingProjection(timeField);
		}

		protected override WorklistItemSearchCriteria[] GetFilterCriteriaCore(IWorklistQueryContext wqc)
		{
			var criteria = new ReportingWorklistItemSearchCriteria();

			// apply base filters
			ApplyFilterCriteria(criteria, wqc);

			// add reporting-specific filters
			if (GetSupportsReportingStaffRoleFilter(GetClass()))
			{
				this.InterpretedByStaffFilter.Apply(criteria.ReportPart.Interpreter, wqc);
				this.TranscribedByStaffFilter.Apply(criteria.ReportPart.Transcriber, wqc);
				this.VerifiedByStaffFilter.Apply(criteria.ReportPart.Verifier, wqc);
				this.SupervisedByStaffFilter.Apply(criteria.ReportPart.Supervisor, wqc);
			}

			return new[] { criteria };
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
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			return new [] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(InterpretationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepScheduledStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
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
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.EqualTo(ActivityStatus.SC);
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			criteria.ProcedureStep.Scheduling.StartTime.IsNotNull();
			return new[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(InterpretationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepScheduledStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
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
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });

			criteria.ReportPart.Interpreter.NotEqualTo(wqc.Staff);
			criteria.ProcedureStep.Scheduling.Performer.Staff.IsNull();
			criteria.ReportPart.Supervisor.IsNull();

			return new [] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(VerificationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
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
			var assignedToMe = BaseCriteria();
			assignedToMe.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);

			var bySupervisor = BaseCriteria();
			bySupervisor.ReportPart.Supervisor.EqualTo(wqc.Staff);

			return new[] { assignedToMe, bySupervisor };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(VerificationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}

		private ReportingWorklistItemSearchCriteria BaseCriteria()
		{
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
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
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(InterpretationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
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
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ReportPart.Interpreter.EqualTo(wqc.Staff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(TranscriptionStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
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
			var criteria = new ReportingWorklistItemSearchCriteria();
			criteria.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteria.ProcedureStep.Scheduling.Performer.Staff.EqualTo(wqc.Staff);
			return new WorklistItemSearchCriteria[] { criteria };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(TranscriptionReviewStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
		}

		protected override WorklistItemProjection GetProjectionCore(WorklistItemField timeField)
		{
			// add the "HasErrors" flag for this worklist
			return base.GetProjectionCore(timeField).AddFields(new[] { WorklistItemField.ReportPartHasErrors });
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
			var criteriaNotEqual = new ReportingWorklistItemSearchCriteria();
			criteriaNotEqual.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNotEqual.ReportPart.Interpreter.EqualTo(wqc.Staff);
			criteriaNotEqual.ProcedureStep.Scheduling.Performer.Staff.NotEqualTo(wqc.Staff);

			var criteriaNull = new ReportingWorklistItemSearchCriteria();
			criteriaNull.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.IP });
			criteriaNull.ReportPart.Interpreter.EqualTo(wqc.Staff);
			criteriaNull.ProcedureStep.Scheduling.Performer.Staff.IsNull();

			return new WorklistItemSearchCriteria[] { criteriaNotEqual, criteriaNull };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(VerificationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepStartTime,
				null,
				WorklistOrdering.PrioritizeOldestItems);
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
			var unsupervised = new ReportingWorklistItemSearchCriteria();
			unsupervised.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.CM });
			unsupervised.ReportPart.Verifier.EqualTo(wqc.Staff);

			var supervised = new ReportingWorklistItemSearchCriteria();
			supervised.ProcedureStep.State.In(new[] { ActivityStatus.SC, ActivityStatus.CM });
			supervised.ReportPart.Verifier.NotEqualTo(wqc.Staff);
			supervised.ReportPart.Interpreter.EqualTo(wqc.Staff);

			return new WorklistItemSearchCriteria[] { unsupervised, supervised };
		}

		public override Type[] GetProcedureStepSubclasses()
		{
			return new[] { typeof(PublicationStep) };
		}

		protected override TimeDirective GetTimeDirective()
		{
			return new TimeDirective(
				WorklistItemField.ProcedureStepCreationTime,
				WorklistTimeRange.Today,
				WorklistOrdering.PrioritizeNewestItems);
		}
	}
}
