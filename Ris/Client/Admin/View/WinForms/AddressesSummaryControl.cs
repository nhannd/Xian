using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class AddressesSummaryControl : UserControl
    {
        private AddressesSummaryComponent _component;

        public AddressesSummaryControl(AddressesSummaryComponent component)
        {
            InitializeComponent();
            _component = component;

            _addressList.Table = _component.Addresses;
            _addressList.ToolbarModel = _component.AddressListToolbarActions;
            _addressList.MenuModel = _component.AddressListMenuActions;

        }

        private void AddressesSummaryControl_Load(object sender, EventArgs e)
        {
            _component.LoadAddressesTable();
        }

        private void _addressList_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedAddress(_addressList.CurrentSelection);
        }

        private void _addressList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedAddress();
        }

        private void _addButton_Click(object sender, EventArgs e)
        {
            //_component.AddAddress();
        }

        private void _updateButton_Click(object sender, EventArgs e)
        {
            //_component.UpdateAddress(_addressList.CurrentSelection);
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            //_component.DeleteAddress(_addressList.CurrentSelection);
        }
    }
}
