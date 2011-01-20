#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="StaffGroupDetailsEditorComponent"/>.
    /// </summary>
    public partial class StaffGroupDetailsEditorComponentControl : ApplicationComponentUserControl
    {
        private StaffGroupDetailsEditorComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public StaffGroupDetailsEditorComponentControl(StaffGroupDetailsEditorComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_name.DataBindings.Add("Value", _component, "Name", true, DataSourceUpdateMode.OnPropertyChanged);
			_description.DataBindings.Add("Value", _component, "Description", true, DataSourceUpdateMode.OnPropertyChanged);
			_electiveCheckbox.DataBindings.Add("Checked", _component, "IsElective", true, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
