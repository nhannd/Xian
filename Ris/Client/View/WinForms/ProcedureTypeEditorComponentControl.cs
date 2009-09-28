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

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="ProcedureTypeEditorComponent"/>.
	/// </summary>
	public partial class ProcedureTypeEditorComponentControl : ApplicationComponentUserControl
	{
		private ProcedureTypeEditorComponent _component;

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
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_baseType.DataSource = _component.BaseTypeChoices;
			_baseType.DataBindings.Add("Value", _component, "BaseType", true, DataSourceUpdateMode.OnPropertyChanged);
			_baseType.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatBaseTypeItem(e.ListItem); };

			// The Text property of _xmlEditorPanel is only used for validation purpose only.
			_xmlEditorPanel.DataBindings.Add("Text", _component, "PlanXml", true, DataSourceUpdateMode.OnValidation);

			Control xmlEditor = (Control) _component.XmlEditorHost.ComponentView.GuiElement;
			xmlEditor.Dock = DockStyle.Fill;
			_xmlEditorPanel.Controls.Add(xmlEditor);
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
