using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;

namespace ClearCanvas.Ris.Client.Admin
{
    public class AddressesSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [ApplicationComponentView(typeof(AddressesSummaryComponentViewExtensionPoint))]
    public class AddressesSummaryComponent : ApplicationComponent
    {
        private Patient _patient;
        private IPatientAdminService _patientAdminService;
        private TableData<Address> _addresses;

        public AddressesSummaryComponent()
        {
            _addresses = new TableData<Address>();
            _patientAdminService = ApplicationContext.GetService<IPatientAdminService>();

            _addresses.AddColumn<string>("Type", delegate(Address a) { return _patientAdminService.AddressTypeEnumTable[a.Type].Value; });
            _addresses.AddColumn<string>("Street", delegate(Address a) { return a.Street; });
            _addresses.AddColumn<string>("City", delegate(Address a) { return a.City; });
            _addresses.AddColumn<string>("Province", delegate(Address a) { return a.Province; });
            _addresses.AddColumn<string>("PostalCode", delegate(Address a) { return a.PostalCode; });
            _addresses.AddColumn<string>("Country", delegate(Address a) { return a.Country; });
            _addresses.AddColumn<string>("ValidFrom", delegate(Address a) { return a.ValidFrom; });
            _addresses.AddColumn<string>("ValidUntil", delegate(Address a) { return a.ValidUntil; });
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
            Address address = Address.New();

            // For now, hard code default values that correspond to first entry in the the _dummy*Choices string arrays in AddressesEditorComponent
            address.Province = "Ontario";
            address.Country = "Canada";

            AddressesEditorComponent editor = new AddressesEditorComponent(address);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Add Address...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _addresses.Add(address);
                _patient.Addresses.Add(address);
                this.Modified = true;
            }
        }

        public void UpdateAddress(ISelection selection)
        {
            Address address = (Address)selection.Item;
            AddressesEditorComponent editor = new AddressesEditorComponent(address);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Update Address...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                this.Modified = true;
            }
        }

        public void DeleteAddress(ISelection selection)
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this address?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                Address address  = (Address)selection.Item;
                _addresses.Remove(address);
                _patient.Addresses.Remove(address);
                this.Modified = true;
            }
        }

        public void LoadAddressesTable()
        {
            if (_patient != null)
            {
                foreach (Address address in _patient.Addresses)
                {
                    _addresses.Add(address);
                }
            }
        }
    }
}
