using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    public partial class PhoneNumbersSummaryControl : UserControl
    {
        PhoneNumbersSummaryComponent _component;

        public PhoneNumbersSummaryControl(PhoneNumbersSummaryComponent component)
        {
            InitializeComponent();
            _component = component;

            _phoneNumbers.DataSource = _component.PhoneNumbers;
            _phoneNumbers.ToolbarModel = _component.PhoneNumberListToolbarActions;
            _phoneNumbers.MenuModel = _component.PhoneNumberListMenuActions;

        }

        private void _addButton_Click(object sender, EventArgs e)
        {
            _component.AddPhoneNumber();
        }

        private void _updateButton_Click(object sender, EventArgs e)
        {
//            _component.UpdateSelectedPhoneNumber(_phoneNumbers.CurrentSelection);
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            //_component.DeleteSelectedPhoneNumber(_phoneNumbers.CurrentSelection);
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
            _component.SetSelectedPhoneNumber(_phoneNumbers.CurrentSelection);
        }
    }
}
