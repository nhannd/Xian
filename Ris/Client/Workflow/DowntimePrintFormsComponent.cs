using System;
using System.Threading;
using System.Runtime.Serialization;
using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow.OrderEntry;

namespace ClearCanvas.Ris.Client.Workflow
{
	[MenuAction("launch", "global-menus/MenuTools/Print Downtime Forms", "Launch")]
	[IconSet("launch", IconScheme.Colour, "Icons.PrintSmall.png", "Icons.PrintMedium.png", "Icons.PrintLarge.png")]
	[ActionPermission("launch", ClearCanvas.Ris.Application.Common.AuthorityTokens.Workflow.Downtime.PrintForms)]
	[ExtensionOf(typeof(DesktopToolExtensionPoint))]
	public class DowntimePrintFormsTool : Tool<IDesktopToolContext>
	{
		public void Launch()
		{
			try
			{
				DowntimePrintFormsComponent component = new DowntimePrintFormsComponent();

				ApplicationComponent.LaunchAsDialog(
					this.Context.DesktopWindow,
					component,
					SR.TitlePrintDowntimeForms);
			}
			catch (Exception e)
			{
				// could not launch component
				ExceptionHandler.Report(e, this.Context.DesktopWindow);
			}
		}
	}

	/// <summary>
	/// Extension point for views onto <see cref="DowntimePrintFormsComponent"/>
	/// </summary>
	[ExtensionPoint]
	public class DowntimePrintFormsComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
	{
	}

	/// <summary>
	/// DowntimePrintFormsComponent class
	/// </summary>
	[AssociateView(typeof(DowntimePrintFormsComponentViewExtensionPoint))]
	public class DowntimePrintFormsComponent : ApplicationComponent
	{
		public class DowntimeFormViewComponent : DHtmlComponent
		{
			// Internal data contract used for jscript deserialization
			[DataContract]
			public class DowntimeFormContext : DataContractBase
			{
				public DowntimeFormContext(string accessionNumber)
				{
					this.AccessionNumber = accessionNumber;
				}

				[DataMember]
				public string AccessionNumber;
			}

			private DowntimeFormContext _context;

			public override void Start()
			{
				SetUrl(this.PageUrl);
				base.Start();
			}

			public void Refresh()
			{
				NotifyAllPropertiesChanged();
			}

			protected virtual string PageUrl
			{
				get { return WebResourcesSettings.Default.DowntimeFormTemplatePageUrl; }
			}

			protected override DataContractBase GetHealthcareContext()
			{
				return _context;
			}

			public DowntimeFormContext Context
			{
				get { return _context; }
				set
				{
					_context = value;
					Refresh();
				}
			}
		}

		private readonly DowntimeFormViewComponent _formPreviewComponent;
		private ChildComponentHost _formPreviewComponentHost;

		private string _statusText;
		private int _numberOfFormsToPrint;
		private int _numberOfFormsPrinted;

		private bool _isPrinting;
		private bool _printCancelRequested;

		private IEHeaderFooterSettings _headerFooterSettings;
		private IEPrintBackgroundSettings _printBackgroundSettings;

		public DowntimePrintFormsComponent()
		{
			_formPreviewComponent = new DowntimeFormViewComponent();
			_numberOfFormsToPrint = 1;
		}

		public override void Start()
		{
			_formPreviewComponentHost = new ChildComponentHost(this.Host, _formPreviewComponent);
			_formPreviewComponentHost.StartComponent();

			_headerFooterSettings = new IEHeaderFooterSettings();
			_printBackgroundSettings = new IEPrintBackgroundSettings();

			base.Start();
		}

		public override void Stop()
		{
            if (_formPreviewComponentHost != null)
            {
                _formPreviewComponentHost.StopComponent();
                _formPreviewComponentHost = null;    
            }

			base.Stop();
		}

		#region Presentation Model

		public ApplicationComponentHost FormPreviewComponentHost
		{
			get { return _formPreviewComponentHost; }
		}

		public string StatusText
		{
			get { return _statusText; }
		}

		public int NumberOfFormsPrinted
		{
			get { return _numberOfFormsPrinted; }
		}

		public int NumberOfFormsToPrint
		{
			get { return _numberOfFormsToPrint; }
			set { _numberOfFormsToPrint = value; }
		}

		public bool IsPrinting
		{
			get { return _isPrinting; }
		}

		public void StartPrinting()
		{
			try
			{
				_numberOfFormsPrinted = 0;
				_printCancelRequested = false;
				_isPrinting = true;
				UpdateStatus();

				PrintNextForm();
			}
			catch (Exception e)
			{
				ExceptionHandler.Report(e, this.Host.DesktopWindow);
			}
		}

		public void CancelPrinting()
		{
			_printCancelRequested = true;
		}

		#endregion

		private void PrintNextForm()
		{
			if (_printCancelRequested)
			{
				_isPrinting = false;
				_statusText = string.Format(SR.MessagePrintDowntimeFormCancelled, _numberOfFormsPrinted + 1, _numberOfFormsToPrint);
				UpdateStatus();
				return;
			}
			else if (_numberOfFormsPrinted >= _numberOfFormsToPrint)
			{
				_isPrinting = false;
				_statusText = string.Format(SR.MessagePrintDowntimeFormCompleted, _numberOfFormsToPrint);
				UpdateStatus();
				return;
			}

			_statusText = string.Format(SR.MessagePrintDowntimeFormProgress, _numberOfFormsPrinted + 1, _numberOfFormsToPrint);
			UpdateStatus();

			string accessionNumber = null;
			Platform.GetService<IOrderEntryService>(
				delegate(IOrderEntryService service)
				{
					ReserveAccessionNumberResponse response = service.ReserveAccessionNumber(new ReserveAccessionNumberRequest());
					accessionNumber = response.AccessionNumber;
				});

			DowntimeFormViewComponent component = new DowntimeFormViewComponent();
			ChildComponentHost host = new ChildComponentHost(this.Host, component);
			host.StartComponent();
			object view = host.ComponentView.GuiElement;

			component.ScriptCompleted += PrintDocument;
			component.Context = new DowntimeFormViewComponent.DowntimeFormContext(accessionNumber);
		}

		private void PrintDocument(object sender, EventArgs e)
		{
			DowntimeFormViewComponent component = (DowntimeFormViewComponent)sender;
			component.PrintDocument();
			_numberOfFormsPrinted++;
			Thread.Sleep(1000);

			PrintNextForm();
		}

		private void UpdateStatus()
		{
			NotifyAllPropertiesChanged();
		}
	}
}
