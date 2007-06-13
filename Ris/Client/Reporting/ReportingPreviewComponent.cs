using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;
using ClearCanvas.Common.Scripting;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Enterprise.Common;
using ClearCanvas.Ris.Client;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Application.Common.ReportingWorkflow;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Reporting
{
    [ExtensionPoint]
    public class ReportingPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IReportingPreviewToolContext : IToolContext
    {
        ReportingWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="ReportingPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class ReportingPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// ReportingPreviewComponent class
    /// </summary>
    [AssociateView(typeof(ReportingPreviewComponentViewExtensionPoint))]
    public class ReportingPreviewComponent : HtmlApplicationComponent
    {
        class ReportingPreviewToolContext : ToolContext, IReportingPreviewToolContext
        {
            private ReportingPreviewComponent _component;
            public ReportingPreviewToolContext(ReportingPreviewComponent component)
            {
                _component = component;
            }

            public ReportingWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private ReportingWorklistItem _worklistItem;
        private ReportingWorklistPreview _worklistPreview;

        private ToolSet _toolSet;

        private BackgroundTask _previewLoadTask;

        /// <summary>
        /// Constructor
        /// </summary>
        public ReportingPreviewComponent()
        {
        }

        public ReportingWorklistItem WorklistItem
        {
            get { return _worklistItem; }
            set
            {
                _worklistItem = value;
                if (this.IsStarted)
                {
                    UpdateDisplay();
                }
            }
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new ReportingPreviewToolExtensionPoint(), new ReportingPreviewToolContext(this));

            UpdateDisplay();
            
            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        private void UpdateDisplay()
        {
            // if there is a preview showing, clear it
            if (_worklistPreview != null)
            {
                _worklistPreview = null;


                // clear current preview
                NotifyAllPropertiesChanged();
            }

            if (_worklistItem != null && _worklistItem.ProcedureStepRef != null)
            {
                LoadPreviewAsync(_worklistItem);
            }
        }

        private void LoadPreviewAsync(ReportingWorklistItem item)
        {
            // remove any previous task
            if (_previewLoadTask != null)
            {
                // important to unsubscribe - in case the previous task is still running, we don't want to receive events from it anymore
                _previewLoadTask.Terminated -= OnPreviewLoaded;
                _previewLoadTask.Dispose();
                _previewLoadTask = null;
            }

            // create a background task to load the preview
            _previewLoadTask = new BackgroundTask(
                delegate(IBackgroundTaskContext taskContext)
                {
                    try
                    {
                        ReportingWorklistItem worklistItem = (ReportingWorklistItem)taskContext.UserState;
                        Platform.GetService<IReportingWorkflowService>(
                            delegate(IReportingWorkflowService service)
                            {
                                LoadWorklistPreviewResponse response = service.LoadWorklistPreview(new LoadWorklistPreviewRequest(worklistItem));
                                taskContext.Complete(response.WorklistPreview);
                            });

                    }
                    catch (Exception e)
                    {
                        taskContext.Error(e);
                    }
                },
                false, item);

            _previewLoadTask.Terminated += OnPreviewLoaded;
            _previewLoadTask.Run();
        }

        private void OnPreviewLoaded(object sender, BackgroundTaskTerminatedEventArgs args)
        {
            if (args.Reason == BackgroundTaskTerminatedReason.Completed)
            {
                _worklistPreview = (ReportingWorklistPreview)args.Result;


                NotifyAllPropertiesChanged();
            }
            else if (args.Reason == BackgroundTaskTerminatedReason.Exception)
            {
                ExceptionHandler.Report(args.Exception, this.Host.DesktopWindow);
            }
        }

        #region Presentation Model

        public ActionModelNode MenuModel
        {
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "ReportingPreview-menu", _toolSet.Actions); }
        }

        public ReportingWorklistPreview WorklistPreview
        {
            get { return _worklistPreview; }
        }

        public string Name
        {
            get { return PersonNameFormat.Format(_worklistPreview.Name); }
        }

        public string Mrn
        {
            get { return MrnFormat.Format(_worklistPreview.Mrn); }
        }

        public string DateOfBirth
        {
            get { return Format.Date(_worklistPreview.DateOfBirth); }
        }

        public string Sex
        {
            get { return _worklistPreview.Sex; }
        }

        public string AccessionNumber
        {
            get { return _worklistPreview.AccessionNumber; }
        }

        public string RequestedProcedureName
        {
            get { return _worklistPreview.RequestedProcedureName; }
        }

        public string VisitNumber
        {
            get { return String.Format("{0} {1}", _worklistPreview.VisitNumberAssigningAuthority, _worklistPreview.VisitNumberId); }
        }

        #endregion
    }
}
