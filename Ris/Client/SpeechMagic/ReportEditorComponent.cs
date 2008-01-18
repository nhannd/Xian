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
        private StaffSummary _supervisor;
        private ReportDetail _report;
        private ReportPartDetail _reportPart;
        private string _reportContent;

        private bool _isEditingAddendum;
        private bool _verifyEnabled;
        private bool _sendToVerifyEnabled;
        private bool _sendToTranscriptionEnabled;

        private event EventHandler _verifyEvent;
        private event EventHandler _sendToVerifyEvent;
        private event EventHandler _sendToTranscriptionEvent;
        private event EventHandler _saveEvent;
        private event EventHandler _closeComponentEvent;

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
            get { return _reportContent; }
            set { _reportContent = value; }
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

        public event EventHandler VerifyEvent
        {
            add { _verifyEvent += value; }
            remove { _verifyEvent -= value; }
        }

        public event EventHandler SendToVerifyEvent
        {
            add { _sendToVerifyEvent += value; }
            remove { _sendToVerifyEvent -= value; }
        }

        public event EventHandler SendToTranscriptionEvent
        {
            add { _sendToTranscriptionEvent += value; }
            remove { _sendToTranscriptionEvent -= value; }
        }

        public event EventHandler SaveEvent
        {
            add { _saveEvent += value; }
            remove { _saveEvent -= value; }
        }

        public event EventHandler CloseComponentEvent
        {
            add { _closeComponentEvent += value; }
            remove { _closeComponentEvent -= value; }
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
            get { return _supervisor; }
            set { _supervisor = value; }
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
            EventsHelper.Fire(_verifyEvent, this, EventArgs.Empty);
        }

        public void SendToVerify()
        {
            EventsHelper.Fire(_sendToVerifyEvent, this, EventArgs.Empty);
        }

        public void SendToTranscription()
        {
            EventsHelper.Fire(_sendToTranscriptionEvent, this, EventArgs.Empty);
        }

        public void Save()
        {
            EventsHelper.Fire(_saveEvent, this, EventArgs.Empty);
        }

        public void Cancel()
        {
            EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
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
                // could not launch editor
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            return suggestions;
        }

        #endregion
    }
}
