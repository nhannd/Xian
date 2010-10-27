#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
    public interface IRegistrationWorkflowItemToolContext : IWorkflowItemToolContext<RegistrationWorklistItemSummary>
    {
    }

    public interface IRegistrationWorkflowFolderToolContext : IWorkflowFolderToolContext
    {
    }

	public abstract class RegistrationWorkflowFolderSystemBase<TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint>
		: WorklistFolderSystem<RegistrationWorklistItemSummary, TFolderExtensionPoint, TFolderToolExtensionPoint, TItemToolExtensionPoint, IRegistrationWorkflowService>
		where TFolderExtensionPoint : ExtensionPoint<IWorklistFolder>, new()
		where TFolderToolExtensionPoint : ExtensionPoint<ITool>, new()
		where TItemToolExtensionPoint : ExtensionPoint<ITool>, new()
	{
		class RegistrationWorkflowItemToolContext : WorkflowItemToolContext, IRegistrationWorkflowItemToolContext
        {
            public RegistrationWorkflowItemToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }

		class RegistrationWorkflowFolderToolContext : WorkflowFolderToolContext, IRegistrationWorkflowFolderToolContext
        {
            public RegistrationWorkflowFolderToolContext(WorkflowFolderSystem owner)
				:base(owner)
            {
            }
        }


        protected RegistrationWorkflowFolderSystemBase(string title)
            : base(title)
        {
        }

		protected override IWorkflowFolderToolContext CreateFolderToolContext()
		{
			return new RegistrationWorkflowFolderToolContext(this);
		}

		protected override IWorkflowItemToolContext CreateItemToolContext()
		{
			return new RegistrationWorkflowItemToolContext(this);
		}
    }
}