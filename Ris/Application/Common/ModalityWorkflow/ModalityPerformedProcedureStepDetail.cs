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
using ClearCanvas.Enterprise.Common;
using System.Collections.Generic;

namespace ClearCanvas.Ris.Application.Common.ModalityWorkflow
{
	[DataContract]
	public class ModalityPerformedProcedureStepDetail : DataContractBase
	{
		public ModalityPerformedProcedureStepDetail(EntityRef modalityPerformendProcedureStepRef, EnumValueInfo state, DateTime startTime, DateTime? endTime, StaffSummary performer, List<ModalityProcedureStepSummary> modalityProcedureSteps, List<DicomSeriesDetail> dicomSeries, Dictionary<string, string> extendedProperties)
		{
			this.ModalityPerformendProcedureStepRef = modalityPerformendProcedureStepRef;
			this.State = state;
			this.StartTime = startTime;
			this.EndTime = endTime;
			this.Performer = performer;
			this.ModalityProcedureSteps = modalityProcedureSteps;
			this.DicomSeries = dicomSeries;
			this.ExtendedProperties = extendedProperties;
		}

		[DataMember]
		public EntityRef ModalityPerformendProcedureStepRef;

		[DataMember]
		public EnumValueInfo State;

		[DataMember]
		public DateTime StartTime;

		[DataMember]
		public DateTime? EndTime;

		[DataMember]
		public StaffSummary Performer;

		/// <summary>
		/// Modality procedure steps that were performed with this performed procedure step.
		/// </summary>
		[DataMember]
		public List<ModalityProcedureStepSummary> ModalityProcedureSteps;

		[DataMember]
		public List<DicomSeriesDetail> DicomSeries;

		[DataMember]
		public Dictionary<string, string> ExtendedProperties;
	}
}