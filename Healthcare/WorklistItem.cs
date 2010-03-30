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

using System;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Workflow;
using System.Collections.Generic;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Basic worklist item class, that can be extended when required.
	/// </summary>
	public class WorklistItem
	{
		public delegate void UpdateWorklistItemDelegate(WorklistItem item, object value);

		private static readonly Dictionary<WorklistItemField, UpdateWorklistItemDelegate> _fieldUpdaters
			= new Dictionary<WorklistItemField, UpdateWorklistItemDelegate>();

		static WorklistItem()
		{
			//todo: I think we need to be more specific about the ps class here (eg subclass)
			_fieldUpdaters.Add(WorklistItemField.ProcedureStep,
				(item, value) => item.ProcedureStepRef = (EntityRef)value);

			_fieldUpdaters.Add(WorklistItemField.Procedure,
				(item, value) => item.ProcedureRef = (EntityRef)value);

			_fieldUpdaters.Add(WorklistItemField.Order,
				(item, value) => item.OrderRef = (EntityRef)value);

			_fieldUpdaters.Add(WorklistItemField.Patient,
				(item, value) => item.PatientRef = (EntityRef)value);

			_fieldUpdaters.Add(WorklistItemField.PatientProfile,
				(item, value) => item.PatientProfileRef = (EntityRef)value);



			_fieldUpdaters.Add(WorklistItemField.Mrn,
				(item, value) => item.Mrn = (PatientIdentifier) value);

			_fieldUpdaters.Add(WorklistItemField.PatientName,
				(item, value) => item.PatientName = (PersonName) value);

			_fieldUpdaters.Add(WorklistItemField.AccessionNumber,
				(item, value) => item.AccessionNumber = (string) value);

			_fieldUpdaters.Add(WorklistItemField.Priority,
				(item, value) => item.OrderPriority = (OrderPriority) value);

			_fieldUpdaters.Add(WorklistItemField.PatientClass,
				(item, value) => item.PatientClass = (PatientClassEnum) value);

			_fieldUpdaters.Add(WorklistItemField.DiagnosticServiceName,
				(item, value) => item.DiagnosticServiceName = (string) value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureTypeName,
				(item, value) => item.ProcedureName = (string) value);

			_fieldUpdaters.Add(WorklistItemField.ProcedurePortable,
				(item, value) => item.ProcedurePortable = (bool) value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureLaterality,
				(item, value) => item.ProcedureLaterality = (Laterality) value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureStepName,
				(item, value) => item.ProcedureStepName = (string)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureStepState,
				(item, value) => item.ActivityStatus = (ActivityStatus?)value);





			_fieldUpdaters.Add(WorklistItemField.OrderSchedulingRequestTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureScheduledStartTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureCheckInTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureCheckOutTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureStartTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureEndTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureStepCreationTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureStepScheduledStartTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureStepStartTime,
				(item, value) => item.Time = (DateTime?)value);

			_fieldUpdaters.Add(WorklistItemField.ProcedureStepEndTime,
				(item, value) => item.Time = (DateTime?)value);
		}



		/// <summary>
		/// Initialize this worklist item from the specified projection and data tuple.
		/// </summary>
		/// <param name="projection"></param>
		/// <param name="tuple"></param>
		public void InitializeFromTuple(WorklistItemProjection projection, object[] tuple)
		{
			for(var i = 0; i < projection.Fields.Count; i++)
			{
				GetFieldUpdater(projection.Fields[i])(this, tuple[i]);
			}
		}

		/// <summary>
		/// Initialize this worklist item from the specified procedure step and related entities.
		/// This method is not suitable for mapping a large set of procedure steps, because it is not efficient.
		/// </summary>
		/// <param name="step"></param>
		/// <param name="timeField"></param>
		public virtual void InitializeFromProcedureStep(ProcedureStep step, WorklistItemField timeField)
		{
			var rp = step.Procedure;
			var o = step.Procedure.Order;
			var v = step.Procedure.Order.Visit;
			var p = step.Procedure.Order.Patient;
			var pp = step.Procedure.Order.Patient.GetProfile(rp.PerformingFacility);

			this.ProcedureStepRef = step.GetRef();
			this.ProcedureRef = rp.GetRef();
			this.OrderRef = o.GetRef();
			this.PatientRef = p.GetRef();
			this.PatientProfileRef = pp.GetRef();
			this.Mrn = pp.Mrn;
			this.PatientName = pp.Name;
			this.AccessionNumber = o.AccessionNumber;
			this.OrderPriority = o.Priority;
			this.PatientClass = v.PatientClass;
			this.DiagnosticServiceName = o.DiagnosticService.Name;
			this.ProcedureName = rp.Type.Name;
			this.ProcedurePortable = rp.Portable;
			this.ProcedureLaterality = rp.Laterality;
			this.ProcedureStepName = step.Name;
			this.ActivityStatus = step.State;
			this.Time = GetTimeValue(step, timeField);
		}

		/// <summary>
		/// Default constructor required for dynamic instantiation.
		/// </summary>
		public WorklistItem()
		{
			
		}

		public EntityRef ProcedureStepRef { get; internal set; }

		public EntityRef ProcedureRef { get; internal set; }

		public EntityRef OrderRef { get; internal set; }

		public EntityRef PatientRef { get; internal set; }

		public EntityRef PatientProfileRef { get; internal set; }

		public PatientIdentifier Mrn { get; internal set; }

		public PersonName PatientName { get; internal set; }

		public string AccessionNumber { get; internal set; }

		public OrderPriority OrderPriority { get; internal set; }

		public PatientClassEnum PatientClass { get; internal set; }

		public string DiagnosticServiceName { get; internal set; }

		public string ProcedureName { get; internal set; }

		public bool ProcedurePortable { get; internal set; }

		public Laterality ProcedureLaterality { get; internal set; }

		public string ProcedureStepName { get; internal set; }

		public DateTime? Time { get; internal set; }

		public ActivityStatus? ActivityStatus { get; internal set; }


		protected virtual UpdateWorklistItemDelegate GetFieldUpdater(WorklistItemField field)
		{
			return _fieldUpdaters[field];
		}

		protected virtual DateTime? GetTimeValue(ProcedureStep step, WorklistItemField timeField)
		{
			if (timeField == WorklistItemField.OrderSchedulingRequestTime)
				return step.Procedure.Order.SchedulingRequestTime;

			if (timeField == WorklistItemField.ProcedureScheduledStartTime)
				return step.Procedure.ScheduledStartTime;

			if (timeField == WorklistItemField.ProcedureCheckInTime)
				return step.Procedure.ProcedureCheckIn.CheckInTime;

			if (timeField == WorklistItemField.ProcedureCheckOutTime)
				return step.Procedure.ProcedureCheckIn.CheckOutTime;

			if (timeField == WorklistItemField.ProcedureStartTime)
				return step.Procedure.StartTime;

			if (timeField == WorklistItemField.ProcedureEndTime)
				return step.Procedure.EndTime;

			if (timeField == WorklistItemField.ProcedureStepCreationTime)
				return step.CreationTime;

			if (timeField == WorklistItemField.ProcedureStepScheduledStartTime)
				return step.Scheduling == null ? step.Scheduling.StartTime : null;

			if (timeField == WorklistItemField.ProcedureStepStartTime)
				return step.StartTime;

			if (timeField == WorklistItemField.ProcedureStepEndTime)
				return step.EndTime;

			throw new WorkflowException("Invalid time field");
		}
	}
}
