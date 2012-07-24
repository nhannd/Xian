#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;

namespace ClearCanvas.Ris.Application.Common.RegistrationWorkflow
{
	[DataContract]
	public class ProcedureRequisition : DataContractBase
	{
		/// <summary>
		/// Constructor for use by service to return requisition back to client for editing.
		/// </summary>
		public ProcedureRequisition(
			ProcedureTypeSummary procedureType,
			string procedureNumber,
			DateTime? scheduledTime,
			int scheduledDuration,
			EnumValueInfo schedulingCode,
			FacilitySummary performingFacility,
			DepartmentSummary performingDepartment,
			EnumValueInfo laterality,
			bool portableModality,
			bool checkedIn,
			EnumValueInfo status,
			bool canModify,
			bool cancelled)
		{
			this.ProcedureType = procedureType;
			this.ProcedureNumber = procedureNumber;
			this.ScheduledTime = scheduledTime;
			this.ScheduledDuration = scheduledDuration;
			this.SchedulingCode = schedulingCode;
			this.PerformingFacility = performingFacility;
			this.PerformingDepartment = performingDepartment;
			this.Laterality = laterality;
			this.PortableModality = portableModality;
			this.CheckedIn = checkedIn;
			this.Status = status;
			this.CanModify = canModify;
			this.Cancelled = cancelled;
		}

		/// <summary>
		/// Constructor for use by client when initially creating a requisition.
		/// </summary>
		public ProcedureRequisition(ProcedureTypeSummary procedureType, FacilitySummary facility)
		{
			this.ProcedureType = procedureType;
			this.PerformingFacility = facility;
			this.CanModify = true;  // can modify a new requisition
		}


		/// <summary>
		/// The procedure type. Required.
		/// </summary>
		[DataMember]
		public ProcedureTypeSummary ProcedureType;

		/// <summary>
		/// Procedure number.  Will be set by the server. Clients should not set or modify this field.
		/// </summary>
		[DataMember]
		public string ProcedureNumber;

		/// <summary>
		/// Time at which this procedure is scheduled to occur. May be null, indicating that
		/// the procedure is not yet scheduled for a specific time.
		/// </summary>
		[DataMember]
		public DateTime? ScheduledTime;

		/// <summary>
		/// The duration of the block of time which the procedure is expected to take, in minutes.
		/// </summary>
		[DataMember]
		public int ScheduledDuration;

		/// <summary>
		/// Indicates additional info about procedure scheduling via configurable codes.  Optional.
		/// </summary>
		[DataMember]
		public EnumValueInfo SchedulingCode;

		/// <summary>
		/// Status of this procedure, set by the server.
		/// </summary>
		[DataMember]
		public EnumValueInfo Status;

		/// <summary>
		/// Facility at which this procedure will be performed. Required.
		/// </summary>
		[DataMember]
		public FacilitySummary PerformingFacility;

		/// <summary>
		/// Department at which this procedure will be performed. Optional.
		/// </summary>
		[DataMember]
		public DepartmentSummary PerformingDepartment;

		/// <summary>
		/// Indicates whether this procedure is to be performed on a portable modality.
		/// </summary>
		[DataMember]
		public bool PortableModality;

		/// <summary>
		/// Laterality for this procedure.
		/// </summary>
		[DataMember]
		public EnumValueInfo Laterality;

		/// <summary>
		/// Set by the server to indicate whether this requested procedure can be modified
		/// during an order modification (e.g. it cannot be modified if it is already in-progress).
		/// </summary>
		[DataMember]
		public bool CanModify;

		/// <summary>
		/// Indicates if an existing procedure is checked in or not, and if a new procedure should be checked in upon creation.
		/// </summary>
		[DataMember]
		public bool CheckedIn;

		/// <summary>
		/// Set by the server if this procedure is cancelled, or by the client to indicate that the procedure should be cancelled.
		/// </summary>
		[DataMember]
		public bool Cancelled;
	}
}
