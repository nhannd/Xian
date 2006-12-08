using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    public partial class AddressesSummaryControl : UserControl
    {
        private AddressesSummaryComponent _component;

        public AddressesSummaryControl(AddressesSummaryComponent component)
        {
            InitializeComponent();
            _component = component;

            _addressList.Table = _component.Addresses;
            _addressList.ToolbarModel = _component.AddressListActionModel;
            _addressList.MenuModel = _component.AddressListActionModel;

            // need to manually subscribe to this event, since the addressList is not a simple-bound control
            _component.ValidationVisibleChanged += new EventHandler(_component_ShowValidationChanged);
        }

        private void _component_ShowValidationChanged(object sender, EventArgs e)
        {
            // need to manually manage the errorprovider for the addressList, since it is not a simple-bound control
            string msg = _component.ValidationVisible ? (_component as IDataErrorInfo)["Addresses"] : null;
            _errorProvider.SetError(_addressList, msg);
        }

        private void AddressesSummaryControl_Load(object sender, EventArgs e)
        {
            _component.LoadAddressesTable();
        }

        private void _addressList_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedAddress(_addressList.Selection);
        }

        private void _addressList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedAddress();
        }
    }
}
