#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Ris.Application.Common.Admin.ExternalPractitionerAdmin;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionOf(typeof(ExternalPractitionerFolderExtensionPoint))]
	[FolderPath("Unverfied")]
	[FolderDescription("ExternalPractitionerUnverifiedFolderDescription")]
	internal class UnverifiedFolder : ExternalPractitionerFolder
	{
		public UnverifiedFolder()
			: base(new ExternalPractitionerWorkflowTable { PropertyNameForTimeColumn = "LastEditedTime", SortAscending = true})
		{
		}

		protected override void PrepareQueryRequest(ListExternalPractitionersRequest request)
		{
			request.VerifiedState = VerifiedState.NotVerified;
			request.SortByLastEditedTime = true;
			request.SortAscending = true;
		}
	}

	[ExtensionOf(typeof(ExternalPractitionerFolderExtensionPoint))]
	[FolderPath("Verfied Today")]
	[FolderDescription("ExternalPractitionerVerifiedTodayFolderDescription")]
	internal class VerifiedTodayFolder : ExternalPractitionerFolder
	{
		public VerifiedTodayFolder()
			: base(new ExternalPractitionerWorkflowTable { PropertyNameForTimeColumn = "LastVerifiedTime", SortAscending = false})
		{
		}

		protected override void PrepareQueryRequest(ListExternalPractitionersRequest request)
		{
			var today = Platform.Time;
			request.VerifiedState = VerifiedState.Verified;
			request.LastVerifiedRangeFrom = today.Date;
			request.LastVerifiedRangeUntil = today.Date.AddDays(1);
			request.SortByLastVerifiedTime = true;
			request.SortAscending = false;
		}
	}

	[FolderPath("Search Results")]
	public class ExternalPractitionerSearchFolder : SearchResultsFolder<ExternalPractitionerSummary, ExternalPractitionerSearchParams>
	{
		public ExternalPractitionerSearchFolder()
			: base(new ExternalPractitionerWorkflowTable())
		{
		}

		protected override TextQueryResponse<ExternalPractitionerSummary> DoQuery(ExternalPractitionerSearchParams query, int specificityThreshold)
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
