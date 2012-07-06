#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Security.Permissions;
using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	[ExtensionPoint]
	public class ExternalPractitionerFolderExtensionPoint : ExtensionPoint<IFolder>
	{
	}

	[ExtensionPoint]
	public class ExternalPractitionerItemToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	[ExtensionPoint]
	public class ExternalPractitionerFolderToolExtensionPoint : ExtensionPoint<ITool>
	{
	}

	public interface IExternalPractitionerItemToolContext : IWorkflowItemToolContext<ExternalPractitionerSummary>
	{
	}

	public interface IExternalPractitionerFolderToolContext : IWorkflowFolderToolContext
	{
	}

	public class ExternalPractitionerSearchParams : SearchParams
	{
		public ExternalPractitionerSearchParams(string textSearch)
			: base(textSearch)
		{
		}
	}

	[ExtensionOf(typeof(FolderSystemExtensionPoint))]
	[PrincipalPermission(SecurityAction.Demand, Role = Application.Extended.Common.AuthorityTokens.FolderSystems.ExternalPractitioner)]
	public class ExternalPractitionerFolderSystem : WorkflowFolderSystem<
		ExternalPractitionerSummary,
		ExternalPractitionerFolderToolExtensionPoint,
		ExternalPractitionerItemToolExtensionPoint,
		ExternalPractitionerSearchParams>
	{
		class ExternalPractitionerItemToolContext : WorkflowItemToolContext, IExternalPractitionerItemToolContext
		{
			public ExternalPractitionerItemToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		class ExternalPractitionerFolderToolContext : WorkflowFolderToolContext, IExternalPractitionerFolderToolContext
		{
			public ExternalPractitionerFolderToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		public ExternalPractitionerFolderSystem()
			: base(SR.TitleExternalPractitionerFolderSystem)
		{
			this.Folders.Add(new UnverifiedFolder());
			this.Folders.Add(new VerifiedTodayFolder());
		}

		#region Search Related

		public override string SearchMessage
		{
			get { return SR.MessageSearchMessageExternalPractitioner; }
		}

		public override bool SearchEnabled
		{
			get { return true; }
		}

		protected override SearchResultsFolder CreateSearchResultsFolder()
		{
			return new ExternalPractitionerSearchFolder();
		}

		public override SearchParams CreateSearchParams(string searchText)
		{
			return new ExternalPractitionerSearchParams(searchText);
		}

		public override bool AdvancedSearchEnabled
		{
			// advance searching not currently supported
			get { return false; }
		}

		public override void LaunchSearchComponent()
		{
			// advance searching not currently supported
			return;
		}

		public override Type SearchComponentType
		{
			// advance searching not currently supported
			get { return null; }
		}

		#endregion

		protected override string GetPreviewUrl(WorkflowFolder folder, ICollection<ExternalPractitionerSummary> items)
		{
			return WebResourcesSettings.Default.ExternalPractitionerFolderSystemUrl;
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new ExternalPractitionerItemToolContext(this);
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new ExternalPractitionerFolderToolContext(this);
		}
	}
}
