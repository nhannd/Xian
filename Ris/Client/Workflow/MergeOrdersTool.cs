#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{
	[ButtonAction("apply", "folderexplorer-items-toolbar/Merge Orders", "Apply")]
	[MenuAction("apply", "folderexplorer-items-contextmenu/Merge Orders", "Apply")]
	[Tooltip("apply", "Merge Orders")]
	[IconSet("apply", IconScheme.Colour, "MergeOrdersSmall.png", "MergeOrdersMedium.png", "MergeOrdersLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Order.Merge)]
	public abstract class MergeOrdersToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
	{
		protected MergeOrdersToolBase()
			: base("MergeOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		protected abstract void InvalidateFolders();

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count < 2)
					return false;

				var list = new List<TItem>(this.Context.SelectedItems);

				// Obvious cases where merging should not be allowed.
				// Cannot merge the same order.  If Unique list has the same number as the original list, every item in the list is unique.
				var sameOrder = CollectionUtils.TrueForAll(list, item => item.AccessionNumber.Equals(list[0].AccessionNumber));
				if (sameOrder)
					return false;

				// Cannot merge orders from different patient
				var differentPatient = CollectionUtils.Contains(list, item => !item.PatientRef.Equals(list[0].PatientRef, true));
				if (differentPatient)
					return false;

				// Return true, let the server decide how to inform user of more complicated error.
				return true;
			}
		}

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			var list = new List<TItem>(this.Context.SelectedItems);
			var orderRefs = CollectionUtils.Map<TItem, EntityRef>(list, x => x.OrderRef);
			var component = new MergeOrdersComponent(orderRefs);

			if (ApplicationComponentExitCode.Accepted != ApplicationComponent.LaunchAsDialog(this.Context.DesktopWindow, component, SR.TitleMergeOrders))
				return false;

			InvalidateFolders();

			return true;
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationMergeOrdersTool : MergeOrdersToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected override bool Execute(RegistrationWorklistItemSummary item)
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
	public class BookingMergeOrdersTool : MergeOrdersToolBase<RegistrationWorklistItemSummary, IRegistrationWorkflowItemToolContext>
	{
		protected override bool Execute(RegistrationWorklistItemSummary item)
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
	public class PerformingMergeOrdersTool : MergeOrdersToolBase<ModalityWorklistItemSummary, IPerformingWorkflowItemToolContext>
	{
		protected override bool Execute(ModalityWorklistItemSummary item)
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
