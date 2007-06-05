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
    /// Extension point for views onto <see cref="InterpretationComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class InterpretationComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// InterpretationComponent class
    /// </summary>
    [AssociateView(typeof(InterpretationComponentViewExtensionPoint))]
    public class InterpretationComponent : ApplicationComponent
    {
        private ReportingWorklistItem _worklistItem;
        private string _reportContent;

        private IEnumerable _reportingFolders;

        /// <summary>
        /// Constructor
        /// </summary>
        public InterpretationComponent(ReportingWorklistItem item, string reportContent, IEnumerable folders)
        {
            _worklistItem = item;
            _reportContent = reportContent;
            _reportingFolders = folders;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
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
                Platform.GetService<IReportingWorkflowService>(
                    delegate(IReportingWorkflowService service)
                    {
                        service.CompleteInterpretationAndVerify(new CompleteInterpretationAndVerifyRequest(_worklistItem));
                    });

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

            Host.Exit();
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

            Host.Exit();        
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

            Host.Exit();
        }

        public void Save()
        {
            Platform.ShowMessageBox("Save not implemented");

            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }
    }
}
