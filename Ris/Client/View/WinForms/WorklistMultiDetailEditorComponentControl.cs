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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="WorklistMultiDetailEditorComponent"/>
    /// </summary>
    public partial class WorklistMultiDetailEditorComponentControl : ApplicationComponentUserControl
    {
        private WorklistMultiDetailEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistMultiDetailEditorComponentControl(WorklistMultiDetailEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _worklistCategory.DataSource = _component.CategoryChoices;
            _worklistCategory.DataBindings.Add("Value", _component, "SelectedCategory", true, DataSourceUpdateMode.OnPropertyChanged);

            _defaultName.DataBindings.Add("Value", _component, "DefaultWorklistName", true, DataSourceUpdateMode.OnPropertyChanged);

            _worklistClassTableView.MenuModel = _component.WorklistActionModel;
            _worklistClassTableView.ToolbarModel = _component.WorklistActionModel;
            _worklistClassTableView.Table = _component.WorklistTable;
            _worklistClassTableView.DataBindings.Add("Selection", _component, "SelectedWorklist", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _worklistClassTableView_ItemDoubleClicked(object sender, EventArgs e)
        {
            _component.EditSelectedWorklist();
        }
    }
}
