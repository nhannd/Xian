using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "biography-reports-toolbar/Print Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[ExtensionOf(typeof(BiographyOrderReportsToolExtensionPoint))]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	public class BiographyPrintReportTool : Tool<IBiographyOrderReportsToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			((IBiographyOrderReportsToolContext)this.ContextBase).ContextChanged += delegate
			{
				this.Enabled = DetermineEnablement();
			};
		}

		public void Apply()
		{
			try
			{
				PrintReportComponent component = new PrintReportComponent(
					this.Context.PatientProfileRef,
					this.Context.OrderRef,
					this.Context.ReportRef);

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitlePrintReport);
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}

		private bool DetermineEnablement()
		{
			if (this.Context.PatientProfileRef == null) return false;
			if (this.Context.OrderRef == null) return false;
			if (this.Context.ReportRef == null) return false;

			return true;
		}

		public bool Enabled
		{
			get
			{
				this.Enabled = DetermineEnablement();
				return _enabled;
			}
			set
			{
				if (_enabled != value)
				{
					_enabled = value;
					EventsHelper.Fire(_enabledChanged, this, EventArgs.Empty);
				}
			}
		}

		public event EventHandler EnabledChanged
		{
			add { _enabledChanged += value; }
			remove { _enabledChanged -= value; }
		}

	}
}