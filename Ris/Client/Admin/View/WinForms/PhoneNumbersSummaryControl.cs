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
        }

        private void _addButton_Click(object sender, EventArgs e)
        {
            _component.AddPhoneNumber();
        }

        private void _updateButton_Click(object sender, EventArgs e)
        {
            _component.UpdatePhoneNumber(_phoneNumbers.CurrentSelection);
        }

        private void _deleteButton_Click(object sender, EventArgs e)
        {
            _component.DeleteNumber(_phoneNumbers.CurrentSelection);
        }

        private void PhoneNumbersSummaryControl_Load(object sender, EventArgs e)
        {
            _component.LoadPhoneNumbersTable();
        }

        private void _phoneNumbers_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdatePhoneNumber(_phoneNumbers.CurrentSelection);
        }
    }
}
