using System;
using System.Collections;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using AuthorityTokens = ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client.SpeechMagic
{
    [ExtensionPoint]
    public class ReportEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(ReportEditorComponentViewExtensionPoint))]
    [ExtensionOf(typeof(ReportEditorExtensionPoint))]
    public class ReportEditorComponent : ApplicationComponent, IReportEditor
    {
        public class DHtmlReportEditorComponent : DHtmlComponent
        {
            private readonly ReportEditorComponent _owner;

            public DHtmlReportEditorComponent(ReportEditorComponent owner)
            {
                _owner = owner;
            }

            public void Refresh()
            {
                NotifyAllPropertiesChanged();
            }

            protected override string GetTag(string tag)
            {
                switch (tag)
                {
                    case "Report":
                        ReportPartDetail reportPart = _owner._report.GetPart(0);
                        return reportPart == null ? "" : reportPart.Content;
                    case "Addendum":
                        ReportPartDetail addendumPart = _owner._reportPart.Index > 0 ? _owner._reportPart : null;
                        return addendumPart == null ? "" : addendumPart.Content;
                    case "Preview":
                    default:
                        return JsmlSerializer.Serialize(_owner._report, "report");
                }
            }

            protected override void SetTag(string tag, string data)
            {
                switch (tag)
                {
                    case "Report":
                    case "Addendum":
                        _owner.ReportContent = data;
                        break;
                    default:
                        break;
                }
            }

            protected override DataContractBase GetHealthcareContext()
            {
                return _owner._worklistItem;
            }
        }

        private readonly DHtmlReportEditorComponent _reportPreviewComponent;
        private ChildComponentHost _reportPreviewHost;

        private ReportingWorklistItem _worklistItem;
        private ReportDetail _report;
        private ReportPartDetail _reportPart;

        private bool _isEditingAddendum;
        private bool _verifyEnabled;
        private bool _sendToVerifyEnabled;
        private bool _sendToTranscriptionEnabled;

        private event EventHandler _verifyRequested;
        private event EventHandler _sendToVerifyRequested;
        private event EventHandler _sendToTranscriptionRequested;
        private event EventHandler _saveRequested;
        private event EventHandler _closeRequested;

        private ILookupHandler _supervisorLookupHandler;

        public ReportEditorComponent()
        {
            _reportPreviewComponent = new DHtmlReportEditorComponent(this);
        }

        public override void Start()
        {
            _supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, new string[] { "PRAD" });

            _reportPreviewComponent.SetUrl(this.PreviewUrl);
            _reportPreviewHost = new ChildComponentHost(this.Host, _reportPreviewComponent);
            _reportPreviewHost.StartComponent();

            base.Start();
        }

        private string PreviewUrl
        {
            get { return this.IsEditingAddendum ? ReportEditorComponentSettings.Default.ReportPreviewPageUrl : "about:blank"; }
        }

        #region IReportEditor Members

        public string ReportContent
        {
            get { return _reportPart.Content; }
            set { _reportPart.Content = value; }
        }

        public ReportingWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set { _worklistItem = value; }
        }

        public ReportDetail Report
        {
            get { return _report; }
            set { _report = value; }
        }

        public ReportPartDetail ReportPart
        {
            get { return _reportPart; }
            set { _reportPart = value; }
        }

        public event EventHandler VerifyRequested
        {
            add { _verifyRequested += value; }
            remove { _verifyRequested -= value; }
        }

        public event EventHandler SendToVerifyRequested
        {
            add { _sendToVerifyRequested += value; }
            remove { _sendToVerifyRequested -= value; }
        }

        public event EventHandler SendToTranscriptionRequested
        {
            add { _sendToTranscriptionRequested += value; }
            remove { _sendToTranscriptionRequested -= value; }
        }

        public event EventHandler SaveRequested
        {
            add { _saveRequested += value; }
            remove { _saveRequested -= value; }
        }

        public event EventHandler CancelRequested
        {
            add { _closeRequested += value; }
            remove { _closeRequested -= value; }
        }

        public bool IsEditingAddendum
        {
            get { return _isEditingAddendum; }
            set { _isEditingAddendum = value; }
        }

        public bool VerifyEnabled
        {
            get { return _verifyEnabled; }
            set { _verifyEnabled = value; }
        }

        public bool SendToVerifyEnabled
        {
            get { return _sendToVerifyEnabled; }
            set { _sendToVerifyEnabled = value; }
        }

        public bool SendToTranscriptionEnabled
        {
            get { return _sendToTranscriptionEnabled; }
            set { _sendToTranscriptionEnabled = value; }
        }

        public StaffSummary Supervisor
        {
            get { return _reportPart.Supervisor; }
            set { _reportPart.Supervisor = value; }
        }

        #endregion

        #region Presentation Model

        public ApplicationComponentHost ReportPreviewHost
        {
            get { return _reportPreviewHost; }
        }

        public bool CanSendToTranscription
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UseTranscriptionWorkflow); }
        }

        public bool CanVerifyReport
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport); }
        }

        public void Verify()
        {
            EventsHelper.Fire(_verifyRequested, this, EventArgs.Empty);
        }

        public void SendToVerify()
        {
            EventsHelper.Fire(_sendToVerifyRequested, this, EventArgs.Empty);
        }

        public void SendToTranscription()
        {
            EventsHelper.Fire(_sendToTranscriptionRequested, this, EventArgs.Empty);
        }

        public void Save()
        {
            EventsHelper.Fire(_saveRequested, this, EventArgs.Empty);
        }

        public void Cancel()
        {
            EventsHelper.Fire(_closeRequested, this, EventArgs.Empty);
        }

        public ILookupHandler SupervisorLookupHandler
        {
            get { return _supervisorLookupHandler; }
        }

        public IList GetRadiologistSuggestion(string query)
        {
            ArrayList suggestions = new ArrayList();
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        GetRadiologistListResponse response = service.GetRadiologistList(new GetRadiologistListRequest());
                        suggestions.AddRange(response.Radiologists);
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            return suggestions;
        }

        #endregion
    }
}
