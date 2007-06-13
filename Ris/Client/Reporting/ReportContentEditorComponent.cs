using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Reporting
{
    /// <summary>
    /// Extension point for views onto <see cref="ReportContentEditorComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportContentEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportContentEditorComponent class
    /// </summary>
    [AssociateView(typeof(ReportContentEditorComponentViewExtensionPoint))]
    public class ReportContentEditorComponent : ApplicationComponent
    {

        private ReportingWorklistItem _worklistItem;
        private string _reportContent;
        private bool _readOnly;

        private bool _canCompleteInterpretationAndVerify;
        private bool _canCompleteVerification;
        private bool _canCompleteInterpretationForVerification;
        private bool _canCompleteInterpretationForTranscription;

        private event EventHandler _verifyEvent;
        private event EventHandler _sendToVerifyEvent;
        private event EventHandler _sendToTranscriptionEvent;
        private event EventHandler _closeComponentEvent;

        public ReportContentEditorComponent(ReportingWorklistItem item)
            : this(item, false)
        {
        }

        public ReportContentEditorComponent(ReportingWorklistItem item, bool readOnly)
        {
            _worklistItem = item;
            _readOnly = readOnly;
        }

        public override void Start()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        if (_readOnly == false)
                        {
                            GetOperationEnablementResponse enablementResponse = service.GetOperationEnablement(new GetOperationEnablementRequest(_worklistItem));
                            _canCompleteInterpretationAndVerify = enablementResponse.OperationEnablementDictionary["CompleteInterpretationAndVerify"];
                            _canCompleteVerification = enablementResponse.OperationEnablementDictionary["CompleteVerification"];
                            _canCompleteInterpretationForVerification = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForVerification"];
                            _canCompleteInterpretationForTranscription = enablementResponse.OperationEnablementDictionary["CompleteInterpretationForTranscription"];
                        }

                        LoadReportForEditResponse response = service.LoadReportForEdit(new LoadReportForEditRequest(_worklistItem));
                        _reportContent = response.ReportContent;
                    });
            }
            catch (Exception e)
            {
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
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

        public event EventHandler CloseComponentEvent
        {
            add { _closeComponentEvent += value; }
            remove { _closeComponentEvent -= value; }
        }

        #region Presentation Model

        public string PatientName
        {
            get { return PersonNameFormat.Format(_worklistItem.PersonNameDetail); }
        }

        public string Mrn
        {
            get { return MrnFormat.Format(_worklistItem.Mrn); }
        }

        public string AccessionNumber
        {
            get { return _worklistItem.AccessionNumber; }
        }

        public string DiagnosticService
        {
            get { return _worklistItem.DiagnosticServiceName; }
        }

        public string RequestedProcedure
        {
            get { return _worklistItem.RequestedProcedureName; }
        }

        public string Priority
        {
            get { return _worklistItem.Priority; }
        }

        public string Report
        {
            get { return _reportContent; }
            set 
            {
                _reportContent = value;
                this.Modified = true;
            }
        }

        public bool ReadOnly
        {
            get { return _readOnly; }
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

        public bool SaveEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler SaveEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }

        #endregion

        public void Verify()
        {
            try
            {
                if (_canCompleteInterpretationAndVerify)
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteInterpretationAndVerify(new CompleteInterpretationAndVerifyRequest(_worklistItem));
                        });
                }
                else if (_canCompleteVerification)
                {
                    Platform.GetService<IReportingWorkflowService>(
                        delegate(IReportingWorkflowService service)
                        {
                            service.CompleteVerification(new CompleteVerificationRequest(_worklistItem));
                        });
                }

                this.ExitCode = ApplicationComponentExitCode.Normal;
                EventsHelper.Fire(_verifyEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Normal;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void SendToVerify()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForVerification(new CompleteInterpretationForVerificationRequest(_worklistItem));
                    });

                this.ExitCode = ApplicationComponentExitCode.Normal;
                EventsHelper.Fire(_sendToVerifyEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Error;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void SendToTranscription()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationForTranscription(new CompleteInterpretationForTranscriptionRequest(_worklistItem));
                    });

                this.ExitCode = ApplicationComponentExitCode.Normal;
                EventsHelper.Fire(_sendToTranscriptionEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Error;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

        }

        public void Save()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        SaveReportResponse response = service.SaveReport(new SaveReportRequest(_worklistItem.ProcedureStepRef, this.Report));
                    });

                this.ExitCode = ApplicationComponentExitCode.Normal;
                EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Error;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            EventsHelper.Fire(_closeComponentEvent, this, EventArgs.Empty);
        }
    }
}
