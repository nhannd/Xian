#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class CompleteDowntimeProcedureRequest : DataContractBase
	{
		public CompleteDowntimeProcedureRequest(
			EntityRef procedureRef,
			bool reportProvided,
			Dictionary<string, string> reportPartExtendedProperties,
			EntityRef interpreterRef,
			EntityRef transcriptionistRef)
		{
			ProcedureRef = procedureRef;
			ReportProvided = reportProvided;
			ReportPartExtendedProperties = reportPartExtendedProperties;
			TranscriptionistRef = transcriptionistRef;
			InterpreterRef = interpreterRef;
		}

		/// <summary>
		/// Reference to the procedure complete downtime for.
		/// </summary>
		[DataMember]
		public EntityRef ProcedureRef;

		/// <summary>
		/// Indicates whether a transcribed report has already been provided.
		/// </summary>
		[DataMember]
		public bool ReportProvided;

		/// <summary>
		/// Transcribed report data, if provided.
		/// </summary>
		[DataMember]
		public Dictionary<string, string> ReportPartExtendedProperties;

		/// <summary>
		/// Interpreter of the transcribed report, if provided.
		/// </summary>
		[DataMember]
		public EntityRef InterpreterRef;

		/// <summary>
		/// Transcriptionist that transcribed the report, if provided. Optional.
		/// </summary>
		[DataMember]
		public EntityRef TranscriptionistRef;

	}
}
