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
	public partial class PatientProfileSummaryComponentControl : ApplicationComponentUserControl
	{
		private PatientProfileSummaryComponent _component;

		/// <summary>
		/// Constructor
		/// </summary>
		public PatientProfileSummaryComponentControl(PatientProfileSummaryComponent component)
			: base(component)
		{
			InitializeComponent();
			_component = component;

			_patientProfileTableView.ToolbarModel = _component.SummaryTableActionModel;
			_patientProfileTableView.MenuModel = _component.SummaryTableActionModel;

			_patientProfileTableView.Table = _component.SummaryTable;
			_patientProfileTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);


			_searchText.DataBindings.Add("Value", _component, "SearchString", true, DataSourceUpdateMode.OnPropertyChanged);

			_okButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
			_okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");
			_cancelButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			using (new CursorManager(Cursors.WaitCursor))
			{
				_component.Search();
			}
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}

		private void _locationTableView_ItemDoubleClicked(object sender, EventArgs e)
		{
			_component.DoubleClickSelectedItem();
		}

		private void _clearButton_Click(object sender, EventArgs e)
		{
			_searchText.Value = "";
			_component.Search();
		}

		private void _searchText_Enter(object sender, EventArgs e)
		{
			this.AcceptButton = _searchButton;
		}

		private void _searchText_Leave(object sender, EventArgs e)
		{
			this.AcceptButton = _okButton;
		}
	}
}
