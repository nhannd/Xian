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

    [AssociateView(typeof(PatientIdentifierEditorComponentViewExtensionPoint))]
    public class PatientIdentifierEditorComponent : ApplicationComponent
    {
        PatientIdentifier _patientIdentifier;
        private string[] _dummyAssigningAuthorityChoices = new string[] { "UHN", "MSH", "Ontario" };

        private IPatientAdminService _patientAdminService;

        public PatientIdentifierEditorComponent(PatientIdentifier patientIdentifier)
        {
            _patientIdentifier = patientIdentifier;
        }

        /// <summary>
        /// Sets the subject upon which the editor acts
        /// Not for use by the view
        /// </summary>
        public PatientIdentifier PatientIdentifier
        {
            get { return _patientIdentifier; }
            set { _patientIdentifier = value; }
        }

        public override void Start()
        {
            base.Start();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }


        public string Id
        {
            get { return _patientIdentifier.Id; }
            set
            {
                _patientIdentifier.Id = value;
                this.Modified = true;
            }
        }

        public string[] TypeChoices
        {
            get { return _patientAdminService.PatientIdentifierTypeEnumTable.Values; }
        }

        public string Type
        {
            get { return _patientAdminService.PatientIdentifierTypeEnumTable[_patientIdentifier.Type].Value; }
            set
            {
                _patientIdentifier.Type = _patientAdminService.PatientIdentifierTypeEnumTable[value].Code;
                this.Modified = true;
            }
        }

        public string[] AssigningAuthorityChoices
        {
            get { return _dummyAssigningAuthorityChoices; }
        }

        public string AssigningAuthority
        {
            get { return _patientIdentifier.AssigningAuthority; }
            set
            {
                _patientIdentifier.AssigningAuthority = value;
                this.Modified = true;
            }
        }

        public void Accept()
        {
            this.ExitCode = ApplicationComponentExitCode.Normal;
            Host.Exit();
        }

        public void Cancel()
        {
            this.ExitCode = ApplicationComponentExitCode.Cancelled;
            Host.Exit();
        }

        public bool AcceptEnabled
        {
            get { return this.Modified; }
        }

        public event EventHandler AcceptEnabledChanged
        {
            add { this.ModifiedChanged += value; }
            remove { this.ModifiedChanged -= value; }
        }
    }
}
