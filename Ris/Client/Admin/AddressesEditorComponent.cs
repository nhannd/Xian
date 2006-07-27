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
    public class AddressesEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(AddressesEditorComponentViewExtensionPoint))]
    public class AddressesEditorComponent : ApplicationComponent
    {
        Address _address;
        private string[] _dummyProvinceChoices = new string[] { "Ontario", "Alberta", "British Columbia", "Manitoba", "New Brunswick", "Newfoundland", "Nova Scotia", "PEI", "Quebec", "Saskatchewan" };
        private string[] _dummyCountryChoices = new string[] { "Canada" };
        static IPatientAdminService _patientAdminService;

        static AddressesEditorComponent()
        {
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public AddressesEditorComponent(Address address)
        {
            _address = address;
        }

        public Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string Street
        {
            get { return _address.Street; }
            set { _address.Street = value; }
        }

        public string City
        {
            get { return _address.City; }
            set { _address.City = value; }
        }

        public string Province
        {
            get { return _address.Province; }
            set { _address.Province = value; }
        }

        public string[] ProvinceChoices
        {
            get { return _dummyProvinceChoices; }
        }

        public string Country
        {
            get { return _address.Country; }
            set { _address.Country = value; }
        }

        public string[] CountryChoices
        {
            get { return _dummyCountryChoices; }
        }

        public string PostalCode
        {
            get { return _address.PostalCode; }
            set { _address.PostalCode = value; }
        }

        //public DateTime? ValidFrom
        //{
        //    get { return _address.ValidFrom; }
        //    set { _address.ValidFrom = value; }
        //}

        //public DateTime? ValidUntil
        //{
        //    get { return _address.ValidUntil; }
        //    set { _address.ValidUntil = value; }
        //}

        public string Type
        {
            get { return _patientAdminService.AddressTypeEnumTable[_address.Type].Value; }
            set { _address.Type = _patientAdminService.AddressTypeEnumTable[value].Code; }
        }

        public string[] TypeChoices
        {
            get { return _patientAdminService.AddressTypeEnumTable.Values; }
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
