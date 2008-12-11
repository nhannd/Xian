#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Modify Order", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Modify Order", "Apply")]
	[IconSet("apply", IconScheme.Colour, "ModifyOrderSmall.png", "ModifyOrderMedium.png", "ModifyOrderLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Order.Modify)]

	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public abstract class ModifyOrderToolBase<TItem, TContext> : WorkflowItemTool<TItem, TContext>
		where TItem : WorklistItemSummaryBase
		where TContext : IWorkflowItemToolContext<TItem>
	{
		public ModifyOrderToolBase()
			: base("ModifyOrder")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(IOrderEntryService));
		}

		protected abstract void InvalidateFolders();

		protected bool ExecuteCore(WorklistItemSummaryBase item)
		{
			OrderEditorComponent component = new OrderEditorComponent(
				item.PatientRef,
				item.PatientProfileRef,
				item.OrderRef,
				OrderEditorComponent.Mode.ModifyOrder);

			IWorkspace workspace = ApplicationComponent.LaunchAsWorkspace(
				this.Context.DesktopWindow,
				component,
				string.Format("Modify Order - {0} {1}", PersonNameFormat.Format(item.PatientName), MrnFormat.Format(item.Mrn)));

			Type selectedFolderType = this.Context.SelectedFolder.GetType();
			workspace.Closed += delegate
				{
					if (component.ExitCode == ApplicationComponentExitCode.Accepted)
					{
						InvalidateFolders();
						DocumentManager.InvalidateFolder(selectedFolderType);
					}
				};

			return true;
		}
	}

	[ExtensionOf(typeof(RegistrationWorkflowItemToolExtensionPoint))]
	public class RegistrationModifyOrderTool : ModifyOrderToolBase<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(
					this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		protected override bool Execute(RegistrationWorklistItem item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.CheckedInFolder));
		}
	}

	[ExtensionOf(typeof(BookingWorkflowItemToolExtensionPoint))]
	public class BookingModifyOrderTool : ModifyOrderToolBase<RegistrationWorklistItem, IRegistrationWorkflowItemToolContext>
	{
		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(
					this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		protected override bool Execute(RegistrationWorklistItem item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Registration.ToBeScheduledFolder));
		}
	}

	[ExtensionOf(typeof(PerformingWorkflowItemToolExtensionPoint))]
	public class PerformingModifyOrderTool : ModifyOrderToolBase<ModalityWorklistItem, IPerformingWorkflowItemToolContext>
	{
		protected override bool Execute(ModalityWorklistItem item)
		{
			return ExecuteCore(item);
		}

		protected override void InvalidateFolders()
		{
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.ScheduledFolder));
			DocumentManager.InvalidateFolder(typeof(Folders.Performing.CheckedInFolder));
		}
	}
}
