using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.TranscriptionWorkflow
{
	[DataContract]
	public class LoadTranscriptionForEditRequest : DataContractBase
	{
		public LoadTranscriptionForEditRequest(EntityRef reportingStepRef)
		{
			ReportingStepRef = reportingStepRef;
		}

		[DataMember]
		public EntityRef ReportingStepRef;
	}
}