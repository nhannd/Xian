#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Enterprise.Desktop.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="UserEditorComponent"/>
    /// </summary>
    public partial class UserEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly UserEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserEditorComponentControl(UserEditorComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _userId.DataBindings.Add("Value", _component, "UserId", true, DataSourceUpdateMode.OnPropertyChanged);
            _userId.DataBindings.Add("ReadOnly", _component, "IsUserIdReadOnly", true, DataSourceUpdateMode.OnPropertyChanged);

			_displayName.DataBindings.Add("Value", _component, "DisplayName", true, DataSourceUpdateMode.OnPropertyChanged);

            _emailAddress.DataBindings.Add("Value", _component, "EmailAddress", true, DataSourceUpdateMode.OnPropertyChanged);

            _validFrom.DataBindings.Add("Value", _component, "ValidFrom", true, DataSourceUpdateMode.OnPropertyChanged);
            _validUntil.DataBindings.Add("Value", _component, "ValidUntil", true, DataSourceUpdateMode.OnPropertyChanged);
            _accountEnabledCheckBox.DataBindings.Add("Checked", _component, "AccountEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _passwordExpiryDate.DataBindings.Add("Value", _component, "PasswordExpiryTime", true, DataSourceUpdateMode.OnPropertyChanged);

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
