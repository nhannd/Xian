using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PhoneNumbersEditorControl : UserControl
    {
        private PhoneNumbersEditorComponent _component;

        public PhoneNumbersEditorControl(PhoneNumbersEditorComponent component)
        {
            InitializeComponent();
            _component = component;

            _use.DataSource = _component.UseChoices;
            _use.DataBindings.Add("Value", _component, "Use", false, DataSourceUpdateMode.OnPropertyChanged);

            _equipment.DataSource = _component.EquipmentChoices;
            _equipment.DataBindings.Add("Value", _component, "Equipment", false, DataSourceUpdateMode.OnPropertyChanged);

            _countryCode.DataBindings.Add("Value", _component, "CountryCode", false, DataSourceUpdateMode.OnPropertyChanged);
            _areaCode.DataBindings.Add("Value", _component, "AreaCode", false, DataSourceUpdateMode.OnPropertyChanged);
            _number.DataBindings.Add("Value", _component, "Number", false, DataSourceUpdateMode.OnPropertyChanged);
            _extension.DataBindings.Add("Value", _component, "Extension", false, DataSourceUpdateMode.OnPropertyChanged);
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
