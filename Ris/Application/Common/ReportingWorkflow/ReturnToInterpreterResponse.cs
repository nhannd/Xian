using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class ReturnToInterpreterResponse : SaveReportResponse
	{
		public ReturnToInterpreterResponse(EntityRef reportingStepRef)
			: base(reportingStepRef)
		{
		}
	}
}
