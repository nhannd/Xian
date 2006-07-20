using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Presentation;
using ClearCanvas.Healthcare;

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

        public PatientEditorComponent()
        {
            _patient = Patient.New();
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
            MessageBoxAction result = this.Host.ShowMessageBox("Save changes before closing?", MessageBoxActions.YesNoCancel);
            switch (result)
            {
                case MessageBoxAction.Yes:
                    SaveChanges();
                    return true;
                case MessageBoxAction.No:
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
