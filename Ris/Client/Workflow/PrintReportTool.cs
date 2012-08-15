#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Workflow
{
	//todo: loc
	[MenuAction("apply", "folderexplorer-items-contextmenu/Print Report", "Apply")]
	[ButtonAction("apply", "folderexplorer-items-toolbar/Print Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(ReportingWorkflowItemToolExtensionPoint))]
	public class PrintReportTool : ReportingWorkflowItemTool
	{
		public PrintReportTool()
			: base("PrintReport")
		{
		}

		public override bool Enabled
		{
			get
			{
				if (this.Context.SelectedItems.Count != 1)
					return false;

				var item = CollectionUtils.FirstElement(this.Context.SelectedItems);
				return item.ReportRef != null;
			}
		}

		protected override bool Execute(ReportingWorklistItemSummary item)
		{
			if (item.ReportRef == null)
				return false;

			try
			{
				//todo: loc
				var title = string.Format("Print Report for {0} {1} -  {2}",
										  Formatting.MrnFormat.Format(item.Mrn),
										  Formatting.PersonNameFormat.Format(item.PatientName),
										  Formatting.AccessionFormat.Format(item.AccessionNumber));

				var component = new PrintReportComponent(item.OrderRef, item.ReportRef);
				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					title);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}

			// always return false - we didn't change any data
			return false;
		}
	}
}
