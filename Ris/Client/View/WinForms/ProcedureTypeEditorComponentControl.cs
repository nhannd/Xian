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

			_baseType.DataSource = _component.BaseTypeChoices;
			_baseType.DataBindings.Add("Value", _component, "BaseType", true, DataSourceUpdateMode.OnPropertyChanged);
			_baseType.Format += delegate(object sender, ListControlConvertEventArgs e) { e.Value = _component.FormatBaseTypeItem(e.ListItem); };

			var xmlEditor = (Control) _component.XmlEditorHost.ComponentView.GuiElement;
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
