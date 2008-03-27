#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

using System;
using System.Collections;
using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="WorklistEditorComponent"/>
    /// </summary>
    public partial class WorklistEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly WorklistEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistEditorComponentControl(WorklistEditorComponent component)
            : base(component)
        {
            InitializeComponent();

            _component = component;
            _component.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_component_PropertyChanged);

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

            _type.DataSource = _component.TypeChoices;
            _type.DataBindings.Add("Value", _component, "Type", true, DataSourceUpdateMode.OnPropertyChanged);
            _type.DataBindings.Add("Enabled", _component, "TypeEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

            _procedureTypeGroupsSelector.AvailableItemsTable = _component.AvailableProcedureTypeGroups;
            _procedureTypeGroupsSelector.SelectedItemsTable = _component.SelectedProcedureTypeGroups;
            _procedureTypeGroupsSelector.ItemAdded += OnItemsAddedOrRemoved;
            _procedureTypeGroupsSelector.ItemRemoved += OnItemsAddedOrRemoved;
            _component.AvailableItemsChanged += _procedureTypeGroupsSelector.OnAvailableItemsChanged;

            _usersSelector.AvailableItemsTable = _component.AvailableUsers;
            _usersSelector.SelectedItemsTable = _component.SelectedUsers;
            _usersSelector.ItemAdded += OnItemsAddedOrRemoved;
            _usersSelector.ItemRemoved += OnItemsAddedOrRemoved;

            _facilities.NullItem = _component.NullFilterItem;
            _facilities.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatFacility(args.ListItem); };
            _facilities.DataBindings.Add("Items", _component, "FacilityChoices", true, DataSourceUpdateMode.Never);
            _facilities.DataBindings.Add("CheckedItems", _component, "SelectedFacilities", true,
                                         DataSourceUpdateMode.OnPropertyChanged);

            _priority.NullItem = _component.NullFilterItem;
            _priority.DataBindings.Add("Items", _component, "PriorityChoices", true, DataSourceUpdateMode.Never);
            _priority.DataBindings.Add("CheckedItems", _component, "SelectedPriorities", true,
                                         DataSourceUpdateMode.OnPropertyChanged);

            _patientClass.NullItem = _component.NullFilterItem;
            _patientClass.DataBindings.Add("Items", _component, "PatientClassChoices", true, DataSourceUpdateMode.Never);
            _patientClass.DataBindings.Add("CheckedItems", _component, "SelectedPatientClasses", true,
                                         DataSourceUpdateMode.OnPropertyChanged);

            _portable.NullItem = _component.NullFilterItem;
            _portable.DataBindings.Add("Items", _component, "PortableChoices", true, DataSourceUpdateMode.Never);
            _portable.DataBindings.Add("CheckedItems", _component, "SelectedPortabilities", true,
                                         DataSourceUpdateMode.OnPropertyChanged);


            _fixedWindowRadioButton.DataBindings.Add("Checked", _component, "IsFixedTimeWindow", true, DataSourceUpdateMode.OnPropertyChanged);
            _fixedWindowRadioButton.DataBindings.Add("Enabled", _component, "FixedSlidingChoiceEnabled", true, DataSourceUpdateMode.Never);
            _slidingWindowRadioButton.DataBindings.Add("Checked", _component, "IsSlidingTimeWindow", true, DataSourceUpdateMode.Never);
            _slidingWindowRadioButton.DataBindings.Add("Enabled", _component, "FixedSlidingChoiceEnabled", true, DataSourceUpdateMode.Never);

            _slidingScale.DataSource = _component.SlidingScaleChoices;
            _slidingScale.DataBindings.Add("Value", _component, "SlidingScale", true, DataSourceUpdateMode.OnPropertyChanged);
            _slidingScale.DataBindings.Add("Enabled", _component, "SlidingScaleEnabled", true, DataSourceUpdateMode.Never);
            
            
            _fromCheckBox.DataBindings.Add("Checked", _component, "StartTimeChecked", true, DataSourceUpdateMode.OnPropertyChanged);
            _fromFixed.DataBindings.Add("Enabled", _component, "FixedStartTimeEnabled", true, DataSourceUpdateMode.Never);
            _fromFixed.DataBindings.Add("Value", _component, "FixedStartTime", true, DataSourceUpdateMode.OnPropertyChanged);

            _fromSliding.DataSource = _component.SlidingStartTimeChoices;
            _fromSliding.DataBindings.Add("Enabled", _component, "SlidingStartTimeEnabled", true, DataSourceUpdateMode.Never);
            _fromSliding.DataBindings.Add("Value", _component, "SlidingStartTime", true, DataSourceUpdateMode.OnPropertyChanged);

            _toCheckBox.DataBindings.Add("Checked", _component, "EndTimeChecked", true, DataSourceUpdateMode.OnPropertyChanged);
            _toFixed.DataBindings.Add("Enabled", _component, "FixedEndTimeEnabled", true, DataSourceUpdateMode.Never);
            _toFixed.DataBindings.Add("Value", _component, "FixedEndTime", true, DataSourceUpdateMode.OnPropertyChanged);

            _toSliding.DataSource = _component.SlidingEndTimeChoices;
            _toSliding.DataBindings.Add("Enabled", _component, "SlidingEndTimeEnabled", true, DataSourceUpdateMode.Never);
            _toSliding.DataBindings.Add("Value", _component, "SlidingEndTime", true, DataSourceUpdateMode.OnPropertyChanged);

            _acceptButton.DataBindings.Add("Enabled", _component, "Modified", true, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void _component_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            // for some reason, if we try to data-bind to these properties we get weird exceptions
            // therefore, workaround is to handle the PropertyChanged event manually

            if (e.PropertyName == "SlidingStartTimeChoices")
            {
                _fromSliding.DataSource = _component.SlidingStartTimeChoices;
            }

            if (e.PropertyName == "SlidingEndTimeChoices")
            {
                _toSliding.DataSource = _component.SlidingEndTimeChoices;
            }
        }

        private void OnItemsAddedOrRemoved(object sender, EventArgs args)
        {
            _component.ItemsAddedOrRemoved();
        }

        private void _acceptButton_Click(object sender, System.EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, System.EventArgs e)
        {
            _component.Cancel();
        }
    }
}
