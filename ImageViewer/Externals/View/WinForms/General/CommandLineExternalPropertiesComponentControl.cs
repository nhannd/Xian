#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

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
			component.AutoQuoteArguments = false;

			base.ErrorProvider.SetIconPadding(_txtCommand, _btnCommand.Width);

			_helpMessage = component.ArgumentFieldsHelpText;
			_lnkHelpFields.Visible = !string.IsNullOrEmpty(_helpMessage);

			_txtName.DataBindings.Add("Text", component, "Label", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtCommand.DataBindings.Add("Text", component, "Command", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtWorkingDir.DataBindings.Add("Text", component, "WorkingDirectory", false, DataSourceUpdateMode.OnPropertyChanged);
			_txtArguments.DataBindings.Add("Text", component, "ArgumentString", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkAllowMultiValueFields.DataBindings.Add("Checked", component, "AllowMultiValueFields", false, DataSourceUpdateMode.OnPropertyChanged);
			_chkAutoQuoteArguments.DataBindings.Add("Checked", component, "AutoQuoteArguments", false, DataSourceUpdateMode.OnPropertyChanged);
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