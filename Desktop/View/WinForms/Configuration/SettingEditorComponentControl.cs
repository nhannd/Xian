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
	/// Provides a Windows Forms user-interface for <see cref="SettingEditorComponent"/>
	/// </summary>
	public partial class SettingEditorComponentControl : ApplicationComponentUserControl
	{
		private SettingEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public SettingEditorComponentControl(SettingEditorComponent component)
			:base(component)
		{
			InitializeComponent();

			_component = component;

			base.AcceptButton = _okButton;
			base.CancelButton = _cancelButton;

			_defaultValue.DataBindings.Add("Text", _component, "DefaultValue", true, DataSourceUpdateMode.Never);
			_currentValue.DataBindings.Add("Text", _component, "CurrentValue", true, DataSourceUpdateMode.OnPropertyChanged);
			_okButton.DataBindings.Add("Enabled", _component, "Modified", true, DataSourceUpdateMode.Never);
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}