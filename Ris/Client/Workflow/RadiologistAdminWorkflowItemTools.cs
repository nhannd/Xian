#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Reassign", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Reassign", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.AssignSmall.png", "Icons.AssignMedium.png", "Icons.AssignLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Report.Reassign)]
	[ExtensionOf(typeof(RadiologistAdminWorkflowItemToolExtensionPoint))]
	public class ReassignTool : ReportingWorkflowItemTool
	{
		public ReassignTool()
			: base("ReassignProcedureStep")
		{
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			try
			{
				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					new ReassignComponent(item),
					SR.TitleReassignItem);

				return exitCode == ApplicationComponentExitCode.Accepted;
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
				return false;
			}
		}
	}
}
