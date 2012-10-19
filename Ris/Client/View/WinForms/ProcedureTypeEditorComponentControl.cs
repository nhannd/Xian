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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ProcedureTypeEditorComponent"/>.
	/// </summary>
	public partial class ProcedureTypeEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ProcedureTypeEditorComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public ProcedureTypeEditorComponentControl(ProcedureTypeEditorComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_id.DataBindings.Add("Value", _component, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
			_defaultDuration.DataBindings.Add("Value", _component, "DefaultDuration", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_defaultModality.DataSource = _component.DefaultModalityChoices;
			_defaultModality.DataBindings.Add("Value", _component, "DefaultModality", true, DataSourceUpdateMode.OnPropertyChanged);
			_defaultModality.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatModalityItem(e.ListItem); };
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
