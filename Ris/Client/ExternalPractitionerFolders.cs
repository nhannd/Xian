using System;
using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client
{
	[ExtensionOf(typeof(ExternalPractitionerFolderExtensionPoint))]
	[FolderPath("Unverfied")]
	[FolderDescription("ExternalPractitionerUnverifiedFolderDescription")]
	internal class UnverifiedFolder : ExternalPractitionerFolder
	{
		protected override void PrepareQueryRequest(ListExternalPractitionersRequest request)
		{
			request.VerifiedState = VerifiedState.NotVerified;
		}
	}

	[ExtensionOf(typeof(ExternalPractitionerFolderExtensionPoint))]
	[FolderPath("Verfied Today")]
	[FolderDescription("ExternalPractitionerVerifiedTodayFolderDescription")]
	internal class VerifiedTodayFolder : ExternalPractitionerFolder
	{
		protected override void PrepareQueryRequest(ListExternalPractitionersRequest request)
		{
			var today = DateTime.Now;
			request.VerifiedState = VerifiedState.Verified;
			request.LastVerifiedRangeFrom = today.Date;
			request.LastVerifiedRangeUntil = today.Date.AddDays(1);
		}
	}

	[FolderPath("Search Results")]
	public class ExternalPractitionerSearchFolder : SearchResultsFolder<ExternalPractitionerSummary, SearchParams>
	{
		public ExternalPractitionerSearchFolder()
			: base(new ExternalPractitionerWorkflowTable())
		{
		}

		protected override TextQueryResponse<ExternalPractitionerSummary> DoQuery(SearchParams query, int specificityThreshold)
		{
			TextQueryResponse<ExternalPractitionerSummary> response = null;

			Platform.GetService(
				delegate(IExternalPractitionerAdminService service)
				{
					var request = new TextQueryRequest {TextQuery = query.TextSearch, SpecificityThreshold = specificityThreshold};
					response = service.TextQuery(request);
				});

			return response;
		}
	}

}
