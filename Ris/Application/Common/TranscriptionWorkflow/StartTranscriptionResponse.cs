using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class StartTranscriptionResponse : DataContractBase
	{
		public StartTranscriptionResponse(EntityRef transcriptionStepRef)
		{
			TranscriptionStepRef = transcriptionStepRef;
		}

		[DataMember]
		public EntityRef TranscriptionStepRef;
	}
}