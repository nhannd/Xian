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
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Ris.Application.Common.RegistrationWorkflow;

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
    public class TechnologistPreviewComponent : HtmlApplicationComponent
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

        private List<RICSummary> _previousRIC;
        private List<RICSummary> _upcomingRIC;
        private List<DiagnosticServiceBreakdownSummary> _dsBreakdown;

        private RICTable _previousRICTable;
        private RICTable _upcomingRICTable;

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
            _previousRICTable = new RICTable();
            _upcomingRICTable = new RICTable();

            _previousRIC = new List<RICSummary>();
            _upcomingRIC = new List<RICSummary>();

            _dsBreakdown = new List<DiagnosticServiceBreakdownSummary>();


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

                _previousRICTable.Items.Clear();
                _upcomingRICTable.Items.Clear();

                _previousRIC.Clear();
                _upcomingRIC.Clear();

                _dsBreakdown.Clear();

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

                _previousRIC.AddRange(CollectionUtils.Select<RICSummary>(
                    _worklistPreview.RICs,
                    delegate(RICSummary summary)
                    {
                        return summary.ScheduledTime < Platform.Time;
                    }));

                _upcomingRIC.AddRange(CollectionUtils.Select<RICSummary>(
                    _worklistPreview.RICs,
                    delegate(RICSummary summary)
                    {
                        return summary.ScheduledTime > Platform.Time;
                    }));

                _upcomingRICTable.Items.AddRange(_upcomingRIC);
                _upcomingRICTable.Sort(new TableSortParams(_upcomingRICTable.Columns[3], true));
                _previousRICTable.Items.AddRange(_previousRIC);
                _previousRICTable.Sort(new TableSortParams(_previousRICTable.Columns[3], true));

                _dsBreakdown.AddRange(_worklistPreview.DSBreakdown);
                //_dsBreakdown.Sort(
                //    delegate(DiagnosticServiceBreakdownSummary x, DiagnosticServiceBreakdownSummary y)
                //    {
                //        return x.ModalityProcedureStepName.CompareTo(y.ModalityProcedureStepName);
                //    });
                //_dsBreakdown.Sort(
                //    delegate(DiagnosticServiceBreakdownSummary x, DiagnosticServiceBreakdownSummary y)
                //    {
                //        return x.RequestedProcedureName.CompareTo(y.RequestedProcedureName);
                //    });

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
            get { return String.Format(SR.MessageAlertUnreconciledRecords, PersonNameFormat.Format(_worklistPreview.Name, "%g. %F")); }
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

        public string AccessionNumber
        {
            get { return _worklistPreview.AccessionNumber; }
        }

        public string Priority
        {
            get { return _worklistPreview.Priority; }
        }

        public string OrderingPhysician
        {
            get { return PersonNameFormat.Format(_worklistPreview.OrderingPhysician.PersonNameDetail); }
        }

        public string Facility
        {
            get { return _worklistPreview.Facility.Name; }
        }

        public string DSBreakdownHTML
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                bool firstRow = true;
                string rpName = "";
                int i = 0;
                foreach (DiagnosticServiceBreakdownSummary ds in _dsBreakdown)
                {
                    sb.Append(string.Format("<tr class=\"row{0}\"", i++ % 2));
                    if (ds.Active)
                    {
                        sb.Append(" class=\"activeDS\"");
                    }
                    sb.Append(">");
                    if (firstRow)
                    {
                        sb.Append(string.Format("<td class=\"node\">{0}</td>", ds.DiagnosticServiceName));
                        firstRow = false;
                    }
                    else
                    {
                        sb.Append("<td class=\"blankNode\"></td>");
                    }

                    if (ds.RequestedProcedureName.Equals(rpName))
                    {
                        sb.Append("<td class=\"blankNode\"></td>");
                    }
                    else
                    {
                        sb.Append(string.Format("<td class=\"spacerNode\">{0}</td>", ds.RequestedProcedureName));
                        rpName = ds.RequestedProcedureName;
                    }

                    sb.Append(string.Format("<td class=\"node\">{0}</td>", ds.ModalityProcedureStepName));
                    sb.Append(string.Format("<td>{0}</td>", ds.ModalityProcedureStepStatus));
                    sb.Append(ds.Active ? "<td>*</td>" : "<td></td>");
                    sb.Append("</tr>");
                }

                return sb.ToString();
            }
        }

        public string MPSName
        {
            get { return _worklistPreview.MpsName; }
        }

        public string Modality
        {
            get { return _worklistPreview.Modality.Name; }
        }

        public string Status
        {
            get { return _worklistPreview.Status; }
        }

        public string DiscontinueReason
        {
            get { return _worklistPreview.DiscontinueReason; }
        }

        public string AssignedStaff
        {
            get 
            { 
                return _worklistPreview.AssignedStaff == null 
                    ? "None"
                    : PersonNameFormat.Format(_worklistPreview.AssignedStaff.PersonNameDetail); 
            }
        }

        public string PerformingStaff
        {
            get
            {
                return _worklistPreview.PerformingStaff == null
                    ? "None"
                    : PersonNameFormat.Format(_worklistPreview.PerformingStaff.PersonNameDetail);
            }
        }

        public string ScheduledStartTime
        {
            get 
            { 
                return _worklistPreview.ScheduledStartTime.HasValue 
                    ? Format.DateTime(_worklistPreview.ScheduledStartTime.Value)
                    : string.Empty; 
            }
        }

        public string ScheduledEndTime
        {
            get
            {
                return _worklistPreview.ScheduledEndTime.HasValue
                    ? Format.DateTime(_worklistPreview.ScheduledEndTime.Value)
                    : string.Empty;
            }
        }

        public string StartTime
        {
            get
            {
                return _worklistPreview.StartTime.HasValue
                    ? Format.DateTime(_worklistPreview.StartTime.Value)
                    : string.Empty;
            }
        }

        public string EndTime
        {
            get
            {
                return _worklistPreview.EndTime.HasValue
                    ? Format.DateTime(_worklistPreview.EndTime.Value)
                    : string.Empty;
            }
        }

        public ITable PreviousRICs
        {
            get { return _previousRICTable; }
        }

        public ITable UpcomingRICs
        {
            get { return _upcomingRICTable; }
        }

        public string GetAlertImageURI(AlertNotificationDetail detail)
        {
            string alertImageURI = "";

            switch (detail.Type)
            {
                case "Note Alert":
                    alertImageURI = Server + "/Images/note.png";
                    break;
                case "Language Alert":
                    alertImageURI = Server + "/Images/language.png";
                    break;
                default:
                    alertImageURI = Server + "/Images/healthcare3.jpg";
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
