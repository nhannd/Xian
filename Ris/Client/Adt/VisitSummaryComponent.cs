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
using ClearCanvas.Ris.Client.Common;

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
        private EntityRef<PatientProfile> _patientProfileRef;
        private Patient _patient;
        private PatientProfile _patientProfile;

        private IAdtService _adtService;
        private VisitTable _visitTable;
        private Visit _currentVisitSelection;

        private CrudActionModel _visitActionHandler;

        public VisitSummaryComponent(EntityRef<PatientProfile> patientProfileRef)
        {
            _patientProfileRef = patientProfileRef;
        }

        public override void Start()
        {
            _adtService = ApplicationContext.GetService<IAdtService>();

            _patientProfile = _adtService.LoadPatientProfile(_patientProfileRef, false);
            _patient = _adtService.LoadPatientAndAllProfiles(_patientProfileRef);
            _patientRef = new EntityRef<Patient>(_patient);

            _visitTable = new VisitTable();

            _visitActionHandler = new CrudActionModel();
            _visitActionHandler.Add.SetClickHandler(AddVisit);
            _visitActionHandler.Edit.SetClickHandler(UpdateSelectedVisit);
            _visitActionHandler.Delete.SetClickHandler(DeleteSelectedVisit);

            _visitActionHandler.Add.Enabled = true;

            this.Host.SetTitle(string.Format(SR.TitleVisitSummaryComponent,
                Format.Custom(_patientProfile.Name), Format.Custom(_patientProfile.Mrn)));

            base.Start();
        }

        public override void Stop()
        {
            // TODO prepare the component to exit the live phase
            // This is a good place to do any clean up
            base.Stop();
        }

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
            VisitEditorComponent editor = new VisitEditorComponent(_patientRef);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddVisit);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                LoadVisitsTable();
                this.Modified = true;
            }
        }

        public void UpdateSelectedVisit()
        {             
            // can occur if user double clicks while holding control
            if (_currentVisitSelection == null) return;

            VisitEditorComponent editor = new VisitEditorComponent(_patientRef, new EntityRef<Visit>(_currentVisitSelection));
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateVisit);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                LoadVisitsTable();
                this.Modified = true;
            }
        }

        public void DeleteSelectedVisit()
        {
            if (this.Host.ShowMessageBox(SR.MessageDeleteSelectedVisit, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //delete the visit
                LoadVisitsTable();
                this.Modified = true;
            }
        }

        public void LoadVisitsTable()
        {
            _visitTable.Items.Clear();
            _visitTable.Items.AddRange(_adtService.ListPatientVisits(_patientRef));
        }

        public void Close()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
        }
    }
}
