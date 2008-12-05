using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class StartTranscriptionReviewResponse : DataContractBase
	{
		public StartTranscriptionReviewResponse(EntityRef transcriptionReviewStepRef)
		{
			this.TranscriptionReviewStepRef = transcriptionReviewStepRef;
		}

		[DataMember]
		public EntityRef TranscriptionReviewStepRef;
	}
}