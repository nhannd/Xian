using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class StartTranscriptionReviewRequest : DataContractBase
	{
		public StartTranscriptionReviewRequest(EntityRef transcriptionReviewStepRef)
		{
			this.TranscriptionReviewStepRef = transcriptionReviewStepRef;
		}

		[DataMember]
		public EntityRef TranscriptionReviewStepRef;
	}
}