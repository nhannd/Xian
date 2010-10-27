#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using ClearCanvas.Common;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public interface IPerformingWorkflowItemToolContext : IWorkflowItemToolContext<ModalityWorklistItemSummary>
    {
    }

    public interface IPerformingWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

	public abstract class PerformingWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<ModalityWorklistItemSummary, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IModalityWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
    {
		class PerformingWorkflowItemToolContext : WorkflowItemToolContext, IPerformingWorkflowItemToolContext
        {
            public PerformingWorkflowItemToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }

        class PerformingWorkflowFolderToolContext : WorkflowFolderToolContext, IPerformingWorkflowFolderToolContext
        {
            public PerformingWorkflowFolderToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }


        protected PerformingWorkflowFolderSystemBase(string title)
            : base(title)
		{
        }

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new PerformingWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new PerformingWorkflowItemToolContext(this);
		}
    }
}
