using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
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
        class AddressActionHandler : CrudActionHandler
        {
            private AddressesSummaryComponent _component;

            internal AddressActionHandler(AddressesSummaryComponent component)
            {
                _component = component;
            }

            protected override void Add()
            {
                _component.AddAddress();
            }

            protected override void Edit()
            {
                _component.UpdateSelectedAddress();
            }

            protected override void Delete()
            {
                _component.DeleteSelectedAddress();
            }
        }

        private Patient _patient;
        private IPatientAdminService _patientAdminService;
        private TableData<Address> _addresses;
        private Address _currentAddressSelection;

        private AddressActionHandler _addressActionHandler;

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
            //_addresses.AddColumn<string>("ValidFrom", delegate(Address a) { return a.ValidFrom; });
            //_addresses.AddColumn<string>("ValidUntil", delegate(Address a) { return a.ValidUntil; });

            _addressActionHandler = new AddressActionHandler(this);

            _addressActionHandler.AddEnabled = true;
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

        public ActionModelNode AddressActions
        {
            get { return _addressActionHandler.ActionModel; }
        }

        public Address CurrentAddressSelection
        {
            get { return _currentAddressSelection; }
            set
            {
                _currentAddressSelection = value;
                AddressSelectionChanged();
            }
        }

        public void SetSelectedAddress(ISelection selection)
        {
            this.CurrentAddressSelection = (Address)selection.Item;
        }

        private void AddressSelectionChanged()
        {
            if (_currentAddressSelection != null)
            {
                _addressActionHandler.EditEnabled = true;
                _addressActionHandler.DeleteEnabled = true;
            }
            else
            {
                _addressActionHandler.EditEnabled = false;
                _addressActionHandler.DeleteEnabled = false;
            }
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

        public void UpdateSelectedAddress()
        {
            Address address = Address.New();
            address.CopyFrom(_currentAddressSelection);

            AddressesEditorComponent editor = new AddressesEditorComponent(address);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(editor, "Update Address...");
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                //_currentAddressSelection.CopyFrom(address);

                // delete and re-insert to ensure that TableView updates correctly
                Address toBeRemoved = _currentAddressSelection;
                _addresses.Remove(toBeRemoved);
                _patient.Addresses.Remove(toBeRemoved);

                _addresses.Add(address);
                _patient.Addresses.Add(address);

                this.Modified = true;
            }
        }

        public void DeleteSelectedAddress()
        {
            if (this.Host.ShowMessageBox("Are you sure you want to delete this address?", MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                Address toBeRemoved  = _currentAddressSelection;
                _addresses.Remove(toBeRemoved);
                _patient.Addresses.Remove(toBeRemoved);
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
