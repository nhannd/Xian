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

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using System;

namespace ClearCanvas.Healthcare
{
	public class WorklistItemSearchCriteria : SearchCriteria
	{
		private static readonly Dictionary<WorklistItemField, Converter<WorklistItemSearchCriteria, ISearchCriteria>> _fieldMappings
			= new Dictionary<WorklistItemField, Converter<WorklistItemSearchCriteria, ISearchCriteria>>();

		static WorklistItemSearchCriteria()
		{
			_fieldMappings.Add(WorklistItemField.OrderSchedulingRequestTime,
				criteria => criteria.Order.SchedulingRequestTime);

			_fieldMappings.Add(WorklistItemField.ProcedureScheduledStartTime,
				criteria => criteria.Procedure.ScheduledStartTime);

			_fieldMappings.Add(WorklistItemField.ProcedureCheckInTime,
				criteria => criteria.ProcedureCheckIn.CheckInTime);

			_fieldMappings.Add(WorklistItemField.ProcedureCheckOutTime,
				criteria => criteria.ProcedureCheckIn.CheckOutTime);

			_fieldMappings.Add(WorklistItemField.ProcedureStartTime,
				criteria => criteria.Procedure.StartTime);

			_fieldMappings.Add(WorklistItemField.ProcedureEndTime,
				criteria => criteria.Procedure.EndTime);

			_fieldMappings.Add(WorklistItemField.ProcedureStepCreationTime,
				criteria => criteria.ProcedureStep.CreationTime);

			_fieldMappings.Add(WorklistItemField.ProcedureStepScheduledStartTime,
				criteria => criteria.ProcedureStep.Scheduling.StartTime);

			_fieldMappings.Add(WorklistItemField.ProcedureStepStartTime,
				criteria => criteria.ProcedureStep.StartTime);

			_fieldMappings.Add(WorklistItemField.ProcedureStepEndTime,
				criteria => criteria.ProcedureStep.EndTime);

			_fieldMappings.Add(WorklistItemField.ReportPartPreliminaryTime,
				criteria => ((ReportingWorklistItemSearchCriteria)criteria).ReportPart.PreliminaryTime);

			_fieldMappings.Add(WorklistItemField.ReportPartCompletedTime,
				criteria => ((ReportingWorklistItemSearchCriteria)criteria).ReportPart.CompletedTime);
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		public WorklistItemSearchCriteria()
		{
		}

		/// <summary>
		/// Copy constructor.
		/// </summary>
		/// <param name="other"></param>
		protected WorklistItemSearchCriteria(WorklistItemSearchCriteria other)
			: base(other)
		{
		}

		///<summary>
		///Creates a new object that is a copy of the current instance.
		///</summary>
		///
		///<returns>
		///A new object that is a copy of this instance.
		///</returns>
		///<filterpriority>2</filterpriority>
		public override object Clone()
		{
			return new WorklistItemSearchCriteria(this);
		}

		/// <summary>
		/// Clones this instance, but retains only criteria related to the Patient or Patient Profile,
		/// discarding criteria related to the procedure, order, report, etc.
		/// </summary>
		/// <returns></returns>
		public object ClonePatientCriteriaOnly()
		{
			return this.Clone(subCriteria => subCriteria.GetKey() == "PatientProfile", false);
		}

		/// <summary>
		/// Gets the sub-criteria object associated with the specified time field.
		/// </summary>
		/// <param name="timeField"></param>
		/// <returns></returns>
		public ISearchCriteria GetTimeFieldSubCriteria(WorklistItemField timeField)
		{
			return _fieldMappings[timeField](this);
		}

		public PatientProfileSearchCriteria PatientProfile
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("PatientProfile"))
				{
					this.SubCriteria["PatientProfile"] = new PatientProfileSearchCriteria("PatientProfile");
				}
				return (PatientProfileSearchCriteria)this.SubCriteria["PatientProfile"];
			}
		}

		public VisitSearchCriteria Visit
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("Visit"))
				{
					this.SubCriteria["Visit"] = new VisitSearchCriteria("Visit");
				}
				return (VisitSearchCriteria)this.SubCriteria["Visit"];
			}
		}

		public OrderSearchCriteria Order
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("Order"))
				{
					this.SubCriteria["Order"] = new OrderSearchCriteria("Order");
				}
				return (OrderSearchCriteria)this.SubCriteria["Order"];
			}
		}

		public ProcedureSearchCriteria Procedure
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("Procedure"))
				{
					this.SubCriteria["Procedure"] = new ProcedureSearchCriteria("Procedure");
				}
				return (ProcedureSearchCriteria)this.SubCriteria["Procedure"];
			}
		}

		public ProcedureCheckInSearchCriteria ProcedureCheckIn
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("ProcedureCheckIn"))
				{
					this.SubCriteria["ProcedureCheckIn"] = new ProcedureCheckInSearchCriteria("ProcedureCheckIn");
				}
				return (ProcedureCheckInSearchCriteria)this.SubCriteria["ProcedureCheckIn"];
			}
		}

		public ProcedureStepSearchCriteria ProcedureStep
		{
			get
			{
				if (!this.SubCriteria.ContainsKey("ProcedureStep"))
				{
					this.SubCriteria["ProcedureStep"] = new ProcedureStepSearchCriteria("ProcedureStep");
				}
				return (ProcedureStepSearchCriteria)this.SubCriteria["ProcedureStep"];
			}
		}
	}
}
