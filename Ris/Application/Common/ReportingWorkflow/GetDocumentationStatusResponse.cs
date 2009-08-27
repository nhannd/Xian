using System.Runtime.Serialization;
using ClearCanvas.Enterprise.Common;

namespace ClearCanvas.Ris.Application.Common.ReportingWorkflow
{
	[DataContract]
	public class GetDocumentationStatusResponse : DataContractBase
	{
		public GetDocumentationStatusResponse(bool isIncomplete, string reason)
		{
			this.IsIncomplete = isIncomplete;
			this.Reason = reason;
		}

		[DataMember]
		public bool IsIncomplete;

		[DataMember]
		public string Reason;
	}
}