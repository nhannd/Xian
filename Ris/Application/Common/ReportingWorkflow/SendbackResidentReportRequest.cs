using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class SendbackResidentReportRequest : DataContractBase
	{
		public SendbackResidentReportRequest(EntityRef stepRef)
		{
			this.VerificationStepRef = stepRef;
		}

		[DataMember]
		public EntityRef VerificationStepRef;
	}
}
