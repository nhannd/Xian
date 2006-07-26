using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
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
        private Patient _patient;
        private IPatientAdminService _patientAdminService;
        private TableData<PatientIdentifierTableEntry> _patientIdentifiers;

        private PatientIdentifier _selectedPatientIdentifier;

        public PatientEditorComponent()
        {
            _patient = Patient.New();
            _patientIdentifiers = new TableData<PatientIdentifierTableEntry>();
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

        public string FamilyName
        {
            get { return _patient.Name.FamilyName; }
            set { _patient.Name.FamilyName = value; }
        }

        public string GivenName
        {
            get { return _patient.Name.GivenName; }
            set { _patient.Name.GivenName = value; }
        }

        public string MiddleName
        {
            get { return _patient.Name.MiddleName; }
            set { _patient.Name.MiddleName = value; }
        }

        public string Sex
        {
            get { return _patientAdminService.SexEnumTable[_patient.Sex].Value; }
            set { _patient.Sex = _patientAdminService.SexEnumTable[value].Code; }
        }

        public string[] SexChoices
        {
            get { return _patientAdminService.SexEnumTable.Values; }
        }

        public ITableData PatientIdentifiers
        {
            get { return _patientIdentifiers; }
        }

        public void LoadIdentifierTable()
        {
            if (_patient != null)
            {
                foreach (PatientIdentifier identifier in _patient.Identifiers)
                {
                    _patientIdentifiers.Add(new PatientIdentifierTableEntry(identifier));
                }
            }
        }

        public void SetIdentifierSelection(ISelection selection)
        {
            if (selection == null) return;
            PatientIdentifierTableEntry entry = (PatientIdentifierTableEntry)selection.Item;
            _selectedPatientIdentifier = PatientIdentifier.New();
            if (entry != null)
            {
                _selectedPatientIdentifier.CopyFrom(entry.PatientIdentifier);
            }
        }

        public void AddIdentifer()
        {
            PatientIdentifier identifier = PatientIdentifier.New();
            PatientIdentifierEditorComponent editor = new PatientIdentifierEditorComponent(identifier);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "test");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _patientIdentifiers.Add(new PatientIdentifierTableEntry(identifier));
                _patient.Identifiers.Add(identifier);
            }
        }

        public void UpdateSelectedIdentifier(ISelection selection)
        {
            PatientIdentifierTableEntry entry = (PatientIdentifierTableEntry)selection.Item;
            PatientIdentifierEditorComponent editor = new PatientIdentifierEditorComponent(entry.PatientIdentifier);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "test");
        }

        public void DeleteSelectedIdentifier(ISelection selection)
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this identifier", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                PatientIdentifierTableEntry entry = (PatientIdentifierTableEntry)selection.Item;
                PatientIdentifier patientIdentifer = entry.PatientIdentifier;
                _patientIdentifiers.Remove(entry);
                _patient.Identifiers.Remove(patientIdentifer);
            }

        }

        public void Accept()
        {
            SaveChanges();
            Host.Exit();
        }

        public void Cancel()
        {
            DiscardChanges();
            Host.Exit();
        }

        public override bool CanExit()
        {
            DialogBoxAction result = this.Host.ShowMessageBox("Save changes before closing?", MessageBoxActions.YesNoCancel);
            switch (result)
            {
                case DialogBoxAction.Yes:
                    SaveChanges();
                    return true;
                case DialogBoxAction.No:
                    DiscardChanges();
                    return true;
                default:
                    return false;
            }
        }

        private void SaveChanges()
        {
            // TODO save data here

            this.ExitCode = ApplicationComponentExitCode.Normal;
        }

        private void DiscardChanges()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
        }
    }
}
