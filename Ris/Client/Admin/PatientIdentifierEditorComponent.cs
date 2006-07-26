using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Enterprise;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    [ExtensionPoint()]
    public class PatientIdentifierEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(PatientIdentifierEditorComponentViewExtensionPoint))]
    public class PatientIdentifierEditorComponent : ApplicationComponent
    {
        PatientIdentifier _patientIdentifier;
        private string[] _dummyAssigningAuthorityChoices = new string[] { "UHN", "MSH", "Ontario" };

        static IPatientAdminService _patientAdminService;

        static PatientIdentifierEditorComponent()
        {
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public PatientIdentifierEditorComponent(PatientIdentifier patientIdentifier)
        {
            _patientIdentifier = patientIdentifier;
        }

        public string Id
        {
            get { return _patientIdentifier.Id; }
            set { _patientIdentifier.Id = value; }
        }

        public string[] TypeChoices
        {
            get { return _patientAdminService.PatientIdentifierTypeEnumTable.Values; }
        }

        public string Type
        {
            get { return _patientAdminService.PatientIdentifierTypeEnumTable[_patientIdentifier.Type].Value; }
            set { _patientIdentifier.Type = _patientAdminService.PatientIdentifierTypeEnumTable[value].Code; }
        }

        public string[] AssigningAuthorityChoices
        {
            get { return _dummyAssigningAuthorityChoices; }
        }

        public string AssigningAuthority
        {
            get { return _patientIdentifier.AssigningAuthority; }
            set { _patientIdentifier.AssigningAuthority = value; }
        }

        public PatientIdentifier PatientIdentifier
        {
            get { return _patientIdentifier; }
            set { _patientIdentifier = value; }
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
