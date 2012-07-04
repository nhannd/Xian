#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ProtocollingWorkflow;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow.Extended
{
	public abstract class ProtocolWorkflowItemTool : WorkflowItemTool<ReportingWorklistItemSummary, IReportingWorkflowItemToolContext>
	{
		protected ProtocolWorkflowItemTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IProtocollingWorkflowService));
		}

		protected ReportingWorklistItemSummary GetSelectedItem()
		{
			if (this.Context.SelectedItems.Count != 1)
				return null;
			return CollectionUtils.FirstElement(this.Context.SelectedItems);
		}

		protected EntityRef GetSupervisorRef()
		{
			ProtocollingSupervisorSelectionComponent component = new ProtocollingSupervisorSelectionComponent();
			if (ApplicationComponentExitCode.Accepted == ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleSelectSupervisor))
			{
				return component.Staff != null ? component.Staff.StaffRef : null;
			}
			else
				return null;
		}
	}
}
