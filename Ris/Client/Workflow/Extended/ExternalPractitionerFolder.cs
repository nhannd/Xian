#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public abstract class ExternalPractitionerFolder : WorkflowFolder<ExternalPractitionerSummary>
	{
		protected ExternalPractitionerFolder(Table<ExternalPractitionerSummary> table)
			: base(table)
		{
		}

		protected override QueryItemsResult QueryItems(int firstRow, int maxRows)
		{
			QueryItemsResult result = null;
			Platform.GetService(
				delegate(IExternalPractitionerAdminService service)
				{
					var request = new ListExternalPractitionersRequest { QueryItems = true, QueryCount = true, Page = new SearchResultPage(firstRow, maxRows) };
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