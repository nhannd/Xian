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
    public partial class UserEditorComponentControl : CustomUserControl
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
            _userId.DataBindings.Add("Enabled", _component, "IsUserIdEditable", true, DataSourceUpdateMode.OnPropertyChanged);

            _familyName.DataBindings.Add("Value", _component, "FamilyName", true, DataSourceUpdateMode.OnPropertyChanged);
            _givenName.DataBindings.Add("Value", _component, "GivenName", true, DataSourceUpdateMode.OnPropertyChanged);
            _middleName.DataBindings.Add("Value", _component, "MiddleName", true, DataSourceUpdateMode.OnPropertyChanged);
            _prefix.DataBindings.Add("Value", _component, "Prefix", true, DataSourceUpdateMode.OnPropertyChanged);
            _suffix.DataBindings.Add("Value", _component, "Suffix", true, DataSourceUpdateMode.OnPropertyChanged);
            _degree.DataBindings.Add("Value", _component, "Degree", true, DataSourceUpdateMode.OnPropertyChanged);

            _authorityGroups.Table = _component.Groups;

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
