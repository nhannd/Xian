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
    public partial class PhoneNumberEditorControl : CustomUserControl
    {
        private PhoneNumberEditorComponent _component;

        public PhoneNumberEditorControl(PhoneNumberEditorComponent component)
        {
            InitializeComponent();
            _component = component;

            _use.DataSource = _component.UseChoices;
            _use.DataBindings.Add("Value", _component, "Use", true, DataSourceUpdateMode.OnPropertyChanged);

            _equipment.DataSource = _component.EquipmentChoices;
            _equipment.DataBindings.Add("Value", _component, "Equipment", true, DataSourceUpdateMode.OnPropertyChanged);

            _countryCode.DataBindings.Add("Value", _component, "CountryCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _areaCode.DataBindings.Add("Value", _component, "AreaCode", true, DataSourceUpdateMode.OnPropertyChanged);
            _number.DataBindings.Add("Value", _component, "Number", true, DataSourceUpdateMode.OnPropertyChanged);
            _extension.DataBindings.Add("Value", _component, "Extension", true, DataSourceUpdateMode.OnPropertyChanged);

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
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
