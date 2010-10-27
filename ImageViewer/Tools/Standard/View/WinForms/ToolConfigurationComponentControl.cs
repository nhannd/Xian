#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;
using ClearCanvas.ImageViewer.Tools.Standard.Configuration;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
	public partial class ToolConfigurationComponentControl : UserControl
	{
		private readonly ToolConfigurationComponent _component;
		private readonly BindingSource _bindingSource;

		public ToolConfigurationComponentControl(ToolConfigurationComponent component)
		{
			_component = component;
			InitializeComponent();

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component.Options;

			_modality.DataSource = _bindingSource;
			_modality.DisplayMember = "Modality";

			_autoCineMultiframes.DataBindings.Add("Checked", _bindingSource, "AutoCineMultiframes", false, DataSourceUpdateMode.OnPropertyChanged);
		}
	}
}