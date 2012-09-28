#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;

namespace ClearCanvas.Healthcare.Extended
{
	/// <summary>
	/// EmergencyScheduledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyScheduledWorklistDescription")]
	public class EmergencyScheduledWorklist : RegistrationWorklist
	{
		protected override WorklistItemSearchCriteria[] GetInvariantCriteriaCore(IWorklistQueryContext wqc)
		{
			// this is slightly different than the registration scheduled worklist, because we include
			// 'checked in' items here, rather than having a separate 'checked in' worklist
			var criteria = new RegistrationWorklistItemSearchCriteria();
			criteria.Procedure.Status.EqualTo(ProcedureStatus.SC);
			//criteria.Order.Status.EqualTo(OrderStatus.SC);
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
	/// EmergencyInProgressWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyInProgressWorklistDescription")]
	public class EmergencyInProgressWorklist : RegistrationInProgressWorklist
	{
	}

	/// <summary>
	/// EmergencyPerformedWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyPerformedWorklistDescription")]
	public class EmergencyPerformedWorklist : RegistrationPerformedWorklist
	{
	}

	/// <summary>
	/// EmergencyCancelledWorklist entity
	/// </summary>
	[ExtensionOf(typeof(WorklistExtensionPoint))]
	[WorklistCategory("WorklistCategoryEmergency")]
	[WorklistClassDescription("EmergencyCancelledWorklistDescription")]
	public class EmergencyCancelledWorklist : RegistrationCancelledWorklist
	{
	}
}
