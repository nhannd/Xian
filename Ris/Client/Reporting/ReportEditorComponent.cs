#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Collections;
using System.Runtime.InteropServices;
using System.Threading;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.Jsml;
using ClearCanvas.Ris.Application.Common.Admin;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

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
            private readonly ReportEditorComponent _component;

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

            public string FormatPersonName(string jsml)
            {
                PersonNameDetail detail = JsmlSerializer.Deserialize<PersonNameDetail>(jsml);
                return detail == null ? "" : PersonNameFormat.Format(detail);
            }
        }

        private readonly ScriptCallback _scriptCallback;

        private readonly EntityRef _reportingStepRef;
        private int _reportPartIndex;
        private ReportSummary _report;

        private bool _canCompleteInterpretationAndVerify;
        private bool _canCompleteVerification;
        private bool _canCompleteInterpretationForVerification;
        private bool _canCompleteInterpretationForTranscription;

        private event EventHandler _verifyEvent;
        private event EventHandler _sendToVerifyEvent;
        private event EventHandler _sendToTranscriptionEvent;
        private event EventHandler _saveEvent;
        private event EventHandler _closeComponentEvent;

        private string _reportContent;
        private StaffSummary _supervisor;

        private ILookupHandler _supervisorLookupHandler;

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
                    ReportPartSummary part = _report.GetPart(_reportPartIndex);
                    if (part != null)
                        _supervisor = part.Supervisor;

                    // For resident, look for the default supervisor if it does not already exist
                    if (_supervisor == null && String.IsNullOrEmpty(SupervisorSettings.Default.SupervisorID) == false)
                    {
                        GetRadiologistListResponse getRadListresponse = service.GetRadiologistList(new GetRadiologistListRequest(SupervisorSettings.Default.SupervisorID));
                        _supervisor = CollectionUtils.FirstElement<StaffSummary>(getRadListresponse.Radiologists);
                    }
                });

            _supervisorLookupHandler = new StaffLookupHandler(this.Host.DesktopWindow, new string[] { "PRAD" });

            base.Start();
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
            get { return VisitNumberFormat.Format(_report.VisitNumber); }
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
            get { return "TODO"; }
        }

        public string PerformedLocation
        {
            get { return _report.PerformedLocation; }
        }

        public string PerformedDate
        {
            get { return "TODO"; }
        }

        public bool VerifyEnabled
        {
            get { return (_canCompleteInterpretationAndVerify || _canCompleteVerification); }
        }

        public bool SendToVerifyEnabled
        {
            get { return _canCompleteInterpretationForVerification; }
        }

        public bool SendToTranscriptionEnabled
        {
            get { return _canCompleteInterpretationForTranscription; }
        }

        public bool CanSendToTranscription
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UseTranscriptionWorkflow); }
        }

        public bool CanVerifyReport
        {
            get { return Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport); }
        }

        public StaffSummary Supervisor
        {
            get { return _supervisor; }
            set
            {
                if (!Equals(value, _supervisor))
                {
                    _supervisor = value;
                    SaveSupervisor();
                }
            }
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
                            service.CompleteInterpretationAndVerify(
                                new CompleteInterpretationAndVerifyRequest(
                                _reportingStepRef, 
                                this.ReportContent,
                                _supervisor == null ? null : _supervisor.StaffRef,
                                null));
                        });
                }
                else if (_canCompleteVerification)
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteVerification(new CompleteVerificationRequest(_reportingStepRef, this.ReportContent));
                        });
                }

                EventsHelper.Fire(_verifyEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate
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
                if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport) == false && _supervisor == null)
                {
                    this.Host.DesktopWindow.ShowMessageBox(SR.MessageChooseRadiologist, MessageBoxActions.Ok);
                    return;
                }

                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForVerification(
                            new CompleteInterpretationForVerificationRequest(
                            _reportingStepRef, 
                            this.ReportContent,
                            _supervisor == null ? null : _supervisor.StaffRef,
                            null));
                    });

                EventsHelper.Fire(_sendToVerifyEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate
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
                        service.CompleteInterpretationForTranscription(
                            new CompleteInterpretationForTranscriptionRequest(
                            _reportingStepRef, 
                            this.ReportContent,
                            _supervisor == null ? null : _supervisor.StaffRef,
                            null));
                    });

                EventsHelper.Fire(_sendToTranscriptionEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate
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
                        service.SaveReport(
                            new SaveReportRequest(
                            _reportingStepRef, 
                            this.ReportContent,
                            _supervisor == null ? null : _supervisor.StaffRef,
                            null));
                    });

                EventsHelper.Fire(_saveEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, SR.ExceptionFailedToSaveReport, this.Host.DesktopWindow,
                    delegate
                    {
                        this.ExitCode = ApplicationComponentExitCode.Error;
                        EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
                    });
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.None;
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
                    return JsmlSerializer.Serialize(_report, "report");
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

        private void SaveSupervisor()
        {
            if (_supervisor != null)
            {
                SupervisorSettings.Default.SupervisorID = _supervisor.StaffId;
                SupervisorSettings.Default.Save();
            }
        }
    }
}
