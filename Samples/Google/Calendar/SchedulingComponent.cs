using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Validation;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Samples.Google.Calendar
{
    public interface ISchedulingToolContext : IToolContext
    {
        DesktopWindow DesktopWindow { get; }
        CalendarEvent SelectedAppointment { get; }
        IList<CalendarEvent> AllAppointments { get; }
    }

    [ExtensionPoint]
    public class SchedulingToolExtensionPoint : ExtensionPoint<ITool>
    {
    }

    /// <summary>
    /// Extension point for views onto <see cref="SchedulingComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class SchedulingComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// SchedulingComponent class
    /// </summary>
    [AssociateView(typeof(SchedulingComponentViewExtensionPoint))]
    public class SchedulingComponent : ApplicationComponent
    {
        public class SchedulingToolContext : ToolContext, ISchedulingToolContext
        {
            private SchedulingComponent _owner;

            internal SchedulingToolContext(SchedulingComponent owner)
            {
                _owner = owner;
            }

            #region ISchedulingToolContext Members

            public DesktopWindow DesktopWindow
            {
                get { return _owner.Host.DesktopWindow; }
            }

            public CalendarEvent SelectedAppointment
            {
                get { return _owner._selectedAppointment; }
            }

            public IList<CalendarEvent> AllAppointments
            {
                get { return _owner._appointments.Items; }
            }

            #endregion
        }

        private string _patientInfo;
        private string _comment;
        private DateTime _appointmentDate;
        private Table<CalendarEvent> _appointments;
        private CalendarEvent _selectedAppointment;


        private Calendar _calendar;

        private ToolSet _extensionTools;
        private ActionModelRoot _menuModel;


        /// <summary>
        /// Constructor
        /// </summary>
        public SchedulingComponent()
        {
        }

        public override void Start()
        {
            _appointmentDate = Platform.Time;
            _calendar = new Calendar();

            _appointments = new Table<CalendarEvent>();
            _appointments.Columns.Add(new TableColumn<CalendarEvent, string>("Date",
                delegate(CalendarEvent e) { return Format.Date(e.StartTime); }));
            _appointments.Columns.Add(new TableColumn<CalendarEvent, string>("Comment",
                delegate(CalendarEvent e) { return e.Description; }));

            _extensionTools = new ToolSet(new SchedulingToolExtensionPoint(), new SchedulingToolContext(this));
            _menuModel = ActionModelRoot.CreateModel(this.GetType().FullName, "scheduling-appointments-contextmenu", _extensionTools.Actions);

            // initialize patient info from active workspace
            UpdatePatientInfo(this.Host.DesktopWindow.ActiveWorkspace);

            // subscribe to desktop window for changes in active workspace
            this.Host.DesktopWindow.Workspaces.ItemActivationChanged += Workspaces_ItemActivationChanged;


            base.Start();
        }

        public override void Stop()
        {
            _extensionTools.Dispose();

            this.Host.DesktopWindow.Workspaces.ItemActivationChanged -= Workspaces_ItemActivationChanged;

            base.Stop();
        }

        private void Workspaces_ItemActivationChanged(object sender, ItemEventArgs<Workspace> e)
        {
            Workspace workspace = e.Item;

            if (workspace.Active)
            {
                UpdatePatientInfo(workspace);
            }
            else
            {
                Reset();
            }
        }

        private void UpdatePatientInfo(Workspace workspace)
        {
            IImageViewer viewer = ImageViewerComponent.GetAsImageViewer(workspace);
            if (viewer != null)
            {
                Patient patient = CollectionUtils.FirstElement<Patient>(viewer.StudyTree.Patients.Values);
                this.PatientInfo = string.Format("{0} {1}", patient.PatientId, patient.PatientsName);

                _appointments.Items.Clear();
                _appointments.Items.AddRange(_calendar.GetEvents(_patientInfo, null, null));
            }
            else
            {
                this.PatientInfo = null;
            }
        }

        public ActionModelNode MenuModel
        {
            get { return _menuModel; }
        }

        public DateTime Today
        {
            get { return DateTime.Today; }
        }

        public ITable Appointments
        {
            get { return _appointments; }
        }

        public ISelection SelectedAppointment
        {
            get { return new Selection(_selectedAppointment); }
            set
            {
                if (value.Item != _selectedAppointment)
                {
                    _selectedAppointment = (CalendarEvent)value.Item;
                    NotifyPropertyChanged("SelectedAppointment");
                }
            }
        }

        [ValidateNotNull]
        public string PatientInfo
        {
            get { return _patientInfo; }
            private set
            {
                if (value != _patientInfo)
                {
                    _patientInfo = value;
                    NotifyPropertyChanged("PatientInfo");
                }
            }
        }

        [ValidateNotNull]
        public string Comment
        {
            get { return _comment; }
            set
            {
                if (value != _comment)
                {
                    _comment = value;
                    this.NotifyPropertyChanged("Comment");
                }
            }
        }

        [ValidateGreaterThan("Today", Message="AppointmentMustBeInFuture")]
        public DateTime AppointmentDate
        {
            get { return _appointmentDate; }
            set
            {
                if (value != _appointmentDate)
                {
                    _appointmentDate = value;
                    this.NotifyPropertyChanged("AppointmentDate");
                }
            }
        }

        public void AddAppointment()
        {
            if (this.HasValidationErrors)
            {
                this.ShowValidation(true);
                return;
            }

            CalendarEvent e = _calendar.AddEvent(_patientInfo, _comment, _appointmentDate, _appointmentDate);

            _appointments.Items.Add(e);
        }

        private void Reset()
        {
            _appointments.Items.Clear();

            this.PatientInfo = null;
            this.Comment = null;
            this.AppointmentDate = Platform.Time;
            this.ShowValidation(false);
        }
    }
}
