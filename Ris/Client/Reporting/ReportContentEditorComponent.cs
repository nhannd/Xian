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
        private bool _canCompleteInterpretationAndVerify;
        private bool _canCompleteVerification;
        private bool _canCompleteInterpretationForVerification;
        private bool _canCompleteInterpretationForTranscription;

        private IEnumerable _reportingFolders;

        private event EventHandler _closeComponentRequest;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportContentEditorComponent(ReportingWorklistItem item, string reportContent, IEnumerable folders)
        {
            _worklistItem = item;
            _reportContent = reportContent;
            _reportingFolders = folders;
        }

        public override void Start()
        {
            try
            {
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        GetOperationEnablementResponse response = service.GetOperationEnablement(new GetOperationEnablementRequest(_worklistItem));
                        _canCompleteInterpretationAndVerify = response.OperationEnablementDictionary["CompleteInterpretationAndVerify"];
                        _canCompleteVerification = response.OperationEnablementDictionary["CompleteVerification"];
                        _canCompleteInterpretationForVerification = response.OperationEnablementDictionary["CompleteInterpretationForVerification"];
                        _canCompleteInterpretationForTranscription = response.OperationEnablementDictionary["CompleteInterpretationForTranscription"];
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

        public event EventHandler CloseComponentRequest
        {
            add { _closeComponentRequest += value; }
            remove { _closeComponentRequest -= value; }
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

                IFolder myVerifiedFolder = CollectionUtils.SelectFirst<IFolder>(_reportingFolders,
                    delegate(IFolder f) { return f is Folders.MyVerifiedFolder; });
                myVerifiedFolder.RefreshCount();

                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Normal;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            EventsHelper.Fire(_closeComponentRequest, this, EventArgs.Empty);
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

                IFolder myVerificationFolder = CollectionUtils.SelectFirst<IFolder>(_reportingFolders,
                    delegate(IFolder f) { return f is Folders.MyVerificationFolder; });
                myVerificationFolder.RefreshCount();

                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Error;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            EventsHelper.Fire(_closeComponentRequest, this, EventArgs.Empty);
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

                IFolder myTranscriptionFolder = CollectionUtils.SelectFirst<IFolder>(_reportingFolders,
                    delegate(IFolder f) { return f is Folders.MyTranscriptionFolder; });
                myTranscriptionFolder.RefreshCount();

                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Error;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            EventsHelper.Fire(_closeComponentRequest, this, EventArgs.Empty);
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
            }
            catch (Exception e)
            {
                this.ExitCode = ApplicationComponentExitCode.Error;
                ExceptionHandler.Report(e, this.Host.DesktopWindow);
            }

            EventsHelper.Fire(_closeComponentRequest, this, EventArgs.Empty);
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            EventsHelper.Fire(_closeComponentRequest, this, EventArgs.Empty);
        }
    }
}
