using System.Collections.Generic;
using System.Runtime.Serialization;
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

		[DataMember]
		public EntityRef TranscriptionStepRef;

		[DataMember]
		public Dictionary<string, string> ReportPartExtendedProperties;
	}
}