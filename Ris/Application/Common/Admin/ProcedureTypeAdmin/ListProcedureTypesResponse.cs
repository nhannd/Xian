using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin
{
	[DataContract]
	public class ListProcedureTypesResponse : DataContractBase
	{
		public ListProcedureTypesResponse(List<ProcedureTypeSummary> ProcedureTypes)
		{
			this.ProcedureTypes = ProcedureTypes;
		}

		[DataMember]
		public List<ProcedureTypeSummary> ProcedureTypes;
	}
}
