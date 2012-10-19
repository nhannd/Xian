#region License

// Copyright (c) 2012, ClearCanvas Inc.
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
    public partial class UserSessionsManagmentControl : ApplicationComponentUserControl
    {
        private readonly UserSessionManagmentComponent _component;

        public UserSessionsManagmentControl(UserSessionManagmentComponent component):base(component)
        {
            InitializeComponent();

            _component = component;

            _userName.Value = component.User.UserName;
            _displayName.Value = component.User.DisplayName;
            _lastLogin.Value = component.User.LastLoginTime.HasValue
                            ? component.User.LastLoginTime.Value.ToString(ClearCanvas.Desktop.Format.DateTimeFormat)
                            : SR.LabelUnknownLastLoginTime;


            _sessionsTable.Table = component.SummaryTable;
            _sessionsTable.MenuModel = component.SummaryTableActionModel;
        	_sessionsTable.ToolbarModel = component.SummaryTableActionModel;
            _sessionsTable.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);
        }

		private void _closeButton_Click(object sender, EventArgs e)
		{
			_component.Close();
		}
    }
}
