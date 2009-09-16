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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client
{
	/// <summary>
	/// Extends <see cref="OrderNoteConversationToolBase{TSummaryItem,TToolContext}"/> to provide a base class for tools which open an 
	/// <see cref="OrderNoteConversationComponent"/> for the purpose of creating a preliminary diagnosis
	/// </summary>
	/// <typeparam name="TSummaryItem"></typeparam>
	/// <typeparam name="TToolContext"></typeparam>
	[MenuAction("pd", "folderexplorer-items-contextmenu/Preliminary Diagnosis", "Open")]
	[ButtonAction("pd", "folderexplorer-items-toolbar/Preliminary Diagnosis", "Open")]
	[Tooltip("pd", "Create/view the preliminary diagnosis for the selected item")]
	[EnabledStateObserver("pd", "Enabled", "EnabledChanged")]
	[IconSet("pd", IconScheme.Colour, "Icons.PrelimDiagConvoToolSmall.png", "Icons.PrelimDiagConvoToolMedium.png", "Icons.PrelimDiagConvoToolLarge.png")]
	[ActionPermission("pd", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.PreliminaryDiagnosis.Create)]
	public abstract class PreliminaryDiagnosisConversationTool<TSummaryItem, TToolContext> : OrderNoteConversationToolBase<TSummaryItem, TToolContext>
		where TSummaryItem : WorklistItemSummaryBase
		where TToolContext : IWorkflowItemToolContext<TSummaryItem>
	{
		protected override EntityRef OrderRef
		{
			get { return this.SummaryItem.OrderRef; }
		}

		public override bool Enabled
		{
			get
			{
				return base.Enabled && this.SummaryItem.OrderRef != null;
			}
		}

		protected override string TitleContextDescription
		{
			get
			{
				return string.Format(SR.FormatTitleContextDescriptionOrderNoteConversation,
					PersonNameFormat.Format(this.SummaryItem.PatientName),
					MrnFormat.Format(this.SummaryItem.Mrn),
					AccessionFormat.Format(this.SummaryItem.AccessionNumber));
			}
		}

		protected override IEnumerable<string> OrderNoteCategories
		{
			get { return new string[] { OrderNoteCategory.PreliminaryDiagnosis.Key }; }
		}

		protected override void OnDialogClosed(ApplicationComponentExitCode exitCode)
		{
			this.Context.InvalidateSelectedFolder();

			base.OnDialogClosed(exitCode);
		}
	}
}