#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class CompleteTranscriptionRequest : SaveTranscriptionRequest
	{
		public CompleteTranscriptionRequest(EntityRef transcriptionStepRef)
			: base(transcriptionStepRef, null)
		{
		}

		public CompleteTranscriptionRequest(EntityRef reportingStepRef, Dictionary<string, string> reportPartExtendedProperties)
			: base(reportingStepRef, reportPartExtendedProperties)
		{
		}
	}
}