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
    public partial class PhoneNumbersSummaryControl : ApplicationComponentUserControl
    {
        PhoneNumbersSummaryComponent _component;

        public PhoneNumbersSummaryControl(PhoneNumbersSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _phoneNumbers.ToolbarModel = _component.PhoneNumberListActionModel;
            _phoneNumbers.MenuModel = _component.PhoneNumberListActionModel;

            _phoneNumbers.Table = _component.PhoneNumbers;
            _phoneNumbers.DataBindings.Add("Selection", _component, "SelectedPhoneNumber", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _phoneNumbers_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedPhoneNumber();
        }
    }
}
