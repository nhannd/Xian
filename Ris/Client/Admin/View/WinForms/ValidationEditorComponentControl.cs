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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;
using System.Reflection;

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="ValidationEditorComponent"/>
    /// </summary>
    public partial class ValidationEditorComponentControl : ApplicationComponentUserControl
    {
        private ValidationEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public ValidationEditorComponentControl(ValidationEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _propertiesTableView.Table = _component.Rules;
            _propertiesTableView.ToolbarModel = _component.RulesActionModel;
            _propertiesTableView.MenuModel = _component.RulesActionModel;
            _propertiesTableView.DataBindings.Add("Selection", _component, "SelectedRule", true, DataSourceUpdateMode.OnPropertyChanged);
            _testButton.DataBindings.Add("Enabled", _component, "CanTestRules");

            foreach (PropertyInfo item in _component.ComponentPropertyChoices)
            {
                _propertiesMenu.Items.Add(item.Name);
            }

            Control editor = (Control)_component.EditorComponentHost.ComponentView.GuiElement;
            editor.Dock = DockStyle.Fill;
            _editorPanel.Controls.Add(editor);
        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            _component.Accept();
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }

        private void _testButton_Click(object sender, EventArgs e)
        {
            _component.TestRules();
        }

        private void _macroButton_Click(object sender, EventArgs e)
        {
            _propertiesMenu.Show(_macroButton, new Point(0, _macroButton.Height), ToolStripDropDownDirection.BelowRight);
        }

        private void _propertiesMenu_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            _component.InsertText(e.ClickedItem.Text);
        }
    }
}
