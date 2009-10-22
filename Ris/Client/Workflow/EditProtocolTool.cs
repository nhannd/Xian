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
	[ActionPermission("apply", Application.Common.AuthorityTokens.Workflow.Protocol.Create)]
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
				(IClickAction)CollectionUtils.SelectFirst(this.Actions, a => a is IClickAction && a.ActionID.EndsWith("apply")));
		}

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count != 1)
					return false;

				var item = CollectionUtils.FirstElement(this.Context.SelectedItems);
				if (item.OrderRef == null)
					return false;

				return item.ProcedureStepRef != null;
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
			if (ActivateIfAlreadyOpen(item))
				return;

			if (!ProtocollingSettings.Default.AllowMultipleProtocollingWorkspaces)
			{
				var documents = DocumentManager.GetAll<ProtocolDocument>();

				// Show warning message and ask if the existing document should be closed or not
				if (documents.Count > 0)
				{
					var firstDocument = CollectionUtils.FirstElement(documents);
					firstDocument.Open();

					var message = string.Format(SR.MessageProtocollingComponentAlreadyOpened, 
						ProtocolDocument.StripTitle(firstDocument.GetTitle()), 
						ProtocolDocument.StripTitle(ProtocolDocument.GetTitle(item)));
					if (DialogBoxAction.No == this.Context.DesktopWindow.ShowMessageBox(message, MessageBoxActions.YesNo))
						return;		// Leave the existing document open

					// close documents and continue
					CollectionUtils.ForEach(documents, document => document.SaveAndClose());
				}
			}

			// open the protocol editor
			var protocolDocument = new ProtocolDocument(item, GetMode(item), this.Context);
			protocolDocument.Open();

			var selectedFolderType = this.Context.SelectedFolder.GetType();
			protocolDocument.Closed += delegate { DocumentManager.InvalidateFolder(selectedFolderType); };
		}

		private static bool ActivateIfAlreadyOpen(ReportingWorklistItem item)
		{
			var document = DocumentManager.Get<ProtocolDocument>(item.OrderRef);
			if (document != null)
			{
				document.Open();
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
			
			return CanEditProtocol(item) 
				? ProtocollingComponentModes.Edit 
				: ProtocollingComponentModes.Review;
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