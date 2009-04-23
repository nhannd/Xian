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
    /// Provides a Windows Forms user-interface for <see cref="WorklistDetailEditorComponent"/>
    /// </summary>
    public partial class WorklistDetailEditorComponentControl : ApplicationComponentUserControl
    {
        private WorklistDetailEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public WorklistDetailEditorComponentControl(WorklistDetailEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;
			_component.PropertyChanged += _component_PropertyChanged;

        	_ownerGroupBox.Visible = _component.IsOwnerPanelVisible;
			_radioGroup.Enabled = _component.IsPersonalGroupRadioButtonEnabled;
			_radioGroup.Checked = _component.IsGroup;
			_radioPersonal.Enabled = _component.IsPersonalGroupRadioButtonEnabled;
			_radioPersonal.DataBindings.Add("Checked", _component, "IsPersonal", true, DataSourceUpdateMode.OnPropertyChanged);

			_groups.DataSource = _component.GroupChoices;
			_groups.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatGroup(args.ListItem); };
			_groups.DataBindings.Add("Enabled", _component, "IsGroupChoicesEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_groups.DataBindings.Add("Value", _component, "SelectedGroup", true, DataSourceUpdateMode.OnPropertyChanged);

            _name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_category.DataBindings.Add("Value", _component, "SelectedCategory", true, DataSourceUpdateMode.OnPropertyChanged);
        	_category.Enabled = !_component.IsWorklistClassReadOnly;

			_worklistClass.DataSource = _component.WorklistClassChoices;
        	_worklistClass.Enabled = !_component.IsWorklistClassReadOnly;
			_worklistClass.Format += delegate(object sender, ListControlConvertEventArgs args)
			                         {
			                         	 args.Value = _component.FormatWorklistClass(args.ListItem);
			                         };

            _worklistClass.DataBindings.Add("Value", _component, "WorklistClass", true, DataSourceUpdateMode.OnPropertyChanged);

            _classDescription.DataBindings.Add("Value", _component, "WorklistClassDescription", true, DataSourceUpdateMode.OnPropertyChanged);
            _okButton.DataBindings.Add("Visible", _component, "AcceptButtonVisible", true, DataSourceUpdateMode.Never);
            _cancelButton.DataBindings.Add("Visible", _component, "CancelButtonVisible", true, DataSourceUpdateMode.Never);

        }

		private void _component_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if(e.PropertyName == "WorklistClassChoices")
			{
				_worklistClass.DataSource = _component.WorklistClassChoices;
			}
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
