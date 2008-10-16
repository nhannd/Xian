using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Application.Common.ProtocollingWorkflow
{
	[DataContract]
	public class ReviseSubmittedProtocolResponse : DataContractBase
	{
		public ReviseSubmittedProtocolResponse(ReportingWorklistItem replacementStep)
		{
			this.ReplacementStep = replacementStep;
		}

		[DataMember]
		public ReportingWorklistItem ReplacementStep;
	}

	[DataContract]
	public class ReviseSubmittedProtocolResponse2 : DataContractBase
	{
		public ReviseSubmittedProtocolResponse2(ReportingWorklistItem replacementStep)
		{
			this.ReplacementStep = replacementStep;
		}

		[DataMember]
		public ReportingWorklistItem ReplacementStep;
	}
}