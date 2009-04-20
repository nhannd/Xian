using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

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

			OpenProtocolEditor(item);

			return true;
		}

		private void OpenProtocolEditor(ReportingWorklistItem item)
		{
			if (!ActivateIfAlreadyOpen(item))
			{
				if (!ProtocollingSettings.Default.AllowMultipleProtocollingWorkspaces)
				{
					List<Workspace> documents = DocumentManager.GetAll<ProtocolDocument>();

					// Show warning message and ask if the existing document should be closed or not
					if (documents.Count > 0)
					{
						Workspace firstDocument = CollectionUtils.FirstElement(documents);
						firstDocument.Activate();

						string message = string.Format(SR.MessageProtocollingComponentAlreadyOpened, firstDocument.Title, PersonNameFormat.Format(item.PatientName));
						if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
						{
							// Leave the existing document open
							return;
						}
						else
						{
							// close documents and continue
							CollectionUtils.ForEach(documents, delegate(Workspace document) { document.Close(); });
						}
					}
				}

				// open the protocol editor
				ProtocolDocument protocolDocument = new ProtocolDocument(item, GetMode(item), this.Context);
				protocolDocument.Open();

				Type selectedFolderType = this.Context.SelectedFolder.GetType();
				protocolDocument.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };
			}
		}

		private static bool ActivateIfAlreadyOpen(ReportingWorklistItem item)
		{
			Workspace workspace = DocumentManager.Get<ProtocolDocument>(item.OrderRef);
			if (workspace != null)
			{
				workspace.Activate();
				return true;
			}
			return false;
		}

		private IContinuousWorkflowComponentMode GetMode(ReportingWorklistItem item)
		{
			if (item == null)
				return ProtocollingComponentModes.Review;

			if (CanCreateProtocol(item))
				return ProtocollingComponentModes.Assign;
			else if (CanEditProtocol(item))
				return ProtocollingComponentModes.Edit;
			else
				return ProtocollingComponentModes.Review;
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