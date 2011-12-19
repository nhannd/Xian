#region License

// Copyright (c) 2011, ClearCanvas Inc.
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
using System.Windows.Forms.Design;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DisplaySetOptionsApplicationComponent"/>.
    /// </summary>
    public partial class DisplaySetCreationConfigurationComponentControl : ApplicationComponentUserControl
    {
        private readonly DisplaySetCreationConfigurationComponent _component;
        private readonly BindingSource _bindingSource;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DisplaySetCreationConfigurationComponentControl(DisplaySetCreationConfigurationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_bindingSource = new BindingSource {DataSource = _component.Options};

            _modality.DataSource = _bindingSource;
			_modality.DisplayMember = "Modality";

			_createSingleImageDisplaySets.DataBindings.Add("Checked", _bindingSource, "CreateSingleImageDisplaySets", false, DataSourceUpdateMode.OnPropertyChanged);
			_createSingleImageDisplaySets.DataBindings.Add("Enabled", _bindingSource, "CreateSingleImageDisplaySetsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

            _createAllImagesDisplaySet.DataBindings.Add("Checked", _bindingSource, "CreateAllImagesDisplaySet", false, DataSourceUpdateMode.OnPropertyChanged);
            _createAllImagesDisplaySet.DataBindings.Add("Enabled", _bindingSource, "CreateAllImagesDisplaySetEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

            _showOriginalSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalSeries", false, DataSourceUpdateMode.OnPropertyChanged);
            _showOriginalSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_splitEchos.DataBindings.Add("Checked", _bindingSource, "SplitMultiEchoSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitEchos.DataBindings.Add("Enabled", _bindingSource, "SplitMultiEchoSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
	
			_showOriginalMultiEchoSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMultiEchoSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMultiEchoSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMultiEchoSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_splitMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "SplitMixedMultiframes", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "SplitMixedMultiframesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_showOriginalMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMixedMultiframeSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMixedMultiframeSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_invertImages.DataBindings.Add("Checked", _bindingSource, "ShowGrayscaleInverted", false, DataSourceUpdateMode.OnPropertyChanged);
			_invertImages.DataBindings.Add("Enabled", _bindingSource, "ShowGrayscaleInvertedEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
