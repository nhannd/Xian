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
    /// Provides a Windows Forms user-interface for <see cref="UserSummaryComponent"/>
    /// </summary>
    public partial class UserSummaryComponentControl : CustomUserControl
    {
        private UserSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserSummaryComponentControl(UserSummaryComponent component)
        {
            InitializeComponent();

            _component = component;

            _users.ToolbarModel = _component.UserListActionModel;
            _users.MenuModel = _component.UserListActionModel;

            _users.Table = _component.Users;
            _users.DataBindings.Add("Selection", _component, "SelectedUser", true, DataSourceUpdateMode.OnPropertyChanged);

            // TODO add .NET databindings to _component
        }

        private void _users_Load(object sender, EventArgs e)
        {
            _component.LoadUserTable();
        }

        private void _users_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.UpdateSelectedUser();
        }
    }
}
