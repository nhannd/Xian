#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IReportingWorkflowItemToolContext : IWorkflowItemToolContext<ReportingWorklistItemSummary>
	{
	}

	public interface IReportingWorkflowFolderToolContext : IWorkflowFolderToolContext
	{
	}

	public abstract class ReportingWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<ReportingWorklistItemSummary, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IReportingWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
	{
		class ReportingWorkflowItemToolContext : WorkflowItemToolContext, IReportingWorkflowItemToolContext
		{
			public ReportingWorkflowItemToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		class ReportingWorkflowFolderToolContext : WorkflowFolderToolContext, IReportingWorkflowFolderToolContext
		{
			public ReportingWorkflowFolderToolContext(WorkflowFolderSystem owner)
				: base(owner)
			{
			}
		}

		protected ReportingWorkflowFolderSystemBase(string title)
			: base(title)
		{
		}

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new ReportingWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new ReportingWorkflowItemToolContext(this);
		}

		protected static bool CurrentStaffCanSupervise()
		{
			string filters = ReportingSettings.Default.SupervisorStaffTypeFilters;
			List<string> staffTypes = string.IsNullOrEmpty(filters)
										? new List<string>()
										: CollectionUtils.Map<string, string>(filters.Split(','), delegate(string s) { return s.Trim(); });
			string currentUserStaffType = LoginSession.Current.Staff.StaffType.Code;
			return staffTypes.Contains(currentUserStaffType);
		}
	}
}
