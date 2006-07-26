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

            _addressList.DataSource = _component.Addresses;
        }

        private void _addButton_Click(object sender, EventArgs e)
        {
            _component.AddAddress();
        }

        private void _updateButton_Click(object sender, EventArgs e)
        {
            _component.UpdateAddress(_addressList.CurrentSelection);
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            _component.DeleteAddress(_addressList.CurrentSelection);
        }

        private void _addresses_DoubleClick(object sender, EventArgs e)
        {
            _component.UpdateAddress(_addressList.CurrentSelection);
        }

        private void AddressesEditorControl_Load(object sender, EventArgs e)
        {
            _component.LoadAddressesTable();
        }
    }
}
