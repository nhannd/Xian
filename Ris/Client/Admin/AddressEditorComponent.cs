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

    [AssociateView(typeof(AddressesEditorComponentViewExtensionPoint))]
    public class AddressEditorComponent : ApplicationComponent
    {
        Address _address;
        private string[] _dummyProvinceChoices = new string[] { "Ontario", "Alberta", "British Columbia", "Manitoba", "New Brunswick", "Newfoundland", "Nova Scotia", "PEI", "Quebec", "Saskatchewan" };
        private string[] _dummyCountryChoices = new string[] { "Canada" };
        private IPatientAdminService _patientAdminService;

        public AddressEditorComponent(Address address)
        {
            _address = address;
        }

        /// <summary>
        /// Sets the subject upon which the editor acts
        /// Not for use by the view
        /// </summary>
        public Address Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public override void Start()
        {
            base.Start();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public string Street
        {
            get { return _address.Street; }
            set
            {
                _address.Street = value;
                this.Modified = true;
            }
        }

        public string City
        {
            get { return _address.City; }
            set
            {
                _address.City = value;
                this.Modified = true;
            }
        }

        public string Province
        {
            get { return _address.Province; }
            set
            {
                _address.Province = value;
                this.Modified = true;
            }
        }

        public string[] ProvinceChoices
        {
            get { return _dummyProvinceChoices; }
        }

        public string Country
        {
            get { return _address.Country; }
            set
            {
                _address.Country = value;
                this.Modified = true;
            }
        }

        public string[] CountryChoices
        {
            get { return _dummyCountryChoices; }
        }

        public string PostalCode
        {
            get { return _address.PostalCode; }
            set
            {
                _address.PostalCode = value;
                this.Modified = true;
            }
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
            set
            {
                _address.Type = _patientAdminService.AddressTypeEnumTable[value].Code;
                this.Modified = true;
            }
        }

        public string[] TypeChoices
        {
            get { return _patientAdminService.AddressTypeEnumTable.Values; }
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
