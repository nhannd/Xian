using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProcedureTypeAdmin
{
	[DataContract]
	public class ListProcedureTypesRequest : PagedDataContractBase
	{
		public ListProcedureTypesRequest()
		{
		}

		public ListProcedureTypesRequest(SearchResultPage page)
			: base(page)
		{
		}

		[DataMember]
		public string Id;

		[DataMember]
		public string Name;
	}
}
		