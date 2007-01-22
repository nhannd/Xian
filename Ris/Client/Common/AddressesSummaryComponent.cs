using System;
using System.Collections;
using System.Text;

using ClearCanvas.Common;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.Tools;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Healthcare;
using ClearCanvas.Enterprise;
using ClearCanvas.Ris.Services;
using ClearCanvas.Desktop.Tables;

namespace ClearCanvas.Ris.Client.Common
{
    public class AddressesSummaryComponentViewExtensionPoint : ExtensionPoint<IApplicationComponentView>
    {
    }

    [AssociateView(typeof(AddressesSummaryComponentViewExtensionPoint))]
    public class AddressesSummaryComponent : ApplicationComponent
    {
        private IList _addressList;
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
            _addressActionHandler.Edit.Enabled = false;
            _addressActionHandler.Delete.Enabled = false;
        }

        public IList Subject
        {
            get { return _addressList; }
            set { _addressList = value; }
        }

        public override void Start()
        {
            if (_addressList != null)
            {
                foreach (Address address in _addressList)
                {
                    _addresses.Items.Add(address);
                }
            }
            base.Start();
        }

        #region Presentation Model

        public ITable Addresses
        {
            get { return _addresses; }
        }

        public ActionModelNode AddressListActionModel
        {
            get { return _addressActionHandler; }
        }

        public ISelection SelectedAddress
        {
            get { return _currentAddressSelection == null ? Selection.Empty : new Selection(_currentAddressSelection); }
            set
            {
                _currentAddressSelection = (Address)value.Item;
                AddressSelectionChanged();
            }
        }

        public void AddAddress()
        {
            Address address = new Address();

            AddressEditorComponent editor = new AddressEditorComponent(address);
            ApplicationComponentExitCode exitCode = ApplicationComponent.LaunchAsDialog(this.Host.DesktopWindow, editor, SR.TitleAddAddress);
            if (exitCode == ApplicationComponentExitCode.Normal)
            {
                _addresses.Items.Add(address);
                _addressList.Add(address);
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
                _addressList.Remove(toBeRemoved);

                _addresses.Items.Add(address);
                _addressList.Add(address);

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
                _addressList.Remove(toBeRemoved);
                this.Modified = true;
            }
        }

        #endregion

        private void AddressSelectionChanged()
        {
            _addressActionHandler.Edit.Enabled =
                _addressActionHandler.Delete.Enabled = (_currentAddressSelection != null);
        }
    }
}
