using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Open Protocol", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Open Protocol", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Protocol.Create)]
	[ExtensionOf(typeof(ProtocolWorkflowItemToolExtensionPoint))]
	public class EditProtocolTool : ProtocolWorkflowItemTool
	{
		public EditProtocolTool()
			: base("EditProtocol")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Reporting.DraftProtocolFolder), this);
			this.Context.RegisterDoubleClickHandler(
				(IClickAction)CollectionUtils.SelectFirst(this.Actions,
					delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
		}

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count != 1)
					return false;

				ReportingWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
				if (item.OrderRef == null)
					return false;

				if (item.ProcedureStepRef == null)
					return false;

				return true;
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItem> items)
		{
			// this tool is only registered as a drop handler for the Drafts folder
			// and the only operation that would make sense in this context is StartInterpretation
			return this.Context.GetOperationEnablement("StartOrderProtocol");
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			// open the protocol editor
			ProtocollingComponentDocument protocollingComponentDocument = new ProtocollingComponentDocument(item, GetMode(item), this.Context);
			protocollingComponentDocument.Open();

			Type selectedFolderType = this.Context.SelectedFolder.GetType();
			protocollingComponentDocument.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };

			return true;
		}

		private static bool ActivateIfAlreadyOpen(ReportingWorklistItem item)
		{
			Workspace workspace = DocumentManager.Get<ProtocollingComponentDocument>(item.OrderRef);
			if (workspace != null)
			{
				workspace.Activate();
				return true;
			}
			return false;
		}

		private ProtocollingComponentMode GetMode(ReportingWorklistItem item)
		{
			if (item == null)
				return ProtocollingComponentMode.Review;

			if (CanCreateProtocol(item))
				return ProtocollingComponentMode.Assign;
			else if (CanEditProtocol(item))
				return ProtocollingComponentMode.Edit;
			else
				return ProtocollingComponentMode.Review;
		}

		private bool CanCreateProtocol(ReportingWorklistItem item)
		{
			return this.Context.GetOperationEnablement("StartProtocol");
		}

		private bool CanEditProtocol(ReportingWorklistItem item)
		{
			// there is no specific workflow operation for editing a previously created draft,
			// so we enable the tool if it looks like a draft and SaveReport is enabled
			return this.Context.GetOperationEnablement("SaveProtocol") && item.ActivityStatus.Code == StepState.InProgress;
		}
	}
}