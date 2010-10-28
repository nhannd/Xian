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

namespace ClearCanvas.Ris.Client.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DateTimeEntryComponent"/>
    /// </summary>
    public partial class DateTimeEntryComponentControl : ApplicationComponentUserControl
    {
        private DateTimeEntryComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DateTimeEntryComponentControl(DateTimeEntryComponent component)
            :base(component)
        {
            InitializeComponent();

            _component = component;

			_date.DataBindings.Add("Nullable", _component, "AllowNull");
			_date.DataBindings.Add("Value", _component, "DateAndTime", true, DataSourceUpdateMode.OnPropertyChanged);

			_time.DataBindings.Add("Nullable", _component, "AllowNull");
			_time.DataBindings.Add("Value", _component, "DateAndTime", true, DataSourceUpdateMode.OnPropertyChanged);
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
