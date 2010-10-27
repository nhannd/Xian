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

namespace ClearCanvas.ImageViewer.TestTools.View.WinForms
{
    public partial class ChangePixelAspectRatioComponentControl : ApplicationComponentUserControl
    {
        private ChangePixelAspectRatioComponent _component;

        public ChangePixelAspectRatioComponentControl(ChangePixelAspectRatioComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_convertDisplaySet.DataBindings.Add("Checked", _component, "ConvertWholeDisplaySet", false, DataSourceUpdateMode.OnPropertyChanged);
			_increasePixelDimensions.DataBindings.Add("Checked", _component, "IncreasePixelDimensions", false, DataSourceUpdateMode.OnPropertyChanged);
			_removeCalibration.DataBindings.Add("Checked", _component, "RemoveCalibration", false, DataSourceUpdateMode.OnPropertyChanged);
			_row.DataBindings.Add("Value", _component, "AspectRatioRow", false, DataSourceUpdateMode.OnPropertyChanged);
			_column.DataBindings.Add("Value", _component, "AspectRatioColumn", false, DataSourceUpdateMode.OnPropertyChanged);

			_ok.Click += delegate { _component.Accept(); };
			_cancel.Click += delegate { _component.Cancel(); };
		}
    }
}
