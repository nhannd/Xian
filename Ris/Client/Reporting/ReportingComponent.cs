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
using System.Collections.Generic;
using System.Threading;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using AuthorityTokens=ClearCanvas.Ris.Application.Common.AuthorityTokens;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// Defines an interface for providing a custom report editor.
    /// </summary>
    public interface IReportEditorProvider
    {
        IReportEditor GetEditor(IReportingContext context);
    }

    /// <summary>
    /// Defines an interface for providing a custom report editor page with access to the reporting
    /// context.
    /// </summary>
    public interface IReportingContext
    {
        /// <summary>
        /// Gets the reporting worklist item.
        /// </summary>
        ReportingWorklistItem WorklistItem { get; }

        /// <summary>
        /// Gets the report associated with the worklist item.
        /// </summary>
        ReportDetail Report { get; }

        /// <summary>
        /// Gets the index of the active report part (the part that is being edited).
        /// </summary>
        int ActiveReportPartIndex { get; }

        /// <summary>
        /// Gets a value indicating whether the Verify operation is enabled.
        /// </summary>
        bool CanVerify { get; }

        /// <summary>
        /// Gets a value indicating whether the Send To Verify operation is enabled.
        /// </summary>
        bool CanSendToBeVerified { get; }

        /// <summary>
        /// Gets a value indicating whether the Send To Transcription operation is enabled.
        /// </summary>
        bool CanSendToTranscription { get; }

        /// <summary>
        /// Gets or sets the report content for the active report part.
        /// </summary>
        string ReportContent { get; set; }

        /// <summary>
        /// Gets or sets the supervisor for the active report part.
        /// </summary>
        StaffSummary Supervisor { get; set; }

        /// <summary>
        /// Notifies that the editor requests to verify the report.
        /// </summary>
        void VerifyReport();

        /// <summary>
        /// Notifies that the editor requests to send the report to be verified.
        /// </summary>
        void SendToBeVerified();

        /// <summary>
        /// Notifies that the editor requests to send the report to transcription.
        /// </summary>
        void SendToTranscription();

        /// <summary>
        /// Notifies that the editor requests to save the report.
        /// </summary>
        void SaveReport();

        /// <summary>
        /// Notifies that the editor requests to cancel.
        /// </summary>
        void CancelEditing();
    }

    /// <summary>
    /// Defines an interface to a custom report editor.
    /// </summary>
    public interface IReportEditor
    {
        IApplicationComponent GetComponent();
    }

    /// <summary>
    /// Defines an extension point for providing a custom report editor.
    /// </summary>
    [ExtensionPoint]
    public class ReportEditorProviderExtensionPoint : ExtensionPoint<IReportEditorProvider>
    {
    }

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
        #region IReportingContext implementation

        class ReportingContext : IReportingContext
        {
            private readonly ReportingComponent _owner;

            public ReportingContext(ReportingComponent owner)
            {
                _owner = owner;
            }

            public ReportingWorklistItem WorklistItem
            {
                get { return _owner._worklistItem; }
            }

            public ReportDetail Report
            {
                get { return _owner._report; }
            }

            public int ActiveReportPartIndex
            {
                get { return _owner._activeReportPartIndex; }
            }

            public bool CanVerify
            {
                get { return _owner.CanVerify; }
            }

            public bool CanSendToBeVerified
            {
                get { return _owner.CanSendToBeVerified; }
            }

            public bool CanSendToTranscription
            {
                get { return _owner.CanSendToTranscription; }
            }

            public string ReportContent
            {
                get { return _owner._reportContent; }
                set { _owner._reportContent = value; }
            }

            public StaffSummary Supervisor
            {
                get { return _owner._supervisor; }
                set
                {
                    _owner.SetSupervisor(value);
                }
            }

            public void VerifyReport()
            {
                _owner.Verify();
            }

            public void SendToBeVerified()
            {
                _owner.SendToBeVerified();
            }

            public void SendToTranscription()
            {
                _owner.SendToTranscription();
            }

            public void SaveReport()
            {
                _owner.Save();
            }

            public void CancelEditing()
            {
                _owner.Cancel();
            }
        }

        #endregion

        private readonly ReportingWorklistItem _worklistItem;

        private BannerComponent _bannerComponent;
        private ChildComponentHost _bannerHost;

        private IReportEditor _reportEditor;
        private ChildComponentHost _reportEditorHost;

        private PriorReportComponent _priorReportComponent;
        private ChildComponentHost _priorReportHost;

        private OrderDetailViewComponent _orderDetailComponent;
        private ChildComponentHost _orderDetailHost;

        private bool _canCompleteInterpretationAndVerify;
        private bool _canCompleteVerification;
        private bool _canCompleteInterpretationForVerification;
        private bool _canCompleteInterpretationForTranscription;

        private ReportDetail _report;
        private int _activeReportPartIndex;
    	private Dictionary<string, string> _extendedProperties;
        private StaffSummary _supervisor;
        private string _reportContent;

    	/// <summary>
        /// Constructor
        /// </summary>
        public ReportingComponent(ReportingWorklistItem worklistItem)
        {
            _worklistItem = worklistItem;


        }

        public override void Start()
        {
            _bannerComponent = new BannerComponent(_worklistItem);
            _bannerHost = new ChildComponentHost(this.Host, _bannerComponent);
            _bannerHost.StartComponent();

            _priorReportComponent = new PriorReportComponent(_worklistItem);
            _priorReportHost = new ChildComponentHost(this.Host, _priorReportComponent);
            _priorReportHost.StartComponent();

            _orderDetailComponent = new OrderDetailViewComponent(_worklistItem);
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
                    _activeReportPartIndex = response.ReportPartIndex;

                    ReportPartDetail activePart = _report.GetPart(_activeReportPartIndex);
                    _reportContent = activePart == null ? null : activePart.Content;
                    if(activePart != null && activePart.Supervisor != null)
                    {
                        // active part already has a supervisor assigned
                        _supervisor = activePart.Supervisor;
                    }
                    else
                    {
                        // active part does not have a supervisor assigned
                        // if this user has a default supervisor, retreive it, otherwise leave supervisor as null
                        if (!String.IsNullOrEmpty(SupervisorSettings.Default.SupervisorID))
                        {
                            GetRadiologistListResponse getRadListresponse = service.GetRadiologistList(new GetRadiologistListRequest(SupervisorSettings.Default.SupervisorID));
                            _supervisor = CollectionUtils.FirstElement(getRadListresponse.Radiologists);
                        }
                    }

                    //TODO: look at this
                    ListProcedureExtendedPropertiesResponse extendedPropertiesResponse = service.ListProcedureExtendedProperties(new ListProcedureExtendedPropertiesRequest(_worklistItem.ProcedureRef));
                    _extendedProperties = CollectionUtils.FirstElement(extendedPropertiesResponse.ProcedureExtendedProperties);
                });

            // check for a report editor provider.  If not found, use the default one
            IReportEditorProvider provider = CollectionUtils.FirstElement<IReportEditorProvider>(
                                                    new ReportEditorProviderExtensionPoint().CreateExtensions());

            _reportEditor = provider == null ? new ReportEditorComponent(new ReportingContext(this)) : provider.GetEditor(new ReportingContext(this));
            _reportEditorHost = new ChildComponentHost(this.Host, _reportEditor.GetComponent());
            _reportEditorHost.StartComponent();

            base.Start();
        }

        public override void Stop()
        {
            if(_reportEditor != null && _reportEditor is IDisposable)
            {
                ((IDisposable)_reportEditor).Dispose();
                _reportEditor = null;
            }
            
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

        private bool CanVerify
        {
            get { return (_canCompleteInterpretationAndVerify || _canCompleteVerification) && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.VerifyReport); }
        }

        private bool CanSendToBeVerified
        {
            get { return _canCompleteInterpretationForVerification; }
        }

        private bool CanSendToTranscription
        {
            get { return _canCompleteInterpretationForTranscription && Thread.CurrentPrincipal.IsInRole(AuthorityTokens.UseTranscriptionWorkflow); }
        }

        private void Verify()
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
                                _reportContent,
                                _supervisor == null ? null : _supervisor.StaffRef));
                        });
                }
                else if (_canCompleteVerification)
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteVerification(new CompleteVerificationRequest(_worklistItem.ProcedureStepRef, _reportContent));
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

        private void SendToBeVerified()
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
                            _worklistItem.ProcedureStepRef,
                            _reportContent,
                            _supervisor == null ? null : _supervisor.StaffRef));
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

        private void SendToTranscription()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForTranscription(
                            new CompleteInterpretationForTranscriptionRequest(
                            _worklistItem.ProcedureStepRef,
                            _reportContent,
                            _supervisor == null ? null : _supervisor.StaffRef));
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

        private void Save()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.SaveReport(
                            new SaveReportRequest(
                            _worklistItem.ProcedureStepRef,
                            _reportContent,
                            _supervisor == null ? null : _supervisor.StaffRef));
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

        private void Cancel()
        {
            this.Exit(ApplicationComponentExitCode.None);
        }

        #endregion

        private void SetSupervisor(StaffSummary supervisor)
        {
            _supervisor = supervisor;
            SupervisorSettings.Default.SupervisorID = supervisor == null ? "" : supervisor.StaffId;
            SupervisorSettings.Default.Save();
        }
    }
}
