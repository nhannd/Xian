using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin
{
	[DataContract]
	public class DeleteProcedureTypeRequest : DataContractBase
	{
		public DeleteProcedureTypeRequest(EntityRef preocedureTypeRef)
		{
			this.ProcedureTypeRef = preocedureTypeRef;
		}

		[DataMember]
		public EntityRef ProcedureTypeRef;
	}
}
