using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class StartTranscriptionRequest : DataContractBase
	{
		public StartTranscriptionRequest(EntityRef transcriptionStepRef)
		{
			this.TranscriptionStepRef = transcriptionStepRef;
		}

		[DataMember]
		public EntityRef TranscriptionStepRef;
	}
}