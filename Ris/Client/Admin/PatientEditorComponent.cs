using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;

using Iesi.Collections;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(PatientEditorComponentViewExtensionPoint))]
    public class PatientEditorComponent : ApplicationComponent
    {
        class PatientIdentifiersActionHandler : CrudActionHandler
        {
            private PatientEditorComponent _component;

            internal PatientIdentifiersActionHandler(PatientEditorComponent component)
            {
                _component = component;
            }

            protected override void Add()
            {
                _component.AddIdentifer();
            }

            protected override void Edit()
            {
                _component.UpdateSelectedIdentifier();
            }

            protected override void Delete()
            {
                _component.DeleteSelectedIdentifier();
            }
        }



        private Patient _patient;
        private IPatientAdminService _patientAdminService;
        private TableData<PatientIdentifier> _patientIdentifiers;
        private PatientIdentifier _currentPatientIdentifierSelection;

        private PatientIdentifiersActionHandler _patientIdentifiersActionHandler;

        public PatientEditorComponent()
        {
            _patient = Patient.New();
            _patientIdentifiers = new TableData<PatientIdentifier>();

            _patientIdentifiers.AddColumn<string>("Type", delegate(PatientIdentifier pi) { return _patientAdminService.PatientIdentifierTypeEnumTable[pi.Type].Value; });
            _patientIdentifiers.AddColumn<string>("ID", delegate(PatientIdentifier pi) { return pi.Id; });
            _patientIdentifiers.AddColumn<string>("Assigning Authority", delegate(PatientIdentifier pi) { return pi.AssigningAuthority; });

            _patientIdentifiersActionHandler = new PatientIdentifiersActionHandler(this);

            // add is always enabled
            _patientIdentifiersActionHandler.AddEnabled = true;
        }

        /// <summary>
        /// Gets or sets the subject (e.g Patient) that this editor operates on.  This property
        /// should never be used by the view.
        /// </summary>
        public Patient Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public override void Start()
        {
            base.Start();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public ActionModelNode PatientIdentifierToolbarActions
        {
            get { return _patientIdentifiersActionHandler.ToolbarModel; }
        }

        public ActionModelNode PatientIdentifierMenuActions
        {
            get { return _patientIdentifiersActionHandler.MenuModel; }
        }

        public string FamilyName
        {
            get { return _patient.Name.FamilyName; }
            set { 
                _patient.Name.FamilyName = value;
                this.Modified = true;
            }
        }

        public string GivenName
        {
            get { return _patient.Name.GivenName; }
            set { 
                _patient.Name.GivenName = value;
                this.Modified = true;
            }
        }

        public string MiddleName
        {
            get { return _patient.Name.MiddleName; }
            set { 
                _patient.Name.MiddleName = value;
                this.Modified = true;
            }
        }

        public string Sex
        {
            get { return _patientAdminService.SexEnumTable[_patient.Sex].Value; }
            set { 
                _patient.Sex = _patientAdminService.SexEnumTable[value].Code;
                this.Modified = true;
            }
        }

        public string[] SexChoices
        {
            get { return _patientAdminService.SexEnumTable.Values; }
        }

        public DateTime DateOfBirth
        {
            get { return _patient.DateOfBirth; }
            set { 
                _patient.DateOfBirth = value;
                this.Modified = true;
            }
        }

        public bool DeathIndicator
        {
            get { return _patient.DeathIndicator; }
            set { 
                _patient.DeathIndicator = value;
                this.Modified = true;
            }
        }

        public DateTime? TimeOfDeath
        {
            get { return _patient.TimeOfDeath; }
            set { 
                _patient.TimeOfDeath = value;
                this.Modified = true;
            }
        }
            

        public ITableData PatientIdentifiers
        {
            get { return _patientIdentifiers; }
        }

        public PatientIdentifier CurrentPatientIdentifierSelection
        {
            get { return _currentPatientIdentifierSelection; }
            set { 
                _currentPatientIdentifierSelection = value;
                PatientIdentifierSelectionChanged();
            }
        }

        public void SetSelectedIdentifier(ISelection selection)
        {
            this.CurrentPatientIdentifierSelection = (PatientIdentifier)selection.Item;
        }

        private void PatientIdentifierSelectionChanged()
        {
            if (_currentPatientIdentifierSelection != null)
            {
                _patientIdentifiersActionHandler.EditEnabled = true;
                _patientIdentifiersActionHandler.DeleteEnabled = true;
            }
            else
            {
                _patientIdentifiersActionHandler.EditEnabled = false;
                _patientIdentifiersActionHandler.DeleteEnabled = false;
            }
        }

        public void LoadIdentifierTable()
        {
            if (_patient != null)
            {
                foreach (PatientIdentifier identifier in _patient.Identifiers)
                {
                    _patientIdentifiers.Add(identifier);
                }
            }
        }

        public void AddIdentifer()
        {
            PatientIdentifier identifier = PatientIdentifier.New();

            PatientIdentifierEditorComponent editor = new PatientIdentifierEditorComponent(identifier);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Add Identifier...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _patientIdentifiers.Add(identifier);
                _patient.Identifiers.Add(identifier);
                this.Modified = true;
            }
        }

        public void UpdateSelectedIdentifier()
        {
            // can occur if user double clicks while holding control
            if (_currentPatientIdentifierSelection == null) return;

            PatientIdentifier identifier = PatientIdentifier.New();
            identifier.CopyFrom(_currentPatientIdentifierSelection);

            PatientIdentifierEditorComponent editor = new PatientIdentifierEditorComponent(identifier);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Update Identifier...");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _currentPatientIdentifierSelection.CopyFrom(identifier);
                this.Modified = true;
            }
        }

        public void DeleteSelectedIdentifier()
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this identifier", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary PatientIdentifier otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong Identifier being removed from the Patient
                PatientIdentifier toBeRemoved = _currentPatientIdentifierSelection;
                _patientIdentifiers.Remove(toBeRemoved);
                _patient.Identifiers.Remove(toBeRemoved);
                this.Modified = true;
            }
        }
    }
}
