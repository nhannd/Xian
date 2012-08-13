#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Diagnostics;
using System.IO;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("apply", "folderexplorer-items-contextmenu/Print'/Fax Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Print'/Fax Report", "Apply")]
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

				ReportingWorklistItemSummary item = CollectionUtils.FirstElement(this.Context.SelectedItems);
				if (item.ReportRef == null && item.ProcedureRef == null)
					return false;

				return true;
			}
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			try
			{
				//PublishReportComponent component = new PublishReportComponent(
				//        item.PatientProfileRef,
				//        item.OrderRef,
				//        item.ProcedureRef,
				//        item.ReportRef);

				//ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(
				//    this.Context.DesktopWindow,
				//    component,
				//    SR.TitlePrintFaxReport);

				//return exitCode == ApplicationComponentExitCode.Accepted;

				Platform.GetService<IReportingWorkflowService>(
					service =>
						{
							var data = service.PrintReport(new PrintReportRequest(item.ProcedureRef)).ReportPdfData;
							var id = Guid.NewGuid().ToString("N");
							var path = id + ".pdf";
							File.WriteAllBytes(path, data);
							Process.Start(path);
						});
				
				
				return false;
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
				return false;
			}
		}
	}
}
