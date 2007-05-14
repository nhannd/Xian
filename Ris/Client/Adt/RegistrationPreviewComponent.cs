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

        private int _maxRICDisplay;
        private RICTable _RICTable;

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
            _maxRICDisplay = 10;
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

        public int MaxRIC
        {
            get { return _maxRICDisplay; }
            set { _maxRICDisplay = value; } 
        }

        public override void Start()
        {
            _RICTable = new RICTable();
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
                _RICTable.Items.Clear();

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
                _worklistPreview.RICs = SortRICSummaryList(_worklistPreview.RICs);

                foreach (RICSummary summary in _worklistPreview.RICs)
                {
                    _RICTable.Items.Add(summary);
                    if (_RICTable.Items.Count >= this._maxRICDisplay)
                        break;
                }

                NotifyAllPropertiesChanged();
            }
            else if (args.Reason == BackgroundTaskTerminatedReason.Exception)
            {
                ExceptionHandler.Report(args.Exception, this.Host.DesktopWindow);
            }
        }

        private List<RICSummary> SortRICSummaryList(List<RICSummary> ricList)
        {
            List<RICSummary> ricForToday = new List<RICSummary>();
            List<RICSummary> ricForTomorrow = new List<RICSummary>();
            List<RICSummary> ricForFuture = new List<RICSummary>();
            List<RICSummary> ricForYesterday = new List<RICSummary>();
            List<RICSummary> ricForPast = new List<RICSummary>();

            DateTime today = Platform.Time.Date;

            foreach (RICSummary summary in ricList)
            {
                //DateTime datePart = summary.ModalityProcedureStepScheduledTime.Value.Date;
                if (summary.ModalityProcedureStepScheduledTime != null)
                {
                    DateTime datePart = summary.ModalityProcedureStepScheduledTime.Value.Date;

                    if (datePart == today)
                        ricForToday.Add(summary);
                    else if (datePart == today.AddDays(-1))
                        ricForYesterday.Add(summary);
                    else if (datePart == today.AddDays(1))
                        ricForTomorrow.Add(summary);
                    else if (datePart.CompareTo(today) < 0)
                        ricForPast.Add(summary);
                    else
                        ricForFuture.Add(summary);
                }
            }

            Comparison<RICSummary> summaryComparer = new Comparison<RICSummary>(CompareRICSummary);
            ricForToday.Sort(summaryComparer);
            ricForYesterday.Sort(summaryComparer);
            ricForTomorrow.Sort(summaryComparer);
            ricForPast.Sort(summaryComparer);
            ricForFuture.Sort(summaryComparer);

            List<RICSummary> sortedList = new List<RICSummary>();
            sortedList.AddRange(ricForToday);
            sortedList.AddRange(ricForTomorrow);
            sortedList.AddRange(ricForFuture);
            sortedList.AddRange(ricForYesterday);
            sortedList.AddRange(ricForPast);

            return sortedList;
        }

        private int CompareRICSummary(RICSummary summary1, RICSummary summary2)
        {
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
            get { return String.Format("{0} {1}", _worklistPreview.MrnAssigningAuthority, _worklistPreview.MrnID); }
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

        public ITable RIC
        {
            get { return _RICTable; }
        }

        public int MoreRICCount
        {
            get { return _worklistPreview.RICs.Count - _RICTable.Items.Count; }
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
