using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class AddressesEditorControl : UserControl
    {
        private AddressesEditorComponent _component;

        public AddressesEditorControl(AddressesEditorComponent component)
        {
            InitializeComponent();
            _component = component;

            _type.DataSource = _component.TypeChoices;
            _type.DataBindings.Add("Value", _component, "Type", false, DataSourceUpdateMode.OnPropertyChanged);

            _province.DataSource = _component.ProvinceChoices;
            _province.DataBindings.Add("Value", _component, "Province", false, DataSourceUpdateMode.OnPropertyChanged);

            _country.DataSource = _component.CountryChoices;
            _country.DataBindings.Add("Value", _component, "Country", false, DataSourceUpdateMode.OnPropertyChanged);

            _street.DataBindings.Add("Value", _component, "Street", false, DataSourceUpdateMode.OnPropertyChanged);
            _city.DataBindings.Add("Value", _component, "City", false, DataSourceUpdateMode.OnPropertyChanged);
            _postalCode.DataBindings.Add("Value", _component, "PostalCode", false, DataSourceUpdateMode.OnPropertyChanged);
            //_validFrom.DataBindings.Add("Value", _component, "ValidFrom", true, DataSourceUpdateMode.OnPropertyChanged);
            //_validUntil.DataBindings.Add("Value", _component, "ValidUntil", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
