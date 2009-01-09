using System;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;

namespace ClearCanvas.Ris.Client
{
	[ButtonAction("apply", "biography-reports-toolbar/Print//Fax Report", "Apply")]
	[IconSet("apply", IconScheme.Colour, "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[EnabledStateObserver("apply", "Enabled", "EnabledChanged")]
	[ExtensionOf(typeof(BiographyOrderReportsToolExtensionPoint))]
	public class BiographyPublishReportTool : Tool<IBiographyOrderReportsToolContext>
	{
		private bool _enabled;
		private event EventHandler _enabledChanged;

		public override void Initialize()
		{
			base.Initialize();

			this.Context.ContextChanged += delegate { this.Enabled = DetermineEnablement(); };
		}

		public void Apply()
		{
			try
			{
				PublishReportComponent component = new PublishReportComponent(
						this.Context.PatientProfileRef,
						this.Context.OrderRef,
						this.Context.ProcedureRef,
						this.Context.ReportRef);

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitlePrintFaxReport);
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