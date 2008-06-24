using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin
{
	[DataContract]
	public class DeleteProcedureTypeGroupRequest : DataContractBase
	{
		public DeleteProcedureTypeGroupRequest(EntityRef procedureTypeGroupRef)
		{
			this.ProcedureTypeGroupRef = procedureTypeGroupRef;
		}

		[DataMember]
		public EntityRef ProcedureTypeGroupRef;
	}
}
