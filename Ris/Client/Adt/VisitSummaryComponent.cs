using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;
using ClearCanvas.Desktop.Actions;

namespace ClearCanvas.Ris.Client.Adt
{
    /// <summary>
    /// Extension point for views onto <see cref="VisitSummaryComponent"/>
    /// </summary>
    [ExtensionPoint]
    public class VisitSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    /// <summary>
    /// VisitSummaryComponent class
    /// </summary>
    [AssociateView(typeof(VisitSummaryComponentViewExtensionPoint))]
    public class VisitSummaryComponent : ApplicationComponent
    {
        private EntityRef<Patient> _patientRef;
        private Patient _patient;
        private IList<Visit> _visits;
        private IPatientAdminService _patientAdminService;
        private IAdtService _adtService;
        private Table<Visit> _visitTable;
        private Visit _currentVisitSelection;

        private CrudActionModel _visitActionHandler;

        PatientClassEnumTable _patientClasses;
        PatientTypeEnumTable _patientTypes;
        AdmissionTypeEnumTable _admissionTypes;
        AmbulatoryStatusEnumTable _ambulatoryStatuses;
        VisitStatusEnumTable _visitStatuses;

        /// <summary>
        /// Constructor
        /// </summary>
        public VisitSummaryComponent(EntityRef<Patient> patientRef)
        {
            _patientRef = patientRef;
            _adtService = ApplicationContext.GetService<IAdtService>();
            _patient = _adtService.LoadPatient(_patientRef);
            _visits = _adtService.ListPatientVisits(_patientRef);

            _visitTable= new Table<Visit>();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();

            _patientClasses = _patientAdminService.GetPatientClassEnumTable();
            _patientTypes = _patientAdminService.GetPatientTypeEnumTable();
            _admissionTypes = _patientAdminService.GetAdmissionTypeEnumTable();
            _ambulatoryStatuses = _patientAdminService.GetAmbulatoryStatusEnumTable();
            _visitStatuses = _patientAdminService.GetVisitStatusEnumTable();
            
            //number
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Number",
                delegate(Visit v) { return v.VisitNumber.Id; },
                1.0f));
            //site
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Assigning Authority",
                delegate(Visit v) { return v.VisitNumber.AssigningAuthority; },
                1.0f));
            //status
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Status",
                delegate(Visit v) { return _visitStatuses[v.VisitStatus].Value; },
                1.0f));
            //admit date/time
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Admit Date/Time",
                delegate(Visit v) { return Format.Date(v.AdmitDateTime); },
                1.0f));
            //Patient class
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Patient Class",
                delegate(Visit v) { return _patientClasses[v.PatientClass].Value; },
                1.0f));
            //patient type
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Patient Type",
                delegate(Visit v) { return _patientTypes[v.PatientType].Value; },
                1.0f));
            //admission type
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Admission Type",
                delegate(Visit v) { return _admissionTypes[v.AdmissionType].Value; },
                1.0f));
            //facility
            //_visitTable.Columns.Add(new TableColumn<Visit, string>("Facility",
            //    delegate(Visit v) { return v.Facility != null ? v.Facility.Name : ""; },
            //    1.0f));
            //discharge datetime
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Discharge Date/Time",
                delegate(Visit v) { return Format.Date(v.DischargeDateTime); },
                1.0f));
            //discharge disposition
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Discharge Disposition",
                delegate(Visit v) { return v.DischargeDisposition; },
                1.0f));
            //current location
            //_visits.Columns.Add(new TableColumn<Visit, string>("Current Location",
            //    delegate(Visit v) { return v.CurrentLocation.Format(); },
            //    1.0f));
            //practitioners
            //_visits.Columns.Add(new TableColumn<Visit, string>("Some Practitioner",
            //    delegate(Visit v) { return; },
            //    1.0f));
            //VIP
            _visitTable.Columns.Add(new TableColumn<Visit, bool>("VIP?",
                delegate(Visit v) { return v.VipIndicator; },
                1.0f));
            //Ambulatory status
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Ambulatory Status",
                delegate(Visit v) { return _ambulatoryStatuses[v.AmbulatoryStatus].Value; },
                1.0f));
            //preadmit number
            _visitTable.Columns.Add(new TableColumn<Visit, string>("Pre-Admit Number",
                delegate(Visit v) { return v.PreadmitNumber; },
                1.0f));

            _visitActionHandler = new CrudActionModel();
            _visitActionHandler.Add.Handler = AddVisit;
            _visitActionHandler.Edit.Handler = UpdateSelectedVisit;
            _visitActionHandler.Delete.Handler = DeleteSelectedVisit;

            _visitActionHandler.Add.Enabled = true;

        }

        public override void Start()
        {
            // TODO prepare the component for its live phase
            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

        //public Patient Subject
        //{
        //    get { return _patient; }
        //    set { _patient = value; }
        //}
        
        public ITable Visits
        {
            get { return _visitTable; }
        }

        public Visit CurrentVisitSelection
        {
            get { return _currentVisitSelection; }
            set
            {
                _currentVisitSelection = value;
                VisitSelectionChanged();
            }
        }

        public void SetSelectedVisit(ISelection selection)
        {
            this.CurrentVisitSelection = (Visit)selection.Item;
        }

        private void VisitSelectionChanged()
        {
            if (_currentVisitSelection != null)
            {
                //_adtService.LoadVisitDetails(_currentVisitSelection);
                _visitActionHandler.Edit.Enabled = true;
                _visitActionHandler.Delete.Enabled = true;
            }
            else
            {
                _visitActionHandler.Edit.Enabled = false;
                _visitActionHandler.Delete.Enabled = false;
            }
        }


        public ActionModelNode VisitListActionModel
        {
            get { return _visitActionHandler; }
        }

        public void AddVisit()
        {
            Visit visit = new Visit();
            visit.Patient = _patient;

            VisitEditorComponent editor = new VisitEditorComponent(visit);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Add Visit...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _visitTable.Items.Add(visit);
                //_patient.Visits.Add(visit);
                this.Modified = true;
            }
        }

        public void UpdateSelectedVisit()
        {             
            // can occur if user double clicks while holding control
            if (_currentVisitSelection == null) return;

            Visit visit = new Visit();
            visit.CopyFrom(_currentVisitSelection);

            VisitEditorComponent editor = new VisitEditorComponent(visit);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, "Update Visit...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                Visit toBeRemoved = _currentVisitSelection;
                _visitTable.Items.Remove(toBeRemoved);
                //_patient.Visits.Remove(toBeRemoved);

                _visitTable.Items.Add(visit);
                //_patient.Visits.Add(visit);

                this.Modified = true;
            }
        }

        public void DeleteSelectedVisit()
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this visit?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary Visit otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong Visit being removed from the Patient
                Visit toBeRemoved = _currentVisitSelection;
                _visitTable.Items.Remove(toBeRemoved);
                //_patient.Visits.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        public void LoadVisitsTable()
        {
            foreach (Visit visit in _visits)
            {
                _visitTable.Items.Add(visit);
            }
        }

        public void Accept()
        {
            try
            {
                SaveChanges();
                this.ExitCode = ApplicationComponentExitCode.Normal;
            }
            catch (ConcurrencyException)
            {
                this.Host.ShowMessageBox("The visit was modified by another user.  Your changes could not be saved.", MessageBoxActions.Ok);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            catch (Exception e)
            {
                Platform.Log(e);
                this.Host.ShowMessageBox("An error occured while attempting to save changes to this item", MessageBoxActions.Ok);
                this.ExitCode = ApplicationComponentExitCode.Error;
            }
            this.Host.Exit();
            //this.ExitCode = ApplicationComponentExitCode.Normal;
            //Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        private void SaveChanges()
        {
            //IAdtService service = ApplicationContext.GetService<IAdtService>();
            //service.UpdatePatientVisits(_patient);
        }

    }
}
