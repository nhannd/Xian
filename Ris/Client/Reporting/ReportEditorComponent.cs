using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Ris.Application.Common.Jsml;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="ReportEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportEditorComponent class
    /// </summary>
    [AssociateView(typeof(ReportEditorComponentViewExtensionPoint))]
    public class ReportEditorComponent : ApplicationComponent
    {
        /// <summary>
        /// The script callback is an object that is made available to the web browser so that
        /// the javascript code can invoke methods on the host.  It must be COM-visible.
        /// </summary>
        [ComVisible(true)]
        public class ScriptCallback
        {
            private ReportEditorComponent _component;

            public ScriptCallback(ReportEditorComponent component)
            {
                _component = component;
            }

            public void Alert(string message)
            {
                _component.Host.ShowMessageBox(message, MessageBoxActions.Ok);
            }

            public string GetData(string tag)
            {
                return _component.GetData(tag);
            }

            public void SetData(string tag, string data)
            {
                _component.SetData(tag, data);
            }
        }

        private ScriptCallback _scriptCallback;

        private EntityRef _reportingStepRef;
        private int _reportPartIndex;
        private ReportSummary _report;

        private bool _canCompleteInterpretationAndVerify;
        private bool _canCompleteVerification;
        private bool _canCompleteInterpretationForVerification;
        private bool _canCompleteInterpretationForTranscription;

        private event EventHandler _verifyEvent;
        private event EventHandler _sendToVerifyEvent;
        private event EventHandler _sendToTranscriptionEvent;
        private event EventHandler _closeComponentEvent;

        private string _reportContent;

        public ReportEditorComponent(EntityRef reportingStepRef)
        {
            _scriptCallback = new ScriptCallback(this);
            _reportingStepRef = reportingStepRef;
        }

        public override void Start()
        {
            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetOperationEnablementResponse enablementResponse = service.GetOperationEnablement(new GetOperationEnablementRequest(_reportingStepRef));
                    _canCompleteInterpretationAndVerify = enablementResponse.OperationEnablementDictionary["CompleteInterpretationAndVerify"];
                    _canCompleteVerification = enablementResponse.OperationEnablementDictionary["CompleteVerification"];
                    _canCompleteInterpretationForVerification = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForVerification"];
                    _canCompleteInterpretationForTranscription = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForTranscription"];

                    LoadReportForEditResponse response = service.LoadReportForEdit(new LoadReportForEditRequest(_reportingStepRef));
                    _reportPartIndex = response.ReportPartIndex;
                    _report = response.Report;
                });

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
        }

        public string EditorUrl
        {
            get { return this.IsEditingAddendum ? ReportEditorComponentSettings.Default.AddendumEditorPageUrl : ReportEditorComponentSettings.Default.ReportEditorPageUrl; }
        }

        public string PreviewUrl
        {
            get { return this.IsEditingAddendum ? ReportEditorComponentSettings.Default.ReportPreviewPageUrl : "about:blank"; }
        }

        public ScriptCallback ScriptObject
        {
            get { return _scriptCallback; }
        }

        public string ReportContent
        {
            get { return _reportContent; }
            set { _reportContent = value; }
        }

        public bool IsEditingAddendum
        {
            get { return _reportPartIndex > 0; }
        }

        #region Event Handlers

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

        public event EventHandler CloseComponentEvent
        {
            add { _closeComponentEvent += value; }
            remove { _closeComponentEvent -= value; }
        }

        #endregion

        #region Presentation Model

        public string PatientName
        {
            get { return PersonNameFormat.Format(_report.Name); }
        }

        public string Mrn
        {
            get { return MrnFormat.Format(_report.Mrn); }
        }

        public string DateOfBirth
        {
            get { return Format.Date(_report.DateOfBirth); }
        }

        public string VisitNumber
        {
            get { return String.Format("{0} {1}", _report.VisitNumberAssigningAuthority, _report.VisitNumberId); }
        }

        public string AccessionNumber
        {
            get { return _report.AccessionNumber; }
        }

        public string DiagnosticService
        {
            get { return _report.DiagnosticServiceName; }
        }

        public string RequestedProcedure
        {
            get { return _report.RequestedProcedureName; }
        }

        public string PerformedLocation
        {
            get { return _report.PerformedLocation; }
        }

        public string PerformedDate
        {
            get { return Format.Date(_report.PerformedDate); }
        }

        public bool VerifyEnabled
        {
            get { return _canCompleteInterpretationAndVerify || _canCompleteVerification; }
        }

        public bool SendToVerifyEnabled
        {
            get { return _canCompleteInterpretationForVerification; }
        }

        public bool SendToTranscriptionEnabled
        {
            get { return _canCompleteInterpretationForTranscription; }
        }

        public void Verify()
        {
            try
            {
                if (_canCompleteInterpretationAndVerify)
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.SaveReport(new SaveReportRequest(_reportingStepRef, this.ReportContent));
                            service.CompleteInterpretationAndVerify(new CompleteInterpretationAndVerifyRequest(_reportingStepRef));
                        });
                }
                else if (_canCompleteVerification)
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.SaveReport(new SaveReportRequest(_reportingStepRef, this.ReportContent));
                            service.CompleteVerification(new CompleteVerificationRequest(_reportingStepRef));
                        });
                }

                EventsHelper.Fire(_verifyEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate()
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
                    });
            }
        }

        public void SendToVerify()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.SaveReport(new SaveReportRequest(_reportingStepRef, this.ReportContent));
                        service.CompleteInterpretationForVerification(new CompleteInterpretationForVerificationRequest(_reportingStepRef));
                    });

                EventsHelper.Fire(_sendToVerifyEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate()
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
                    });
            }
        }

        public void SendToTranscription()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.SaveReport(new SaveReportRequest(_reportingStepRef, this.ReportContent));
                        service.CompleteInterpretationForTranscription(new CompleteInterpretationForTranscriptionRequest(_reportingStepRef));
                    });

                EventsHelper.Fire(_sendToTranscriptionEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate()
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
                    });
            }
        }

        public void Save()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        SaveReportResponse response = service.SaveReport(new SaveReportRequest(_reportingStepRef, this.ReportContent));
                    });

                EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToSaveReport, this.Host.DesktopWindow,
                    delegate()
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
                    });
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
        }

        #endregion

        public string GetData(string tag)
        {
            switch (tag)
            {
                case "Report":
                    ReportPartSummary reportPart = _report.GetPart(0);
                    return reportPart == null ? "" : reportPart.Content;
                case "Addendum":
                    ReportPartSummary addendumPart = _report.GetPart(_reportPartIndex);
                    return addendumPart == null ? "" : addendumPart.Content;
                case "Preview":
                default:
                    return JsmlSerializer.Serialize<ReportSummary>(_report);
            }
        }

        public void SetData(string tag, string data)
        {
            switch (tag)
            {
                case "Report":
                case "Addendum":
                    this.ReportContent = data;
                    break;
                default:
                    break;
            }
        }
    }
}
