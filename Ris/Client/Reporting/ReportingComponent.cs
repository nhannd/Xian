#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="ReportingComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportingComponent class
    /// </summary>
    [AssociateView(typeof(ReportingComponentViewExtensionPoint))]
    public class ReportingComponent : ApplicationComponent
    {

        private readonly BannerComponent _bannerComponent;
        private ChildComponentHost _bannerHost;

        private readonly IReportEditor _reportEditor;
        private ChildComponentHost _reportEditorHost;

        private readonly PriorReportComponent _priorReportComponent;
        private ChildComponentHost _priorReportHost;

        private readonly OrderDetailViewComponent _orderDetailComponent;
        private ChildComponentHost _orderDetailHost;

        private readonly ReportingWorklistItem _worklistItem;

        private bool _canCompleteInterpretationAndVerify;
        private bool _canCompleteVerification;
        private bool _canCompleteInterpretationForVerification;
        private bool _canCompleteInterpretationForTranscription;

        private ReportDetail _report;
        private ReportPartDetail _reportPart;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportingComponent(ReportingWorklistItem worklistItem)
        {
            _worklistItem = worklistItem;

            _bannerComponent = new BannerComponent(_worklistItem);

            // Look for any report editor extensions.  If not found, use the default one
            ReportEditorExtensionPoint reportEditorExtensionPoint = new ReportEditorExtensionPoint();
            _reportEditor = CollectionUtils.FirstElement<IReportEditor>(reportEditorExtensionPoint.CreateExtensions());
            if (_reportEditor == null)
                _reportEditor = new ReportEditorComponent();

            _priorReportComponent = new PriorReportComponent(_worklistItem);
            _orderDetailComponent = new OrderDetailViewComponent(_worklistItem);
        }

        public override void Start()
        {
            _bannerHost = new ChildComponentHost(this.Host, _bannerComponent);
            _bannerHost.StartComponent();

            _reportEditorHost = new ChildComponentHost(this.Host, _reportEditor);
            _reportEditorHost.StartComponent();

            _priorReportHost = new ChildComponentHost(this.Host, _priorReportComponent);
            _priorReportHost.StartComponent();

            _orderDetailHost = new ChildComponentHost(this.Host, _orderDetailComponent);
            _orderDetailHost.StartComponent();

            Platform.GetService<IReportingWorkflowService>(
                delegate(IReportingWorkflowService service)
                {
                    GetOperationEnablementResponse enablementResponse = service.GetOperationEnablement(new GetOperationEnablementRequest(_worklistItem.ProcedureStepRef));
                    _canCompleteInterpretationAndVerify = enablementResponse.OperationEnablementDictionary["CompleteInterpretationAndVerify"];
                    _canCompleteVerification = enablementResponse.OperationEnablementDictionary["CompleteVerification"];
                    _canCompleteInterpretationForVerification = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForVerification"];
                    _canCompleteInterpretationForTranscription = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForTranscription"];

                    LoadReportForEditResponse response = service.LoadReportForEdit(new LoadReportForEditRequest(_worklistItem.ProcedureStepRef));
                    _report = response.Report;
                    _reportPart = _report.GetPart(response.ReportPartIndex);

                    // For resident, look for the default supervisor if it does not already exist
                    if (_reportPart != null && _reportPart.Supervisor == null && String.IsNullOrEmpty(SupervisorSettings.Default.SupervisorID) == false)
                    {
                        GetRadiologistListResponse getRadListresponse = service.GetRadiologistList(new GetRadiologistListRequest(SupervisorSettings.Default.SupervisorID));
                        _reportPart.Supervisor = CollectionUtils.FirstElement(getRadListresponse.Radiologists);
                    }
                });

            SyncReportEditorData(true);

            base.Start();
        }

        public override void Stop()
        {
            SyncReportEditorData(false);
            
            base.Stop();
        }

        #region Presentation Model

        public ApplicationComponentHost BannerHost
        {
            get { return _bannerHost; }
        }

        public ApplicationComponentHost ReportEditorHost
        {
            get { return _reportEditorHost; }
        }

        public ApplicationComponentHost PriorReportsHost
        {
            get { return _priorReportHost; }
        }

        public ApplicationComponentHost OrderDetailsHost
        {
            get { return _orderDetailHost; }
        }

        #endregion

        #region Private Event Handlers

        void _reportEditorComponent_OnVerifyRequested(object sender, EventArgs e)
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
                                _worklistItem.ProcedureStepRef,
                                _reportEditor.ReportPart.Content,
                                _reportEditor.ReportPart.Supervisor == null ? null : _reportEditor.ReportPart.Supervisor.StaffRef));
                        });
                }
                else if (_canCompleteVerification)
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteVerification(new CompleteVerificationRequest(_worklistItem.ProcedureStepRef, _reportEditor.ReportPart.Content));
                        });
                }

                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate
                    {
                        this.Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        void _reportEditorComponent_OnSendToVerifyRequested(object sender, EventArgs e)
        {
            try
            {
                if (Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport) == false && _reportEditor.ReportPart.Supervisor == null)
                {
                    this.Host.DesktopWindow.ShowMessageBox(SR.MessageChooseRadiologist, MessageBoxActions.Ok);
                    return;
                }

                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForVerification(
                            new CompleteInterpretationForVerificationRequest(
                            _worklistItem.ProcedureStepRef,
                            _reportEditor.ReportPart.Content,
                            _reportEditor.ReportPart.Supervisor == null ? null : _reportEditor.ReportPart.Supervisor.StaffRef));
                    });

                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate
                    {
                        this.Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        void _reportEditorComponent_OnSendToTranscriptionRequested(object sender, EventArgs e)
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForTranscription(
                            new CompleteInterpretationForTranscriptionRequest(
                            _worklistItem.ProcedureStepRef,
                            _reportEditor.ReportPart.Content,
                            _reportEditor.ReportPart.Supervisor == null ? null : _reportEditor.ReportPart.Supervisor.StaffRef));
                    });

                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.InTranscriptionFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Report(ex, SR.ExceptionFailedToPerformOperation, this.Host.DesktopWindow,
                    delegate
                    {
                        this.Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        void _reportEditorComponent_OnSaveRequested(object sender, EventArgs e)
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.SaveReport(
                            new SaveReportRequest(
                            _worklistItem.ProcedureStepRef,
                            _reportEditor.ReportPart.Content,
                            _reportEditor.ReportPart.Supervisor == null ? null : _reportEditor.ReportPart.Supervisor.StaffRef));
                    });

                // Source Folders
                //DocumentManager.InvalidateFolder(typeof(Folders.ToBeReportedFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.VerifiedFolder));
                // Destination Folders
                DocumentManager.InvalidateFolder(typeof(Folders.DraftFolder));
                DocumentManager.InvalidateFolder(typeof(Folders.ToBeVerifiedFolder));
                this.Exit(ApplicationComponentExitCode.Accepted);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Report(ex, SR.ExceptionFailedToSaveReport, this.Host.DesktopWindow,
                    delegate
                    {
                        this.Exit(ApplicationComponentExitCode.Error);
                    });
            }
        }

        void _reportEditorComponent_OnCancelRequested(object sender, EventArgs e)
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void SyncReportEditorData(bool set)
        {
            if (set)
            {
                _reportEditor.Report = _report;
                _reportEditor.ReportPart = _reportPart;
                _reportEditor.WorklistItem = _worklistItem;

                _reportEditor.IsEditingAddendum = _reportPart.Index > 0;
                _reportEditor.VerifyEnabled = _canCompleteInterpretationAndVerify || _canCompleteVerification;
                _reportEditor.SendToVerifyEnabled = _canCompleteInterpretationForVerification;
                _reportEditor.SendToTranscriptionEnabled = _canCompleteInterpretationForTranscription;

                // Setup the various editor closed events.  Do not invalidate the ToBeReported folder type, since it's communual
                _reportEditor.VerifyRequested += _reportEditorComponent_OnVerifyRequested;
                _reportEditor.SendToVerifyRequested += _reportEditorComponent_OnSendToVerifyRequested;
                _reportEditor.SendToTranscriptionRequested += _reportEditorComponent_OnSendToTranscriptionRequested;
                _reportEditor.SaveRequested += _reportEditorComponent_OnSaveRequested;
                _reportEditor.CancelRequested += _reportEditorComponent_OnCancelRequested;
            }
            else // get
            {
                _reportPart = _reportEditor.ReportPart;
                SaveSupervisor(_reportEditor.ReportPart.Supervisor);

                _reportEditor.VerifyRequested -= _reportEditorComponent_OnVerifyRequested;
                _reportEditor.SendToVerifyRequested -= _reportEditorComponent_OnSendToVerifyRequested;
                _reportEditor.SendToTranscriptionRequested -= _reportEditorComponent_OnSendToTranscriptionRequested;
                _reportEditor.SaveRequested -= _reportEditorComponent_OnSaveRequested;
                _reportEditor.CancelRequested -= _reportEditorComponent_OnCancelRequested;
            }
        }

        private void SaveSupervisor(StaffSummary supervisor)
        {
            SupervisorSettings.Default.SupervisorID = supervisor == null ? "" : supervisor.StaffId;
            SupervisorSettings.Default.Save();
        }
    }
}
