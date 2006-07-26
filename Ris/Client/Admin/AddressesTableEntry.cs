using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Healthcare;
using ClearCanvas.Desktop;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Admin
{
    class AddressesTableEntry
    {
        Address _address;

        private static IPatientAdminService _patientAdminService;

        static AddressesTableEntry()
        {
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();
        }

        public Address Address
        {
            get { return _address; }
        }

        public AddressesTableEntry(Address address)
        {
            _address = address;
        }

        [TableColumn("Street")]
        public String Street
        {
            get { return _address.Street; }
        }

        [TableColumn("City")]
        public String City
        {
            get { return _address.City; }
        }

        [TableColumn("Province")]
        public String Province
        {
            get { return _address.Province; }
        }

        [TableColumn("PostalCode")]
        public String PostalCode
        {
            get { return _address.PostalCode; }
        }

        [TableColumn("Country")]
        public String Country
        {
            get { return _address.Country; }
        }

        [TableColumn("Type")]
        public String Type
        {
            get { return _patientAdminService.AddressTypeEnumTable[_address.Type].Value; }
        }

        [TableColumn("ValidFrom")]
        public String ValidFrom
        {
            get { return _address.ValidFrom.ToString(); }
        }

        [TableColumn("ValidUntil")]
        public String ValidUntil
        {
            get { return _address.ValidUntil.ToString(); }
        }
    }
}
