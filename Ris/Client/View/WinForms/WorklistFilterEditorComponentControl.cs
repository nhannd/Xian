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
    /// Provides a Windows Forms user-interface for <see cref="WorklistFilterEditorComponent"/>
    /// </summary>
    public partial class WorklistFilterEditorComponentControl : ApplicationComponentUserControl
    {
        private WorklistFilterEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistFilterEditorComponentControl(WorklistFilterEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

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
        	_patientClass.DataBindings.Add("Visible", _component, "PatientClassVisible");

            _portable.NullItem = _component.NullFilterItem;
            _portable.DataBindings.Add("Items", _component, "PortableChoices", true, DataSourceUpdateMode.Never);
            _portable.DataBindings.Add("CheckedItems", _component, "SelectedPortabilities", true,
                                         DataSourceUpdateMode.OnPropertyChanged);

        	_orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;
			_orderingPractitioner.DataBindings.Add("Value", _component, "SelectedOrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
