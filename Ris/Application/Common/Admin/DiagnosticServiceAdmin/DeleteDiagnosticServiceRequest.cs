using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DiagnosticServiceAdmin
{
	[DataContract]
	public class DeleteDiagnosticServiceRequest : DataContractBase
	{
		public DeleteDiagnosticServiceRequest(EntityRef diagnosticServiceRef)
		{
			this.DiagnosticServiceRef = diagnosticServiceRef;
		}

		[DataMember]
		public EntityRef DiagnosticServiceRef;
	}
}
