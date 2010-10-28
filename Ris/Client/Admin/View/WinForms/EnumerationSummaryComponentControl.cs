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
    /// Provides a Windows Forms user-interface for <see cref="EnumerationSummaryComponent"/>
    /// </summary>
    public partial class EnumerationSummaryComponentControl : ApplicationComponentUserControl
    {
        private EnumerationSummaryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public EnumerationSummaryComponentControl(EnumerationSummaryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _enumerationName.DataSource = _component.EnumerationChoices;
            _enumerationName.DataBindings.Add("Value", _component, "SelectedEnumeration", true, DataSourceUpdateMode.OnPropertyChanged);

            _enumerationClass.DataBindings.Add("Value", _component, "SelectedEnumerationClassName", true, DataSourceUpdateMode.OnPropertyChanged);


			_enumerationValuesTableView.Table = _component.SummaryTable;
			_enumerationValuesTableView.MenuModel = _component.SummaryTableActionModel;
			_enumerationValuesTableView.ToolbarModel = _component.SummaryTableActionModel;
			_enumerationValuesTableView.DataBindings.Add("Selection", _component, "SummarySelection", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _enumerationValuesTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.DoubleClickSelectedItem();
        }
    }
}
