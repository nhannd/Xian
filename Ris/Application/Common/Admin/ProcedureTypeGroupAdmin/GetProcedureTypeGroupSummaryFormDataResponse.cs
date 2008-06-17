using System.Collections.Generic;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeGroupAdmin
{
	[DataContract]
	public class GetProcedureTypeGroupSummaryFormDataResponse : DataContractBase
	{
		public GetProcedureTypeGroupSummaryFormDataResponse(List<EnumValueInfo> categoryChoices)
		{
			this.CategoryChoices = categoryChoices;
		}

		[DataMember]
		public List<EnumValueInfo> CategoryChoices;
	}
}
