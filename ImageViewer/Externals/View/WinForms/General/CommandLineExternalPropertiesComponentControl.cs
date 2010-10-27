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
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Externals.General;
using ClearCanvas.ImageViewer.Externals.View.WinForms.Properties;
using MessageBox=System.Windows.Forms.MessageBox;

namespace ClearCanvas.ImageViewer.Externals.View.WinForms.General
{
	public partial class CommandLineExternalPropertiesComponentControl : ApplicationComponentUserControl
	{
		private readonly string _helpMessage = string.Empty;

		public CommandLineExternalPropertiesComponentControl(CommandLineExternalPropertiesComponent component) : base(component)
		{
			InitializeComponent();

			base.ErrorProvider.SetIconPadding(_txtCommand, _btnCommand.Width);

			_helpMessage = component.ArgumentFieldsHelpText;
			_lnkHelpFields.Visible = !string.IsNullOrEmpty(_helpMessage);

			_txtName.DataBindings.Add("Text", component, "Label", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtCommand.DataBindings.Add("Text", component, "Command", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtWorkingDir.DataBindings.Add("Text", component, "WorkingDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtArguments.DataBindings.Add("Text", component, "Arguments", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkAllowMultiValueFields.DataBindings.Add("Checked", component, "AllowMultiValueFields", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtMultiValueFieldSeparator.DataBindings.Add("Text", component, "MultiValueFieldSeparator", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtMultiValueFieldSeparator.DataBindings.Add("Enabled", component, "AllowMultiValueFields", false, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _lnkHelpFields_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			MessageBox.Show(this, _helpMessage, Resources.TitleHelpSpecialFields);
		}

		private void _btnCommand_Click(object sender, EventArgs e)
		{
			_dlgCommand.FileName = _txtCommand.Text;
			if (_dlgCommand.ShowDialog(this) == DialogResult.OK)
				_txtCommand.Text = _dlgCommand.FileName;
		}
	}
}