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
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Print//Fax Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Print//Fax Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class SendReportToQueueTool : ReportingWorkflowItemTool
	{
		public SendReportToQueueTool()
			: base("SendReportToQueue")
		{
		}

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count != 1 || !this.Context.GetOperationEnablement("SendReportToQueue"))
					return false;

				ReportingWorklistItem item = CollectionUtils.FirstElement(this.Context.SelectedItems);
				if (item.ReportRef == null && item.ProcedureRef == null)
					return false;

				return true;
			}
		}

		protected override bool Execute(ReportingWorklistItem item)
		{
			try
			{
				PublishReportComponent component = new PublishReportComponent(
						item.PatientProfileRef,
						item.OrderRef,
						item.ProcedureRef,
						item.ReportRef);

				ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitlePrintFaxReport);

				return exitCode == ApplicationComponentExitCode.Accepted;
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
				return false;
			}
		}
	}
}
