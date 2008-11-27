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