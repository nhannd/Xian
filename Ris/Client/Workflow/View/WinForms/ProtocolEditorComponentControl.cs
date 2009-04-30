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

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
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
