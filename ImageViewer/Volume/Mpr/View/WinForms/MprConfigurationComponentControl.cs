#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Common.Utilities;
using ClearCanvas.ImageViewer.Volume.Mpr.Configuration;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Volume.Mpr.View.WinForms
{
	public partial class MprConfigurationComponentControl : ApplicationComponentUserControl
	{
		private MprConfigurationComponent _component;

		public MprConfigurationComponentControl(MprConfigurationComponent component)
			: base(component)
		{
			InitializeComponent();

			_component = component;

			_txtProportionalSliceSpacing.DataBindings.Add("Text", _component, "SliceSpacingFactor", true, DataSourceUpdateMode.OnPropertyChanged);
			_txtProportionalSliceSpacing.DataBindings.Add("Enabled", _component, "ProportionalSliceSpacing", true, DataSourceUpdateMode.OnPropertyChanged);
			_radAutomaticSliceSpacing.DataBindings.Add("Checked", _component, "AutomaticSliceSpacing", false, DataSourceUpdateMode.OnPropertyChanged);
			_radProportionalSliceSpacing.DataBindings.Add("Checked", _component, "ProportionalSliceSpacing", false, DataSourceUpdateMode.OnPropertyChanged);

			base.ErrorProvider.SetIconAlignment(_txtProportionalSliceSpacing, ErrorIconAlignment.MiddleLeft);
		}
	}
}