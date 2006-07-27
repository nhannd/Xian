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
        private TableData<PatientIdentifier> _patientIdentifiers;

        public PatientEditorComponent()
        {
            _patient = Patient.New();
            _patientIdentifiers = new TableData<PatientIdentifier>();

            _patientIdentifiers.AddColumn<string>("Type", delegate(PatientIdentifier pi) { return _patientAdminService.PatientIdentifierTypeEnumTable[pi.Type].Value; });
            _patientIdentifiers.AddColumn<string>("ID", delegate(PatientIdentifier pi) { return pi.Id; });
            _patientIdentifiers.AddColumn<string>("AssigningAuthority", delegate(PatientIdentifier pi) { return pi.AssigningAuthority; });

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

        public void UpdateSelectedIdentifier(ISelection selection)
        {
            PatientIdentifier identifier = PatientIdentifier.New();
            PatientIdentifier selectedIdentifier = (PatientIdentifier)selection.Item;
            identifier.CopyFrom(selectedIdentifier);

            PatientIdentifierEditorComponent editor = new PatientIdentifierEditorComponent(identifier);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Update Identifier...");

            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                selectedIdentifier.CopyFrom(identifier);
                this.Modified = true;
            }
        }

        public void DeleteSelectedIdentifier(ISelection selection)
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this identifier", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                PatientIdentifier entry = (PatientIdentifier)selection.Item;
                _patientIdentifiers.Remove(entry);
                _patient.Identifiers.Remove(entry);
                this.Modified = true;
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
            this.ExitCode = ApplicationComponentExitCode.Normal;
        }

        private void DiscardChanges()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
        }
    }
}
