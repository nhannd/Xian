using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client
{
	public abstract class ExternalPractitionerFolder : WorkflowFolder<ExternalPractitionerSummary>
	{
		protected ExternalPractitionerFolder(Table<ExternalPractitionerSummary> table)
			: base(table)
		{
		}

		protected override QueryItemsResult QueryItems()
		{
			QueryItemsResult result = null;
			Platform.GetService(
				delegate(IExternalPractitionerAdminService service)
				{
					var request = new ListExternalPractitionersRequest { QueryItems = true, QueryCount = true };
					PrepareQueryRequest(request);
					var response = service.ListExternalPractitioners(request);
					result = new QueryItemsResult(response.Practitioners, response.ItemCount);
				});

			return result;
		}

		protected override int QueryCount()
		{
			var count = -1;
			Platform.GetService(
				delegate(IExternalPractitionerAdminService service)
				{
					var request = new ListExternalPractitionersRequest { QueryItems = false, QueryCount = true };
					PrepareQueryRequest(request);
					var response = service.ListExternalPractitioners(request);
					count = response.ItemCount;
				});

			return count;
		}

		protected abstract void PrepareQueryRequest(ListExternalPractitionersRequest request);
	}
}