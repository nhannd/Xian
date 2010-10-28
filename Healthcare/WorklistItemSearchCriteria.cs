#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Enterprise.Core;
using System;

namespace ClearCanvas.Healthcare
{
	/// <summary>
	/// Criteria for finding worklist items.
	/// </summary>
	public class WorklistItemSearchCriteria : SearchCriteria
	{
		private static readonly Dictionary<WorklistItemField, Converter<WorklistItemSearchCriteria, ISearchCriteria>> _timeFieldSubCriteriaMappings
			= new Dictionary<WorklistItemField, Converter<WorklistItemSearchCriteria, ISearchCriteria>>();

		/// <summary>
		/// Class constructor.
		/// </summary>
		static WorklistItemSearchCriteria()
		{
			_timeFieldSubCriteriaMappings.Add(WorklistItemField.OrderSchedulingRequestTime,
				criteria => criteria.Order.SchedulingRequestTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureScheduledStartTime,
				criteria => criteria.Procedure.ScheduledStartTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureCheckInTime,
				criteria => criteria.ProcedureCheckIn.CheckInTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureCheckOutTime,
				criteria => criteria.ProcedureCheckIn.CheckOutTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureStartTime,
				criteria => criteria.Procedure.StartTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureEndTime,
				criteria => criteria.Procedure.EndTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureStepCreationTime,
				criteria => criteria.ProcedureStep.CreationTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureStepScheduledStartTime,
				criteria => criteria.ProcedureStep.Scheduling.StartTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureStepStartTime,
				criteria => criteria.ProcedureStep.StartTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ProcedureStepEndTime,
				criteria => criteria.ProcedureStep.EndTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ReportPartPreliminaryTime,
				criteria => ((ReportingWorklistItemSearchCriteria)criteria).ReportPart.PreliminaryTime);

			_timeFieldSubCriteriaMappings.Add(WorklistItemField.ReportPartCompletedTime,
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
			return _timeFieldSubCriteriaMappings[timeField](this);
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
