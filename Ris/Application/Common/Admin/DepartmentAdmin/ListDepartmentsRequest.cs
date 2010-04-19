using ClearCanvas.Enterprise.Common;
using System.Runtime.Serialization;

namespace ClearCanvas.Ris.Application.Common.Admin.DepartmentAdmin
{
	[DataContract]
	public class ListDepartmentsRequest : ListRequestBase
	{
		public ListDepartmentsRequest()
		{
		}

		public ListDepartmentsRequest(SearchResultPage page)
			: base(page)
		{
		}
	}
}
