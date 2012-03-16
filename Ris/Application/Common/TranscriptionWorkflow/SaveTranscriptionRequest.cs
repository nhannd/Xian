#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Common.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class SaveTranscriptionRequest : DataContractBase
	{
		public SaveTranscriptionRequest(EntityRef transcriptionStepRef, Dictionary<string, string> reportPartExtendedProperties)
		{
			this.TranscriptionStepRef = transcriptionStepRef;
			this.ReportPartExtendedProperties = reportPartExtendedProperties;
		}

		public SaveTranscriptionRequest(EntityRef transcriptionStepRef, Dictionary<string, string> reportPartExtendedProperties, EntityRef supervisorRef)
			: this(transcriptionStepRef, reportPartExtendedProperties)
		{
			this.SupervisorRef = supervisorRef;
		}

		[DataMember]
		public EntityRef TranscriptionStepRef;

		[DataMember]
		public Dictionary<string, string> ReportPartExtendedProperties;

		[DataMember]
		public EntityRef SupervisorRef;
	}
}