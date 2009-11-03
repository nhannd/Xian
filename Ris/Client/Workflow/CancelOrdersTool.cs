#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
    [MenuAction("apply", "folderexplorer-items-contextmenu/Cancel Order", "Apply")]
    [ButtonAction("apply", "folderexplorer-items-toolbar/Cancel Order", "Apply")]
    [IconSet("apply", IconScheme.Colour, "Icons.CancelOrderSmall.png", "Icons.CancelOrderMedium.png", "Icons.CancelOrderLarge.png")]
    [EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
    [ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Cancel)]
    public abstract class CancelOrderToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
        where TItem : WorklistItemSummaryBase
        where TContext : IWorkflowItemToolContext<TItem>
    {
    	protected CancelOrderToolBase()
            : base("CancelOrder")
        {
        }

        public override void Initialize()
        {
            base.Initialize();

            // bug #3199: cancel operation moved to IOrderEntryService -> need to register for enablement
            this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
        }

        protected abstract void InvalidateFolders();

        protected bool ExecuteCore(WorklistItemSummaryBase item)
        {
			// first check for warnings
			var warnings = new List<string>();
			Platform.GetService<IOrderEntryService>(
				service => warnings = service.QueryCancelOrderWarnings(new QueryCancelOrderWarningsRequest(item.OrderRef)).Warnings);

			if (warnings.Count > 0)
			{
				var warn = CollectionUtils.FirstElement(warnings);
				var action = this.Context.DesktopWindow.ShowMessageBox(
					warn + "\n\nAre you sure you want to cancel this order?",
					MessageBoxActions.YesNo);
				if (action == DialogBoxAction.No)
					return false;
			}

			var cancelOrderComponent = new CancelOrderComponent();
            var exitCode = ApplicationComponent.LaunchAsDialog(
                this.Context.DesktopWindow,
                cancelOrderComponent,
                String.Format(SR.TitleCancelOrder, PersonNameFormat.Format(item.PatientName)));

            if (exitCode == ApplicationComponentExitCode.Accepted)
            {
                Platform.GetService<IOrderEntryService>(
                	service => service.CancelOrder(new CancelOrderRequest(item.OrderRef, cancelOrderComponent.SelectedCancelReason)));

                InvalidateFolders();
                return true;
            }
        	return false;
        }
    }

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
    public class RegistrationCancelOrderTool : CancelOrderToolBase<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Registration.CancelledFolder), this);
		}

		protected override bool Execute(RegistrationWorklistItem item)
		{
            return ExecuteCore(item);
		}

        protected override void InvalidateFolders()
        {
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.CancelledFolder));
        }
    }

    [ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
    public class BookingCancelOrderTool : CancelOrderToolBase<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
    {
        protected override bool Execute(RegistrationWorklistItem item)
        {
            return ExecuteCore(item);
        }

        protected override void InvalidateFolders()
        {
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.ToBeScheduledFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.Registration.PendingProtocolFolder));
        }
    }

    [ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
    public class PerformingCancelOrderTool : CancelOrderToolBase<ModalityWorklistItem, IPerformingWorkflowItemToolContext>
    {
        protected override bool Execute(ModalityWorklistItem item)
        {
            return ExecuteCore(item);
        }

        protected override void InvalidateFolders()
        {
            DocumentManager.InvalidateFolder(typeof(Folders.Performing.ScheduledFolder));
            DocumentManager.InvalidateFolder(typeof(Folders.Performing.CancelledFolder));
        }
    }
}