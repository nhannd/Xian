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

namespace ClearCanvas.Ris.Client.Workflow.Extended.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ProtocolGroupEditorComponent"/>
    /// </summary>
    public partial class ProtocolGroupEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly ProtocolGroupEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ProtocolGroupEditorComponentControl(ProtocolGroupEditorComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

            _codesSelector.AvailableItemsTable = _component.AvailableProtocolCodes;
            _codesSelector.SelectedItemsTable = _component.SelectedProtocolCodes;
            _codesSelector.DataBindings.Add("SelectedItemsTableSelection", _component, "SelectedProtocolCodesSelection", true, DataSourceUpdateMode.OnPropertyChanged);
            _codesSelector.AppendToSelectedItemsActionModel(_component.SelectedProtocolCodesActionModel);
            
            _codesSelector.ItemAdded += OnItemsAddedOrRemoved;
            _codesSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _readingGroupsSelector.AvailableItemsTable = _component.AvailableReadingGroups;
            _readingGroupsSelector.SelectedItemsTable = _component.SelectedReadingGroups;
            _readingGroupsSelector.ItemAdded += OnItemsAddedOrRemoved;
            _readingGroupsSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void OnItemsAddedOrRemoved(object sender, EventArgs args)
        {
            _component.ItemsAddedOrRemoved();
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
