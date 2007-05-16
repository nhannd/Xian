using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Ris.Application.Common.ModalityWorkflow;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Ris.Application.Common;
using ClearCanvas.Ris.Client.Formatting;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class TechnologistPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface ITechnologistPreviewToolContext : IToolContext
    {
        ModalityWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }

    /// <summary>
    /// Extension point for views onto <see cref="TechnologistPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class TechnologistPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// TechnologistPreviewComponent class
    /// </summary>
    [AssociateView(typeof(TechnologistPreviewComponentViewExtensionPoint))]
    public class TechnologistPreviewComponent : ApplicationComponent
    {
        class TechnologistPreviewToolContext : ToolContext, ITechnologistPreviewToolContext
        {
            private TechnologistPreviewComponent _component;

            public TechnologistPreviewToolContext(TechnologistPreviewComponent component)
            {
                _component = component;
            }

            #region ITechnologistPreviewToolContext Members

            public ModalityWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }

            #endregion
        }

        private bool _showHeader;
        private bool _showReconciliationAlert;

        private ModalityWorklistItem _worklistItem;
        private ModalityWorklistPreview _worklistPreview;

        private ToolSet _toolSet;

        private BackgroundTask _previewLoadTask;

        /// <summary>
        /// Constructor
        /// </summary>
        /// 
        public TechnologistPreviewComponent()
            : this(true, true)
        {
        }

        public TechnologistPreviewComponent(bool showHeader, bool showReconciliationAlert)
        {
            _showHeader = showHeader;
            _showReconciliationAlert = showReconciliationAlert;
        }

        public override void Start()
        {
            _toolSet = new ToolSet(new TechnologistPreviewToolExtensionPoint(), new TechnologistPreviewToolContext(this));

            UpdateDisplay();

            base.Start();
        }

        public override void Stop()
        {
            _toolSet.Dispose();

            base.Stop();
        }

        public ModalityWorklistItem WorklistItem
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

        private void LoadPreviewAsync(ModalityWorklistItem item)
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
                        ModalityWorklistItem worklistItem = (ModalityWorklistItem)taskContext.UserState;
                        Platform.GetService<IModalityWorkflowService>(
                            delegate(IModalityWorkflowService service)
                            {
                                LoadWorklistItemPreviewResponse response = service.LoadWorklistItemPreview(new LoadWorklistItemPreviewRequest(worklistItem.ProcedureStepRef, worklistItem.MrnAssigningAuthority));
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
                _worklistPreview = (ModalityWorklistPreview)args.Result;

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
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "TechnologistPreview-menu", _toolSet.Actions); }
        }
        
        public ModalityWorklistPreview WorklistPreview
        {
            get { return _worklistPreview; }
        }

        public bool ShowHeader
        {
            get { return _showHeader; }
            set { _showHeader = value; }
        }

        public bool ShowReconciliationAlert
        {
            get
            {
                if (_showReconciliationAlert && _worklistPreview != null)
                {
                    return _worklistPreview.HasReconciliationCandidates;
                }
                return false;
            }
        }

        public string ReconciliationMessage
        {
            get { return String.Format(SR.MessageUnreconciledRecordsAlert, PersonNameFormat.Format(_worklistPreview.Name, "%g. %F")); }
        }

        public string Name
        {
            get { return PersonNameFormat.Format(_worklistPreview.Name); }
        }

        public string DateOfBirth
        {
            get { return Format.Date(_worklistPreview.DateOfBirth); }
        }

        public string Mrn
        {
            get { return MrnFormat.Format(_worklistPreview.Mrn); }
        }

        public string Healthcard
        {
            get { return HealthcardFormat.Format(_worklistPreview.Healthcard); }
        }

        public string Sex
        {
            get { return _worklistPreview.Sex; }
        }

        public IList<AlertNotificationDetail> AlertNotifications
        {
            get { return _worklistPreview.AlertNotifications; }
        }

        public bool HasAlert
        {
            get { return (_worklistPreview != null && _worklistPreview.AlertNotifications.Count > 0); }
        }
        
        public string GetAlertImageURI(AlertNotificationDetail detail)
        {
            string alertImageURI = "";

            switch (detail.Type)
            {
                case "Note Alert":
                    alertImageURI = "http://172.16.10.167/RisTemplates/note.png";
                    break;
                case "Language Alert":
                    alertImageURI = "http://172.16.10.167/RisTemplates/language.png";
                    break;
                default:
                    alertImageURI = "http://172.16.10.167/RisTemplates/healthcare3.jpg";
                    break;
            }

            return alertImageURI;
        }

        public string GetAlertTooltip(AlertNotificationDetail detail)
        {
            string alertTooltip = "";
            string patientName = String.Format("{0}. {1}"
                , _worklistPreview.Name.GivenName.Substring(0, 1)
                , _worklistPreview.Name.FamilyName);

            switch (detail.Type)
            {
                case "Note Alert":
                    alertTooltip = String.Format("{0} has high severity notes: \r\n{1}"
                        , patientName
                        , StringUtilities.Combine<string>(detail.Reasons, "\r\n"));
                    break;
                case "Language Alert":
                    alertTooltip = String.Format("{0} speaks: {1}"
                        , patientName
                        , StringUtilities.Combine<string>(detail.Reasons, ", "));
                    break;
                default:
                    break;
            }

            return alertTooltip;
        }

        #endregion

    }
}
