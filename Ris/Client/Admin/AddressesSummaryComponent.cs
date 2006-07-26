using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;

namespace ClearCanvas.Ris.Client.Admin
{
    public class AddressesEditorComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(AddressesEditorComponentViewExtensionPoint))]
    public class AddressesSummaryComponent : ApplicationComponent
    {
        private Patient _patient;
        private IPatientAdminService _patientAdminService;
        private TableData<AddressesTableEntry> _addresses;

        public AddressesSummaryComponent()
        {
            _addresses = new TableData<AddressesTableEntry>();
        }

        public Patient Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }
        
        public ITableData Addresses
        {
            get { return _addresses; }
        }

        public void AddAddress()
        {
            Address temp = Address.New();
            temp.City = "Toronto";
            temp.Country = "Canada";
            temp.PostalCode = "M5B 2H5";
            temp.Province = "Ontario";
            temp.Street = "20 Carlton St.";
            temp.Type = AddressType.B;
            temp.ValidFrom = DateTime.Now;
            temp.ValidUntil = DateTime.Now;

            _addresses.Add(new AddressesTableEntry(temp));
            _patient.Addresses.Add(temp);
        }

        public void UpdateAddress(ISelection selection)
        {
        }

        public void DeleteAddress(ISelection selection)
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this address?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                AddressesTableEntry entry = (AddressesTableEntry)selection.Item;
                Address address = entry.Address;
                _addresses.Remove(entry);
                _patient.Addresses.Remove(address);
            }
        }

        public void LoadAddressesTable()
        {
            if (_patient != null)
            {
                foreach (Address address in _patient.Addresses)
                {
                    _addresses.Add(new AddressesTableEntry(address));
                }
            }
        }
    }
}
