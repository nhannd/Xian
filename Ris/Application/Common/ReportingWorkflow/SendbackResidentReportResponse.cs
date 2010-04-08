using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class SendbackResidentReportResponse : SaveReportResponse
	{
		public SendbackResidentReportResponse(EntityRef reportingStepRef)
			: base(reportingStepRef)
		{
		}
	}
}
