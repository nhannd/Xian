#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;
using ClearCanvas.Desktop.Configuration;

namespace ClearCanvas.Desktop.View.WinForms.Configuration
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="SettingsManagementComponent"/>
	/// </summary>
	public partial class SettingsManagementComponentControl : ApplicationComponentUserControl
	{
		private SettingsManagementComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public SettingsManagementComponentControl(SettingsManagementComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;

			_settingsGroupTableView.Table = _component.SettingsGroupTable;
			_settingsGroupTableView.DataBindings.Add("Selection", _component, "SelectedSettingsGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			_settingsGroupTableView.ToolbarModel = _component.SettingsGroupsActionModel;

			_valueTableView.Table = _component.SettingsPropertiesTable;
			_valueTableView.DataBindings.Add("Selection", _component, "SelectedSettingsProperty", true, DataSourceUpdateMode.OnPropertyChanged);
			_valueTableView.ToolbarModel = _component.SettingsPropertiesActionModel;

			_valueTableView.ItemDoubleClicked += new EventHandler(ValueTableItemDoubleClicked);
		}

		private void ValueTableItemDoubleClicked(object sender, EventArgs e)
		{
			_component.SettingsPropertyDoubleClicked();
		}
	}
}