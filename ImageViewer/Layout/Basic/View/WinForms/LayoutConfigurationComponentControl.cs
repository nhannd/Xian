#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LayoutSettingsApplicationComponent"/>
    /// </summary>
    public partial class LayoutConfigurationComponentControl : UserControl
    {
        private LayoutConfigurationComponent _component;

        public LayoutConfigurationComponentControl(LayoutConfigurationComponent component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component.Layouts;
			_modality.DataSource = bindingSource;
			_modality.DisplayMember = "Text";

			//these values are just constants, so we won't databind them, it's unnecessary.
			_imageBoxRows.Minimum = 1;
			_imageBoxColumns.Minimum = 1;
			_tileRows.Minimum = 1;
			_tileColumns.Minimum = 1;

			_imageBoxRows.Maximum = _component.MaximumImageBoxRows;
			_imageBoxColumns.Maximum = _component.MaximumImageBoxColumns;
			_tileRows.Maximum = _component.MaximumTileRows;
			_tileColumns.Maximum = _component.MaximumTileColumns;

			_imageBoxRows.DataBindings.Add("Value", bindingSource, "ImageBoxRows", true, DataSourceUpdateMode.OnPropertyChanged);
			_imageBoxColumns.DataBindings.Add("Value", bindingSource, "ImageBoxColumns", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileRows.DataBindings.Add("Value", bindingSource, "TileRows", true, DataSourceUpdateMode.OnPropertyChanged);
			_tileColumns.DataBindings.Add("Value", bindingSource, "TileColumns", true, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
