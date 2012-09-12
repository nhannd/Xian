#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
	/// <summary>
	/// Provides a Windows Forms user-interface for <see cref="DiagnosticServiceEditorComponent"/>.
	/// </summary>
	public partial class DiagnosticServiceEditorComponentControl : ApplicationComponentUserControl
	{
		private DiagnosticServiceEditorComponent _component;

		/// <summary>
		/// Constructor.
		/// </summary>
		public DiagnosticServiceEditorComponentControl(DiagnosticServiceEditorComponent component)
			:base(component)
		{
			_component = component;
			InitializeComponent();

			_idBox.DataBindings.Add("Value", _component, "ID", true, DataSourceUpdateMode.OnPropertyChanged);
			_nameBox.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_itemSelector.AvailableItemsTable = _component.AvailableProcedureTypes;
			_itemSelector.SelectedItemsTable = _component.SelectedProcedureTypes;

			_itemSelector.ItemAdded += OnItemsAddedOrRemoved;
			_itemSelector.ItemRemoved += OnItemsAddedOrRemoved;
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void OnItemsAddedOrRemoved(object sender, EventArgs e)
		{
			_component.ItemsAddedOrRemoved();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
	}
}