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
    /// Provides a Windows Forms user-interface for <see cref="WorklistTimeWindowEditorComponent"/>
    /// </summary>
    public partial class WorklistTimeWindowEditorComponentControl : ApplicationComponentUserControl
    {
        private WorklistTimeWindowEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistTimeWindowEditorComponentControl(WorklistTimeWindowEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
            _component.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(_component_PropertyChanged);

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

    }
}
