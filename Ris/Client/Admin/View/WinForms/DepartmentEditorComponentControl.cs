#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DepartmentEditorComponent"/>.
    /// </summary>
    public partial class DepartmentEditorComponentControl : ApplicationComponentUserControl
    {
        private DepartmentEditorComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DepartmentEditorComponentControl(DepartmentEditorComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_id.DataBindings.Add("Value", _component, "Id", true, DataSourceUpdateMode.OnPropertyChanged);
			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
			_facility.DataSource = _component.FacilityChoices;
			_facility.DataBindings.Add("Value", _component, "Facility", true, DataSourceUpdateMode.OnPropertyChanged);
			_facility.Format += ((sender, e) => e.Value = _component.FormatFacility(e.ListItem));

        	_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled");

        }

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			using(new CursorManager(Cursors.WaitCursor))
			{
				_component.Accept();
			}
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
    }
}
