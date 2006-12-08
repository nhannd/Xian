using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Adt.View.WinForms
{
    public partial class PhoneNumbersSummaryControl : UserControl
    {
        PhoneNumbersSummaryComponent _component;

        public PhoneNumbersSummaryControl(PhoneNumbersSummaryComponent component)
        {
            InitializeComponent();
            _component = component;

            _phoneNumbers.Table = _component.PhoneNumbers;
            _phoneNumbers.ToolbarModel = _component.PhoneNumberListActionModel;
            _phoneNumbers.MenuModel = _component.PhoneNumberListActionModel;

            // need to manually subscribe to this event, since the addressList is not a simple-bound control
            _component.ValidationVisibleChanged += new EventHandler(_component_ShowValidationChanged);
        }

        private void _component_ShowValidationChanged(object sender, EventArgs e)
        {
            // need to manually manage the errorprovider for the addressList, since it is not a simple-bound control
            string msg = _component.ValidationVisible ? (_component as IDataErrorInfo)["PhoneNumbers"] : null;
            _errorProvider.SetError(_phoneNumbers, msg);
        }
        
        private void PhoneNumbersSummaryControl_Load(object sender, EventArgs e)
        {
            _component.LoadPhoneNumbersTable();
        }

        private void _phoneNumbers_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedPhoneNumber();
        }

        private void _phoneNumbers_SelectionChanged(object sender, EventArgs e)
        {
            _component.SetSelectedPhoneNumber(_phoneNumbers.Selection);
        }
    }
}
