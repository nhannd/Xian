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
using ClearCanvas.Ris.Application.Common.Admin.PatientAdmin;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;
using ClearCanvas.Ris.Application.Common.PatientReconciliation;
using ClearCanvas.Ris.Client.Formatting;

namespace ClearCanvas.Ris.Client.Adt
{
    [ExtensionPoint]
    public class RegistrationPreviewToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    public interface IRegistrationPreviewToolContext : IToolContext
    {
        RegistrationWorklistItem WorklistItem { get; }
        IDesktopWindow DesktopWindow { get; }
    }
    
    /// <summary>
    /// Extension point for views onto <see cref="RegistrationPreviewComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class RegistrationPreviewComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// RegistrationPreviewComponent class
    /// </summary>
    [AssociateView(typeof(RegistrationPreviewComponentViewExtensionPoint))]
    public class RegistrationPreviewComponent : ApplicationComponent
    {
        class RegistrationPreviewToolContext : ToolContext, IRegistrationPreviewToolContext
        {
            private RegistrationPreviewComponent _component;
            public RegistrationPreviewToolContext(RegistrationPreviewComponent component)
            {
                _component = component;
            }

            public RegistrationWorklistItem WorklistItem
            {
                get { return _component.WorklistItem; }
            }

            public IDesktopWindow DesktopWindow
            {
                get { return _component.Host.DesktopWindow; }
            }
        }

        private RegistrationWorklistItem _worklistItem;
        private RegistrationWorklistPreview _worklistPreview;

        List<RICSummary> _activeRIC;
        List<RICSummary> _pastRIC;

        private int _maxRIC;
        private RICTable _activeRICTable;
        private RICTable _pastRICTable;

        private ToolSet _toolSet;

        private BackgroundTask _previewLoadTask;

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationPreviewComponent()
        {
        }

        public RegistrationWorklistItem WorklistItem
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
            _maxRIC = 20;

            _activeRICTable = new RICTable();
            _pastRICTable = new RICTable();

            _activeRIC = new List<RICSummary>();
            _pastRIC = new List<RICSummary>();

            _toolSet = new ToolSet(new RegistrationPreviewToolExtensionPoint(), new RegistrationPreviewToolContext(this));

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

                _activeRICTable.Items.Clear();
                _pastRICTable.Items.Clear();

                _activeRIC.Clear();
                _pastRIC.Clear();

                // clear current preview
                NotifyAllPropertiesChanged();
            }

            if (_worklistItem != null && _worklistItem.PatientProfileRef != null)
            {
                LoadPreviewAsync(_worklistItem);
            }
        }

        private void LoadPreviewAsync(RegistrationWorklistItem item)
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
                        RegistrationWorklistItem worklistItem = (RegistrationWorklistItem)taskContext.UserState;
                        Platform.GetService<IRegistrationWorkflowService>(
                            delegate(IRegistrationWorkflowService service)
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
                _worklistPreview = (RegistrationWorklistPreview)args.Result;

                DateTime today = Platform.Time.Date;

                _activeRIC = CollectionUtils.Select<RICSummary, List<RICSummary>>(_worklistPreview.RICs,
                    delegate(RICSummary summary)
                    {
                        return (summary.ScheduledTime == null || summary.ScheduledTime.Value.CompareTo(today) >= 0);
                    });

                _pastRIC = CollectionUtils.Select<RICSummary, List<RICSummary>>(_worklistPreview.RICs,
                    delegate(RICSummary summary)
                    {
                        return (summary.ScheduledTime != null && summary.ScheduledTime.Value.CompareTo(today) < 0);
                    });

                _activeRIC = (List<RICSummary>) CollectionUtils.Sort(_activeRIC, RICSummary.ActiveScheduledTimeComparer);
                _pastRIC = (List<RICSummary>) CollectionUtils.Sort<RICSummary>(_pastRIC, RICSummary.ActiveScheduledTimeComparer);

                int remainingRIC = _maxRIC;
                foreach (RICSummary summary in _activeRIC)
                {
                    _activeRICTable.Items.Add(summary);
                    if (_activeRICTable.Items.Count >= remainingRIC)
                        break;
                }

                remainingRIC -= _activeRICTable.Items.Count;
                foreach (RICSummary summary in _pastRIC)
                {
                    _pastRICTable.Items.Add(summary);
                    if (_pastRICTable.Items.Count >= remainingRIC)
                        break;
                }

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
            get { return ActionModelRoot.CreateModel(this.GetType().FullName, "RegistrationPreview-menu", _toolSet.Actions); }
        }

        public RegistrationWorklistPreview WorklistPreview
        {
            get { return _worklistPreview; }
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

        public string CurrentHomeAddress
        {
            get { return _worklistPreview.CurrentHomeAddress == null ? "" : AddressFormat.Format(_worklistPreview.CurrentHomeAddress); }
        }

        public string CurrentHomePhone
        {
            get { return _worklistPreview.CurrentHomePhone == null ? "" : TelephoneFormat.Format(_worklistPreview.CurrentHomePhone); }
        }

        public bool HasMoreBasicInfo
        {
            get 
            {
                int moreAddresses = _worklistPreview.Addresses.Count - (_worklistPreview.CurrentHomeAddress == null ? 0 : 1);
                int morePhoneNumbers = _worklistPreview.TelephoneNumbers.Count - (_worklistPreview.CurrentHomePhone == null ? 0 : 1);
                return (moreAddresses > 0 || morePhoneNumbers > 0); 
            }
        }

        public ITable ActiveRIC
        {
            get { return _activeRICTable; }
        }

        public ITable PastRIC
        {
            get { return _pastRICTable; }
        }

        public int MoreRICCount
        {
            get 
            { 
                return _activeRIC.Count - _activeRICTable.Items.Count
                     + _pastRIC.Count - _pastRICTable.Items.Count; 
            }
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
                    alertImageURI = "http://172.16.10.167/RisTemplates/AlertPen.png";
                    break;
                case "Language Alert":
                    alertImageURI = "http://172.16.10.167/RisTemplates/AlertWorld.png";
                    break;
                case "Reconciliation Alert":
                    alertImageURI = "http://172.16.10.167/RisTemplates/AlertMessenger.png";
                    break;
                case "Schedule Alert":
                    alertImageURI = "http://172.16.10.167/RisTemplates/AlertClock.jpg";
                    break;
                default:
                    alertImageURI = "http://172.16.10.167/RisTemplates/AlertMessenger.jpg";
                    break;
            }

            return alertImageURI;
        }

        public string GetAlertTooltip(AlertNotificationDetail detail)
        {
            string alertTooltip = "";
            string patientName = PersonNameFormat.Format(_worklistPreview.Name, "%g. %F");

            switch (detail.Type)
            {
                case "Note Alert":
                    alertTooltip = String.Format(SR.MessageAlertHighSeverityNote
                        , patientName
                        , StringUtilities.Combine<string>(detail.Reasons, ", "));
                    break;
                case "Language Alert":
                    alertTooltip = String.Format(SR.MessageAlertLanguageNotEnglish
                        , patientName
                        , StringUtilities.Combine<string>(detail.Reasons, ", "));
                    break;
                case "Reconciliation Alert":
                    alertTooltip = String.Format(SR.MessageAlertUnreconciledRecords, patientName);
                    break;
                default:
                    break;
            }

            return alertTooltip;
        }

        #endregion
    }
}
