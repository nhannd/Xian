using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.CannedTextService
{
	[DataContract]
	public class ListCannedTextRequest : ListRequestBase
	{
		public ListCannedTextRequest()
		{
		}

		public ListCannedTextRequest(SearchResultPage page)
			:base(page)
		{
		}
	}
}
