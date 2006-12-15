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

            _addressList.ToolbarModel = _component.AddressListActionModel;
            _addressList.MenuModel = _component.AddressListActionModel;

            //_addressList.DataBindings.Add("Table", _component, "Addresses", true, DataSourceUpdateMode.OnPropertyChanged);
            _addressList.Table = _component.Addresses;
            _addressList.DataBindings.Add("Selection", _component, "SelectedAddress", true, DataSourceUpdateMode.OnPropertyChanged);

            _errorProvider.DataSource = _component;
        }

        private void AddressesSummaryControl_Load(object sender, EventArgs e)
        {
            //_component.LoadAddressesTable();
        }

        private void _addressList_SelectionChanged(object sender, EventArgs e)
        {
            //_component.SetSelectedAddress(_addressList.Selection);
        }

        private void _addressList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedAddress();
        }
    }
}
