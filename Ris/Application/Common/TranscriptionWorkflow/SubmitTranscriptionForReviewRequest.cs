using System.Collections.Generic;
using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class SubmitTranscriptionForReviewRequest : SaveTranscriptionRequest
	{
		public SubmitTranscriptionForReviewRequest(EntityRef transcriptionStepRef)
			: base(transcriptionStepRef, null)
		{
		}

		public SubmitTranscriptionForReviewRequest(EntityRef reportingStepRef, Dictionary<string, string> reportPartExtendedProperties, EntityRef supervisorRef)
			: base(reportingStepRef, reportPartExtendedProperties)
		{
			this.SupervisorRef = supervisorRef;
		}

		[DataMember]
		public EntityRef SupervisorRef;
	}
}