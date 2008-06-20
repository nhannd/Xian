using System;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "biographyorderreports-toolbar/Print Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.EditToolSmall.png", "Icons.EditToolSmall.png", "Icons.EditToolSmall.png")]
	[ExtensionOf(typeof(BiographyOrderReportsToolExtensionPoint))]
	public class BiographyPrintReportTool : Tool<IBiographyOrderReportsToolContext>
	{
		public void Apply()
		{
			try
			{
				RepublishReportComponent component = new RepublishReportComponent(
					this.Context.PatientProfileRef,
					this.Context.OrderRef,
					this.Context.ReportRef);

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					"TODO: Print Report");
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}
}