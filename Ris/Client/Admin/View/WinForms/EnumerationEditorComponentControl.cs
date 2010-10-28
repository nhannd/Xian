#region License

// Copyright (c) 2010, ClearCanvas Inc.
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

namespace ClearCanvas.Ris.Client.Admin.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="EnumerationEditorComponent"/>
    /// </summary>
    public partial class EnumerationEditorComponentControl : ApplicationComponentUserControl
    {
        private EnumerationEditorComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public EnumerationEditorComponentControl(EnumerationEditorComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

            _code.DataBindings.Add("ReadOnly", _component, "IsCodeReadOnly", true, DataSourceUpdateMode.OnPropertyChanged);
            _code.DataBindings.Add("Value", _component, "Code", true, DataSourceUpdateMode.OnPropertyChanged);
            _displayValue.DataBindings.Add("Value", _component, "DisplayValue", true, DataSourceUpdateMode.OnPropertyChanged);
            _description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);

            _insertAfter.DataSource = _component.InsertAfterChoices;
            _insertAfter.DataBindings.Add("Value", _component, "InsertAfter", true, DataSourceUpdateMode.OnPropertyChanged);
            _insertAfter.Format += delegate(object sender, ListControlConvertEventArgs e)
                                       {
                                           e.Value = _component.FormatInsertAfterChoice(e.ListItem);
                                       };

            _okButton.DataBindings.Add("Enabled", _component, "Modified");

        }

        private void _okButton_Click(object sender, EventArgs e)
        {
            using (CursorManager cm = new CursorManager(Cursors.WaitCursor))
            {
                _component.Accept();
            }
        }

        private void _cancelButton_Click(object sender, EventArgs e)
        {
            _component.Cancel();
        }
    }
}
