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

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="OrderNoteboxConfigurationComponent"/>
    /// </summary>
    public partial class OrderNoteboxConfigurationComponentControl : ApplicationComponentUserControl
    {
        private OrderNoteboxConfigurationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public OrderNoteboxConfigurationComponentControl(OrderNoteboxConfigurationComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

        	_foldersTableView.Table = _component.StaffGroupTable;
			_foldersTableView.DataBindings.Add("Selection", _component, "SelectedTableItem", true, DataSourceUpdateMode.OnPropertyChanged);
        	_foldersTableView.MenuModel = _foldersTableView.ToolbarModel = _component.ActionModel;
        	_groupLookup.LookupHandler = _component.StaffGroupLookupHandler;
			_groupLookup.DataBindings.Add("Value", _component, "StaffGroupToAdd", true, DataSourceUpdateMode.OnPropertyChanged);
        }

		private void _addGroupButton_Click(object sender, EventArgs e)
		{
			_component.AddStaffGroup();
		}

		private void _okButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
    }
}
