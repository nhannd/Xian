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
using System.Collections.Generic;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Edit Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Edit Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[IconSetObserver("apply", "CurrentIconSet", "LabelChanged")]
	[LabelValueObserver("apply", "Label", "LabelChanged")]
	[ActionPermission("apply", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Report.Create)]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class EditReportTool : ReportingWorkflowItemTool
	{
		public EditReportTool()
			: base("EditReport")
		{
		}

		public override void Initialize()
		{
			base.Initialize();

			this.Context.RegisterDropHandler(typeof(Folders.Reporting.DraftFolder), this);
            this.Context.RegisterDoubleClickHandler(
                (IClickAction)CollectionUtils.SelectFirst(this.Actions,
                    delegate(IAction a) { return a is IClickAction && a.ActionID.EndsWith("apply"); }));
        }

		public string Label
		{
			get
			{
				ReportingWorklistItem item = GetSelectedItem();
				if (item != null && item.ProcedureStepName == StepType.Interpretation && item.ActivityStatus.Code == StepState.Scheduled)
					return SR.TitleCreateReport;
				else
					return SR.TitleEditReport;
			}
		}

		public IconSet CurrentIconSet
		{
		    get
		    {
		        ReportingWorklistItem item = GetSelectedItem();
		        if (item != null && item.ProcedureStepName == StepType.Interpretation && item.ActivityStatus.Code == StepState.Scheduled)
		            return new IconSet(IconScheme.Colour, "Icons.CreateReportSmall.png", "Icons.CreateReportMedium.png", "Icons.CreateReportMedium.png");
		        else
		            return new IconSet(IconScheme.Colour, "Icons.EditReportToolSmall.png", "Icons.EditReportToolMedium.png", "Icons.EditReportToolLarge.png");
		    }
		}

		public event EventHandler LabelChanged
		{
			add { this.Context.SelectionChanged += value; }
			remove { this.Context.SelectionChanged -= value; }
		}

		public override bool Enabled
		{
			get
			{
				ReportingWorklistItem item = GetSelectedItem();

				if (this.Context.SelectedItems.Count != 1)
					return false;

				return
					this.Context.GetOperationEnablement("StartInterpretation") ||
					this.Context.GetOperationEnablement("StartTranscriptionReview") ||
					this.Context.GetOperationEnablement("StartVerification") ||

					// there is no specific workflow operation for editing a previously created draft,
					// so we enable the tool if it looks like a draft and SaveReport is enabled
					(this.Context.GetOperationEnablement("SaveReport") && item != null && item.ActivityStatus.Code == StepState.InProgress);
			}
		}

		public override bool CanAcceptDrop(ICollection<ReportingWorklistItem> items)
		{
			// this tool is only registered as a drop handler for the Drafts folder
			// and the only operation that would make sense in this context is StartInterpretation
			return this.Context.GetOperationEnablement("StartInterpretation");
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			// check if the document is already open
			if (ActivateIfAlreadyOpen(item))
				return true;

			// open the report editor
			OpenReportEditor(item);

			return true;
		}
	}
}
