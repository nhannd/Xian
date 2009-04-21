#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

            _portable.NullItem = _component.NullFilterItem;
            _portable.DataBindings.Add("Items", _component, "PortableChoices", true, DataSourceUpdateMode.Never);
            _portable.DataBindings.Add("CheckedItems", _component, "SelectedPortabilities", true,
                                         DataSourceUpdateMode.OnPropertyChanged);

        	_orderingPractitioner.LookupHandler = _component.OrderingPractitionerLookupHandler;
			_orderingPractitioner.DataBindings.Add("Value", _component, "SelectedOrderingPractitioner", true, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
