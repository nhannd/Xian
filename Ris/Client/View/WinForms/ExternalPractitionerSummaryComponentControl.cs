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
    /// Provides a Windows Forms user-interface for <see cref="StaffSummaryComponent"/>
    /// </summary>
    public partial class ExternalPractitionerSummaryComponentControl : ApplicationComponentUserControl
    {
        private ExternalPractitionerSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ExternalPractitionerSummaryComponentControl(ExternalPractitionerSummaryComponent component)
            : base(component)
        {
            InitializeComponent();
            _component = component;

            _practitionerTableView.ToolbarModel = _component.SummaryTableActionModel;
			_practitionerTableView.MenuModel = _component.SummaryTableActionModel;

            _practitionerTableView.Table = _component.SummaryTable;
			_practitionerTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);

            _firstName.DataBindings.Add("Value", _component, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged);
            _lastName.DataBindings.Add("Value", _component, "LastName", true, DataSourceUpdateMode.OnPropertyChanged);

            _okButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");
            _okButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");
            _cancelButton.DataBindings.Add("Visible", _component, "ShowAcceptCancelButtons");

			this.AcceptButton = _okButton;
        }

        private void _staffs_Load(object sender, EventArgs e)
        {
        }

        private void _staffs_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.DoubleClickSelectedItem();
        }

        private void _searchButton_Click(object sender, EventArgs e)
        {
            using (new CursorManager(Cursors.WaitCursor))
            {
                _component.Search();
            }
        }

		private void _clearButton_Click(object sender, EventArgs e)
		{
			_component.Clear();
		}

		private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();        
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

		private void _field_Enter(object sender, EventArgs e)
		{
			this.AcceptButton = _searchButton;
		}

		private void _field_Leave(object sender, EventArgs e)
		{
			this.AcceptButton = _okButton;
		}
    }
}