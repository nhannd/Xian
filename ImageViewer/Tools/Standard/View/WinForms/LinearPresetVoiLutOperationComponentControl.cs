#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Standard.PresetVoiLuts.Operations;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	public partial class LinearPresetVoiLutOperationComponentControl : ClearCanvas.Desktop.View.WinForms.ApplicationComponentUserControl
	{
		private readonly LinearPresetVoiLutOperationComponent _component;

		public LinearPresetVoiLutOperationComponentControl(LinearPresetVoiLutOperationComponent component)
		{
			_component = component;
			InitializeComponent();

			BindingSource source = new BindingSource();
			source.DataSource = _component;

			_nameField.DataBindings.Add("Value", source, "PresetName", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowWidth.DataBindings.Add("Value", source, "WindowWidth", true, DataSourceUpdateMode.OnPropertyChanged);
			_windowCenter.DataBindings.Add("Value", source, "WindowCenter", true, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}
