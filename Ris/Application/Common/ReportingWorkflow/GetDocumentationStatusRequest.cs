using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class GetDocumentationStatusRequest : DataContractBase
	{
		public GetDocumentationStatusRequest(EntityRef procedureRef)
		{
			this.ProcedureRef = procedureRef;
		}

		[DataMember]
		public EntityRef ProcedureRef;
	}
}