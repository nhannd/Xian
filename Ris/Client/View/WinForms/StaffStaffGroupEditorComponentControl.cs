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
    /// Provides a Windows Forms user-interface for <see cref="StaffStaffGroupEditorComponent"/>
    /// </summary>
    public partial class StaffStaffGroupEditorComponentControl : ApplicationComponentUserControl
    {
        private StaffStaffGroupEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public StaffStaffGroupEditorComponentControl(StaffStaffGroupEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _staffGroupSelector.AvailableItemsTable = _component.AvailableGroupsTable;
            _staffGroupSelector.SelectedItemsTable = _component.SelectedGroupsTable;
            _staffGroupSelector.ItemAdded += OnItemsAddedOrRemoved;
            _staffGroupSelector.ItemRemoved += OnItemsAddedOrRemoved;
        	_staffGroupSelector.ReadOnly = _component.ReadOnly;
        }

        private void OnItemsAddedOrRemoved(object sender, EventArgs args)
        {
            _component.ItemsAddedOrRemoved();
        }
    }
}
