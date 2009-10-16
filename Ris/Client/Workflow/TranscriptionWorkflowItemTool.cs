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

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Application.Common.TranscriptionWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	public abstract class TranscriptionWorkflowItemTool : WorkflowItemTool<ReportingWorklistItem, IReportingWorkflowItemToolContext>
	{
		protected TranscriptionWorkflowItemTool(string operationName)
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
			var workspace = DocumentManager.Get<TranscriptionDocument>(item.ProcedureStepRef);
			if (workspace != null)
			{
				workspace.Activate();
				return true;
			}
			return false;
		}

		protected void OpenTranscriptionEditor(ReportingWorklistItem item)
		{
			if (ActivateIfAlreadyOpen(item))
				return;

			if (!TranscriptionSettings.Default.AllowMultipleTranscriptionWorkspaces)
			{
				var documents = DocumentManager.GetAll<TranscriptionDocument>();

				// Show warning message and ask if the existing document should be closed or not
				if (documents.Count > 0)
				{
					var firstDocument = CollectionUtils.FirstElement(documents);
					firstDocument.Activate();

					var message = string.Format(SR.MessageTranscriptionComponentAlreadyOpened, firstDocument.Title, TranscriptionDocument.GetTitle(item));
					if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
						return;		// Leave the existing document open

					// close documents and continue
					CollectionUtils.ForEach(documents, document => document.Close());
				}
			}

			// open the report editor
			var doc = new TranscriptionDocument(item, this.Context);
			doc.Open();

			// Need to re-invalidate folders that open a report document, since cancelling the report
			// can re-insert items into the same folder.
			var selectedFolderType = this.Context.SelectedFolder.GetType();  // use closure to remember selected folder at time tool is invoked.
			doc.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };
		}

		protected ReportingWorklistItem GetSelectedItem()
		{
			return this.Context.SelectedItems.Count != 1 
				? null 
				: CollectionUtils.FirstElement(this.Context.SelectedItems);
		}
	}
}
