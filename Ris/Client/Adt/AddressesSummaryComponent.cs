using System;
using System.Collections.Generic;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Adt
{
    public class AddressesSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(AddressesSummaryComponentViewExtensionPoint))]
    public class AddressesSummaryComponent : ApplicationComponent
    {
        private PatientProfile _patient;
        private AddressTable _addresses;
        private Address _currentAddressSelection;
        private CrudActionModel _addressActionHandler;

        public AddressesSummaryComponent()
        {
            _addresses = new AddressTable();

            _addressActionHandler = new CrudActionModel();
            _addressActionHandler.Add.SetClickHandler(AddAddress);
            _addressActionHandler.Edit.SetClickHandler(UpdateSelectedAddress);
            _addressActionHandler.Delete.SetClickHandler(DeleteSelectedAddress);

            _addressActionHandler.Add.Enabled = true;
        }

        public PatientProfile Subject
        {
            get { return _patient; }
            set { _patient = value; }
        }
        
        public ITable Addresses
        {
            get { return _addresses; }
        }

        public ActionModelNode AddressListActionModel
        {
            get { return _addressActionHandler; }
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
                _addressActionHandler.Edit.Enabled = true;
                _addressActionHandler.Delete.Enabled = true;
            }
            else
            {
                _addressActionHandler.Edit.Enabled = false;
                _addressActionHandler.Delete.Enabled = false;
            }
        }

        public void AddAddress()
        {
            Address address = new Address();

            // For now, hard code default values that correspond to first entry in the the _dummy*Choices string arrays in AddressesEditorComponent
            address.Province = "Ontario";
            address.Country = "Canada";

            AddressEditorComponent editor = new AddressEditorComponent(address);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddAddress);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _addresses.Items.Add(address);
                _patient.Addresses.Add(address);
                this.Modified = true;
            }
        }

        public void UpdateSelectedAddress()
        {
            // can occur if user double clicks while holding control
            if (_currentAddressSelection == null) return;

            Address address = (Address)_currentAddressSelection.Clone();

            AddressEditorComponent editor = new AddressEditorComponent(address);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleUpdateAddress);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                // delete and re-insert to ensure that TableView updates correctly
                Address toBeRemoved = _currentAddressSelection;
                _addresses.Items.Remove(toBeRemoved);
                _patient.Addresses.Remove(toBeRemoved);

                _addresses.Items.Add(address);
                _patient.Addresses.Add(address);

                this.Modified = true;
            }
        }

        public void DeleteSelectedAddress()
        {
            if (this.Host.ShowMessageBox( SR.MessageDeleteSelectedAddress, MessageBoxActions.YesNo) == DialogBoxAction.Yes)
            {
                //  Must use temporary Address otherwise as a side effect TableDate.Remove() will change the current selection 
                //  resulting in the wrong Address being removed from the PatientProfile
                Address toBeRemoved  = _currentAddressSelection;
                _addresses.Items.Remove(toBeRemoved);
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
                    _addresses.Items.Add(address);
                }
            }
        }
    }
}
