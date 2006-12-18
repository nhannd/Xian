using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    public partial class AddressesSummaryControl : ApplicationComponentUserControl
    {
        private AddressesSummaryComponent _component;

        public AddressesSummaryControl(AddressesSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _addressList.ToolbarModel = _component.AddressListActionModel;
            _addressList.MenuModel = _component.AddressListActionModel;

            _addressList.Table = _component.Addresses;
            _addressList.DataBindings.Add("Selection", _component, "SelectedAddress", true, DataSourceUpdateMode.OnPropertyChanged);

            // move the error icon up to the top of the table, rather than the middle
            this.ErrorProvider.SetIconAlignment(_addressList, ErrorIconAlignment.TopRight);
        }

        private void _addressList_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedAddress();
        }
    }
}
