#region License

// Copyright (c) 2009, ClearCanvas Inc.
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
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CannedTextEditorComponent"/>
    /// </summary>
    public partial class CannedTextEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly CannedTextEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CannedTextEditorComponentControl(CannedTextEditorComponent component)
            :base(component)
        {
            InitializeComponent();
            _component = component;

			_groups.Enabled = !_component.IsReadOnly;
			_category.Enabled = !_component.IsReadOnly;
			_name.ReadOnly = _component.IsReadOnly;
			_text.ReadOnly = _component.IsReadOnly;

            _typeGroupBox.Enabled = !_component.IsReadOnly && _component.CanChangeType;

            _radioGroup.Checked = _component.IsEditingGroup;
            _radioPersonal.DataBindings.Add("Checked", _component, "IsEditingPersonal", true, DataSourceUpdateMode.OnPropertyChanged);

            _groups.DataSource = _component.StaffGroupChoices;
			_groups.Format += delegate(object sender, ListControlConvertEventArgs args) { args.Value = _component.FormatStaffGroup(args.ListItem); };
			_groups.DataBindings.Add("Visible", _component, "IsEditingGroup", true, DataSourceUpdateMode.OnPropertyChanged);
			_groups.DataBindings.Add("Value", _component, "StaffGroup", true, DataSourceUpdateMode.OnPropertyChanged);

			_category.DataSource = _component.CategoryChoices;
			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_category.DataBindings.Add("Text", _component, "Category", true, DataSourceUpdateMode.OnPropertyChanged);
			_text.DataBindings.Add("Value", _component, "Text", true, DataSourceUpdateMode.OnPropertyChanged);
			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

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
