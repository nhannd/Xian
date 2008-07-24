using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.ProtocolAdmin
{
	[DataContract]
	public class ListProtocolCodesRequest : PagedDataContractBase
	{
		public ListProtocolCodesRequest(SearchResultPage page)
			:base(page)
		{
		}
	}
}
