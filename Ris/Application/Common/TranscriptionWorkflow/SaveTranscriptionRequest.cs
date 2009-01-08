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

		public SaveTranscriptionRequest(EntityRef transcriptionStepRef, Dictionary<string, string> reportPartExtendedProperties, EntityRef supervisorRef)
			: this(transcriptionStepRef, reportPartExtendedProperties)
		{
			this.SupervisorRef = supervisorRef;
		}

		[DataMember]
		public EntityRef TranscriptionStepRef;

		[DataMember]
		public Dictionary<string, string> ReportPartExtendedProperties;

		[DataMember]
		public EntityRef SupervisorRef;
	}
}