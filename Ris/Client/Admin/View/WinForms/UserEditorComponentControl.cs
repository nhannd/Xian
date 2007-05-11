using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="UserEditorComponent"/>
    /// </summary>
    public partial class UserEditorComponentControl : ApplicationComponentUserControl
    {
        private UserEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserEditorComponentControl(UserEditorComponent component)
        {
            InitializeComponent();

            _component = component;

            _userId.DataBindings.Add("Value", _component, "UserId", true, DataSourceUpdateMode.OnPropertyChanged);
            _userId.DataBindings.Add("ReadOnly", _component, "IsUserIdReadOnly", true, DataSourceUpdateMode.OnPropertyChanged);
            _staffName.DataBindings.Add("Value", _component, "StaffName", true, DataSourceUpdateMode.OnPropertyChanged);

            _authorityGroups.Table = _component.Groups;

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
            _clearStaffButton.DataBindings.Add("Enabled", _component, "ClearStaffEnabled");

            // TODO: enable these when functionality implemented on server
            _password.Enabled = false;
            _confirmPassword.Enabled = false;
            _validFrom.Enabled = false;
            _validUntil.Enabled = false;
        }

        private void _acceptButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _staffButton_Click(object sender, EventArgs e)
        {
            _component.SetStaff();
        }

        private void _clearStaffButton_Click(object sender, EventArgs e)
        {
            _component.ClearStaff();
        }
    }
}
