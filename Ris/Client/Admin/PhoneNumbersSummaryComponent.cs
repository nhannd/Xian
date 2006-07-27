using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Admin
{
    public class PhoneNumbersSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(PhoneNumbersSummaryComponentViewExtensionPoint))]
    public class PhoneNumbersSummaryComponent : ApplicationComponent
    {
        private Patient _patient;
        private IPatientAdminService _patientAdminService;
        private TableData<TelephoneNumber> _phoneNumbers;

        public PhoneNumbersSummaryComponent()
        {
            _phoneNumbers = new TableData<TelephoneNumber>();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();

            _phoneNumbers.AddColumn<string>("CountryCode", delegate(TelephoneNumber pn) { return pn.CountryCode; });
            _phoneNumbers.AddColumn<string>("AreaCode", delegate(TelephoneNumber pn) { return pn.AreaCode; });
            _phoneNumbers.AddColumn<string>("Number", delegate(TelephoneNumber pn) { return pn.Number; });
            _phoneNumbers.AddColumn<string>("Extension", delegate(TelephoneNumber pn) { return pn.Extension; });
            _phoneNumbers.AddColumn<string>("Use", delegate(TelephoneNumber pn) { return _patientAdminService.TelephoneUseEnumTable[pn.Use].Value; });
            _phoneNumbers.AddColumn<string>("Equipment", delegate(TelephoneNumber pn) { return _patientAdminService.TelephoneEquipmentEnumTable[pn.Equipment].Value; });
        }

        public Patient Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }

        public ITableData PhoneNumbers
        {
            get { return _phoneNumbers; }
        }

        public void AddPhoneNumber()
        {
            TelephoneNumber phoneNumber = TelephoneNumber.New();

            //provide reasonable defaults for following two fields until mapping file modified to nullable
            phoneNumber.CountryCode = "1";
            phoneNumber.Extension = "N/A";

            PhoneNumbersEditorComponent editor = new PhoneNumbersEditorComponent(phoneNumber);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Add Phone Number...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _phoneNumbers.Add(phoneNumber);
                _patient.TelephoneNumbers.Add(phoneNumber);
                this.Modified = true;
            }
        }

        public void UpdatePhoneNumber(ISelection selection)
        {
            TelephoneNumber phoneNumber = TelephoneNumber.New();
            TelephoneNumber selectedPhoneNumber = (TelephoneNumber)selection.Item;
            phoneNumber.CopyFrom(selectedPhoneNumber);
            
            PhoneNumbersEditorComponent editor = new PhoneNumbersEditorComponent(phoneNumber);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Update Phone Number...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                selectedPhoneNumber.CopyFrom(phoneNumber);
                this.Modified = true;
            }
        }

        public void DeleteNumber(ISelection selection)
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this phone number?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                TelephoneNumber phoneNumber  = (TelephoneNumber)selection.Item;
                _phoneNumbers.Remove(phoneNumber);
                _patient.TelephoneNumbers.Remove(phoneNumber);
                this.Modified = true;
            }
        }

        public void LoadPhoneNumbersTable()
        {
            if (_patient != null)
            {
                foreach (TelephoneNumber phoneNumber in _patient.TelephoneNumbers)
                {
                    _phoneNumbers.Add(phoneNumber);
                }
            }
        }


    }
}
