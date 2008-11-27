using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class RejectTranscriptionRequest : SaveTranscriptionRequest
	{
		public RejectTranscriptionRequest(EntityRef reportingStepRef, string rejectReason)
			: this(reportingStepRef, null, rejectReason)
		{
		}

		public RejectTranscriptionRequest(EntityRef reportingStepRef, Dictionary<string, string> reportPartExtendedProperties, string rejectReason)
			: base(reportingStepRef, reportPartExtendedProperties)
		{
			this.RejectReason = rejectReason;
		}

		[DataMember]
		public string RejectReason;
	}
}