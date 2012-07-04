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

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ProtocolEditorComponent"/>
	/// </summary>
	public partial class ProtocolEditorComponentControl : ApplicationComponentUserControl
	{
		private readonly ProtocolEditorComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public ProtocolEditorComponentControl(ProtocolEditorComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_urgency.DataSource = _component.UrgencyChoices;
			_urgency.DataBindings.Add("Value", _component, "Urgency", true, DataSourceUpdateMode.OnPropertyChanged);
			_urgency.DataBindings.Add("Enabled", _component, "CanEdit", true, DataSourceUpdateMode.OnPropertyChanged);

			_author.DataBindings.Add("Value", _component, "Author", true, DataSourceUpdateMode.OnPropertyChanged);
			_author.DataBindings.Add("Visible", _component, "ShowAuthor", true, DataSourceUpdateMode.OnPropertyChanged);

			_protocolGroup.DataSource = _component.ProtocolGroupChoices;
			_protocolGroup.DataBindings.Add("Value", _component, "ProtocolGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			_protocolGroup.DataBindings.Add("Enabled", _component, "CanEdit", true, DataSourceUpdateMode.OnPropertyChanged);
			_btnSetDefault.DataBindings.Add("Enabled", _component, "SetDefaultProtocolGroupEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_component.PropertyChanged += _component_PropertyChanged;

			_protocolCodesSelector.ShowToolbars = false;
			_protocolCodesSelector.ShowColumnHeading = false;
			_protocolCodesSelector.AvailableItemsTable = _component.AvailableProtocolCodesTable;
			_protocolCodesSelector.SelectedItemsTable = _component.SelectedProtocolCodesTable;
			_protocolCodesSelector.DataBindings.Add("SelectedItemsTableSelection", _component, "SelectedProtocolCodesSelection", true, DataSourceUpdateMode.OnPropertyChanged);
			_protocolCodesSelector.DataBindings.Add("Enabled", _component, "CanEdit", true, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.LookupHandler = _component.SupervisorLookupHandler;
			_supervisor.DataBindings.Add("Value", _component, "Supervisor", true, DataSourceUpdateMode.OnPropertyChanged);
			_supervisor.DataBindings.Add("Enabled", _component, "CanEdit", true, DataSourceUpdateMode.OnPropertyChanged);
			_rememberSupervisorCheckbox.DataBindings.Add("Checked", _component, "RememberSupervisor", true, DataSourceUpdateMode.OnPropertyChanged);

			_supervisor.Visible = _component.SupervisorVisible;
			_rememberSupervisorCheckbox.Visible = _component.RememberSupervisorVisible;
		}

		void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "ProtocolGroupChoices")
			{
				// Re-setting the datasource overwrites objects bound to the control's "Value", which we don't want, so remove the binding first then re-bind
				_protocolGroup.DataBindings.Clear();
				_protocolGroup.DataSource = _component.ProtocolGroupChoices;
				_protocolGroup.DataBindings.Add("Value", _component, "ProtocolGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			}
		}

		private void _btnSetDefault_Click(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.SetDefaultProtocolGroup();
			}
		}
	}
}
