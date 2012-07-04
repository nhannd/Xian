#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class BookingWorkflowFolderExtensionPoint : ExtensionPoint<IWorklistFolder>
	{
	}

	[ExtensionPoint]
	public class BookingWorkflowItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class BookingWorkflowFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = ClearCanvas.Ris.Application.Common.AuthorityTokens.FolderSystems.Booking)]
	public class BookingWorkflowFolderSystem
		: RegistrationWorkflowFolderSystemBase<BookingWorkflowFolderExtensionPoint,
			BookingWorkflowFolderToolExtensionPoint, BookingWorkflowItemToolExtensionPoint>
	{
		public BookingWorkflowFolderSystem()
			: base(SR.TitleBookingFolderSystem)
		{
		}

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<RegistrationWorklistItemSummary> items)
		{
			return WebResourcesSettings.Default.BookingFolderSystemUrl;
		}

        protected override SearchResultsFolder CreateSearchResultsFolder()
        {
            return new Workflow.Folders.Registration.BookingSearchFolder();
        }

	}
}