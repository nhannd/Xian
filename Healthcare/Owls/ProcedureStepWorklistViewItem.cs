#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;

namespace ClearCanvas.Healthcare.Owls
{
	using UpdateViewItemDelegate = UpdateViewItemDelegate<WorklistViewItemBase>;


    /// <summary>
    /// WorklistViewItem entity
    /// </summary>
	public partial class ProcedureStepWorklistViewItem
	{
		private static readonly Dictionary<WorklistItemField, UpdateViewItemDelegate> _fieldMappings
			= new Dictionary<WorklistItemField, UpdateViewItemDelegate>();

		static ProcedureStepWorklistViewItem()
		{
			_fieldMappings.Add(WorklistItemField.ProcedureStep,
				(item, value, updateReferences) => ((ProcedureStepWorklistViewItem)item).SetProcedureStepInfo((ProcedureStep)value, updateReferences));

			_fieldMappings.Add(WorklistItemField.ProcedureCheckIn,
				(item, value, updateReferences) => ((ProcedureStepWorklistViewItem)item).SetCheckInInfo((ProcedureCheckIn)value));
		}

		/// <summary>
		/// Sets the procedure check-in component of this view item.
		/// </summary>
		/// <param name="checkIn"></param>
		public virtual void SetCheckInInfo(ProcedureCheckIn checkIn)
		{
			_procedureCheckIn = new WorklistViewItemProcedureCheckInInfo(
				checkIn,
				checkIn.Version,
				checkIn.CheckInTime,
				checkIn.CheckOutTime);
		}

		public virtual void SetProcedureStepInfo(ProcedureStep step, bool updateReferences)
		{
			_procedureStep = new WorklistViewItemProcedureStepInfo(
				step,
				step.Version,
				step.GetClass().Name,
				step.Name,
				step.State,
				step.CreationTime,
				new WorklistViewItemProcedureStepSchedulingInfo(
					step.Scheduling == null ? null : step.Scheduling.StartTime,
					step.AssignedStaff == null ? null : new WorklistViewItemProcedureStepPerformerInfo(step.AssignedStaff) ),
					step.PerformingStaff == null ? null : new WorklistViewItemProcedureStepPerformerInfo(step.PerformingStaff), 
				step.StartTime,
				step.EndTime,
				step.Is<TranscriptionReviewStep>() ? step.As<TranscriptionReviewStep>().HasErrors : false);

			if(updateReferences)
			{
				SetProcedureInfo(step.Procedure, true);
			}
		}

		protected override UpdateViewItemDelegate GetFieldUpdater(WorklistItemField field)
		{
			UpdateViewItemDelegate updater;
			return _fieldMappings.TryGetValue(field, out updater) ? updater : base.GetFieldUpdater(field);
		}

	
		/// <summary>
		/// This method is called from the constructor.  Use this method to implement any custom
		/// object initialization.
		/// </summary>
		private void CustomInitialize()
		{
		}
	}
}