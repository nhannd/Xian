using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class SendbackResidentReportResponse : DataContractBase
	{
		public SendbackResidentReportResponse(ReportingWorklistItemSummary replacementInterpretationStep)
		{
			this.ReplacementInterpretationStep = replacementInterpretationStep;
		}

		[DataMember]
		public ReportingWorklistItemSummary ReplacementInterpretationStep;
	}
}
