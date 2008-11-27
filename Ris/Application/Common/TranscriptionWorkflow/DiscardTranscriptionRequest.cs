using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class DiscardTranscriptionRequest : DataContractBase
	{
		public DiscardTranscriptionRequest(EntityRef transcriptionStepRef)
		{
			TranscriptionStepRef = transcriptionStepRef;
		}

		[DataMember]
		public EntityRef TranscriptionStepRef;
	}
}