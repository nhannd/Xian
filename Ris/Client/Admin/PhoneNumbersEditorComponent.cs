using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Admin
{
    public class PhoneNumbersEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(PhoneNumbersEditorComponentViewExtensionPoint))]
    public class PhoneNumbersEditorComponent : ApplicationComponent
    {
        TelephoneNumber _phoneNumber;
        static IPatientAdminService _patientAdminService;

        static PhoneNumbersEditorComponent()
        {
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public PhoneNumbersEditorComponent(TelephoneNumber phoneNumber)
        {
            _phoneNumber = phoneNumber;
        }

        public TelephoneNumber PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public string CountryCode
        {
            get { return _phoneNumber.CountryCode; }
            set { _phoneNumber.CountryCode = value; }
        }

        public string AreaCode
        {
            get { return _phoneNumber.AreaCode; }
            set { _phoneNumber.AreaCode = value; }
        }

        public string Number
        {
            get { return _phoneNumber.Number; }
            set { _phoneNumber.Number = value; }
        }

        public string Extension
        {
            get { return _phoneNumber.Extension; }
            set { _phoneNumber.Extension = value; }
        }

        public string Use
        {
            get { return _patientAdminService.TelephoneUseEnumTable[_phoneNumber.Use].Value; }
            set { _phoneNumber.Use = _patientAdminService.TelephoneUseEnumTable[value].Code; }
        }

        public string[] UseChoices
        {
            get { return _patientAdminService.TelephoneUseEnumTable.Values; }
        }

        public string Equipment
        {
            get { return _patientAdminService.TelephoneEquipmentEnumTable[_phoneNumber.Equipment].Value; }
            set { _phoneNumber.Equipment = _patientAdminService.TelephoneEquipmentEnumTable[value].Code; }
        }

        public string[] EquipmentChoices
        {
            get { return _patientAdminService.TelephoneEquipmentEnumTable.Values; }
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
