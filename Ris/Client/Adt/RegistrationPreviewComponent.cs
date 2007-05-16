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

        private bool _showHeader;
        private bool _showReconciliationAlert;

        private RegistrationWorklistItem _worklistItem;
        private RegistrationWorklistPreview _worklistPreview;

        List<RICSummary> _recentRIC;
        List<RICSummary> _futureRIC;
        List<RICSummary> _pastRIC;

        private int _maxRIC;
        private RICTable _recentRICTable;
        private RICTable _futureRICTable;
        private RICTable _pastRICTable;

        private ToolSet _toolSet;

        private BackgroundTask _previewLoadTask;

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationPreviewComponent()
            :this(true, true)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public RegistrationPreviewComponent(bool showHeader, bool showReconciliationAlert)
        {
            _showHeader = showHeader;
            _showReconciliationAlert = showReconciliationAlert;
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
            _maxRIC = 10;

            _recentRICTable = new RICTable();
            _futureRICTable = new RICTable();
            _pastRICTable = new RICTable();

            _recentRIC = new List<RICSummary>();
            _futureRIC = new List<RICSummary>();
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

                _recentRICTable.Items.Clear();
                _futureRICTable.Items.Clear();
                _pastRICTable.Items.Clear();

                _recentRIC.Clear();
                _futureRIC.Clear();
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

                DateTime? startOfToday = Platform.Time.Date;
                DateTime? startOfTomorrow = startOfToday.Value.AddDays(1);
                DateTime? endOfTomorrow = startOfToday.Value.AddDays(2);
                DateTime? startOfYesterday = startOfToday.Value.AddDays(-1);

                _recentRIC = SelectScheduledRICByTime(_worklistPreview.RICs, startOfToday, startOfTomorrow);
                _recentRIC.AddRange(SelectScheduledRICByTime(_worklistPreview.RICs, startOfTomorrow, endOfTomorrow));
                _recentRIC.AddRange(SelectScheduledRICByTime(_worklistPreview.RICs, startOfYesterday, startOfToday));

                _futureRIC = SelectScheduledRICByTime(_worklistPreview.RICs, endOfTomorrow, null);
                _pastRIC = SelectScheduledRICByTime(_worklistPreview.RICs, null, startOfYesterday);

                int remainingRIC = _maxRIC;

                foreach (RICSummary summary in _recentRIC)
                {
                    _recentRICTable.Items.Add(summary);
                    if (_recentRICTable.Items.Count >= remainingRIC)
                        break;
                }

                remainingRIC -= _recentRICTable.Items.Count;
                foreach (RICSummary summary in _futureRIC)
                {
                    _futureRICTable.Items.Add(summary);
                    if (_futureRICTable.Items.Count >= remainingRIC)
                        break;
                }

                remainingRIC -= _futureRICTable.Items.Count;
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

        private List<RICSummary> SelectScheduledRICByTime(List<RICSummary> target, DateTime? startTime, DateTime? endTime)
        {
            List<RICSummary> selectedList = CollectionUtils.Select<RICSummary, List<RICSummary>>(target,
                delegate(RICSummary summary)
                {
                    if (summary.ModalityProcedureStepScheduledTime == null)
                        return false;

                    bool inRange = true;
                    if (startTime != null && summary.ModalityProcedureStepScheduledTime < startTime)
                        inRange = false;

                    if (endTime != null && summary.ModalityProcedureStepScheduledTime > endTime)
                        inRange = false;

                    return inRange;
                });

            selectedList.Sort(new Comparison<RICSummary>(CompareRICSummary));

            return selectedList;
        }

        private int CompareRICSummary(RICSummary summary1, RICSummary summary2)
        {
            if (summary1.ModalityProcedureStepScheduledTime == null && summary2.ModalityProcedureStepScheduledTime == null)
                return 0;
            else if (summary1.ModalityProcedureStepScheduledTime == null)
                return 1;
            else if (summary2.ModalityProcedureStepScheduledTime == null)
                return -1;

            return summary1.ModalityProcedureStepScheduledTime.Value.CompareTo(summary2.ModalityProcedureStepScheduledTime.Value);
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

        public ITable RecentRIC
        {
            get { return _recentRICTable; }
        }

        public ITable FutureRIC
        {
            get { return _futureRICTable; }
        }

        public ITable PastRIC
        {
            get { return _pastRICTable; }
        }

        public int MoreRICCount
        {
            get 
            { 
                return _recentRIC.Count - _recentRICTable.Items.Count
                     + _futureRIC.Count - _futureRICTable.Items.Count
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
            string patientName = PersonNameFormat.Format(_worklistPreview.Name, "%g. %F");

            switch (detail.Type)
            {
                case "Note Alert":
                    alertTooltip = String.Format(SR.MessageAlertHighSeverityNote
                        , patientName
                        , StringUtilities.Combine<string>(detail.Reasons, "\r\n"));
                    break;
                case "Language Alert":
                    alertTooltip = String.Format(SR.MessageAlertLanguageNotEnglish
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
