using System;
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class TranscriptionWorkflowItemTool : WorkflowItemTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
		public TranscriptionWorkflowItemTool(string operationName)
			: base(operationName)
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterWorkflowService(typeof(ITranscriptionWorkflowService));
		}

		protected bool ActivateIfAlreadyOpen(ReportingWorklistItem item)
		{
			Workspace workspace = DocumentManager.Get<TranscriptionDocument>(item.ProcedureStepRef);
			if (workspace != null)
			{
				workspace.Activate();
				return true;
			}
			return false;
		}

		protected void OpenTranscriptionEditor(ReportingWorklistItem item)
		{
			if (!ActivateIfAlreadyOpen(item))
			{
				if (!TranscriptionSettings.Default.AllowMultipleTranscriptionWorkspaces)
				{
					List<Workspace> documents = DocumentManager.GetAll<TranscriptionDocument>();

					// Show warning message and ask if the existing document should be closed or not
					if (documents.Count > 0)
					{
						Workspace firstDocument = CollectionUtils.FirstElement(documents);
						firstDocument.Activate();

						string message = string.Format(SR.MessageTranscriptionComponentAlreadyOpened, firstDocument.Title, PersonNameFormat.Format(item.PatientName));
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

				// open the report editor
				TranscriptionDocument doc = new TranscriptionDocument(item, this.Context);
				doc.Open();

				// Need to re-invalidate folders that open a report document, since cancelling the report
				// can re-insert items into the same folder.
				Type selectedFolderType = this.Context.SelectedFolder.GetType();  // use closure to remember selected folder at time tool is invoked.
				doc.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };
			}
		}

		protected ReportingWorklistItem GetSelectedItem()
		{
			if (this.Context.SelectedItems.Count != 1)
				return null;
			return CollectionUtils.FirstElement(this.Context.SelectedItems);
		}
	}
}
