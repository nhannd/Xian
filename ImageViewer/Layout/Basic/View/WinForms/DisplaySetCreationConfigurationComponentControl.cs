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
using System.Windows.Forms.Design;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DisplaySetOptionsApplicationComponent"/>.
    /// </summary>
    public partial class DisplaySetCreationConfigurationComponentControl : ApplicationComponentUserControl
    {
        private DisplaySetCreationConfigurationComponent _component;
		BindingSource _bindingSource;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DisplaySetCreationConfigurationComponentControl(DisplaySetCreationConfigurationComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component.Options;

			_modality.DataSource = _bindingSource;
			_modality.DisplayMember = "Modality";

			_createSingleImageDisplaySets.DataBindings.Add("Checked", _bindingSource, "CreateSingleImageDisplaySets", false, DataSourceUpdateMode.OnPropertyChanged);
			_createSingleImageDisplaySets.DataBindings.Add("Enabled", _bindingSource, "CreateSingleImageDisplaySetsEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_splitEchos.DataBindings.Add("Checked", _bindingSource, "SplitMultiEchoSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitEchos.DataBindings.Add("Enabled", _bindingSource, "SplitMultiEchoSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
	
			_showOriginalMultiEchoSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMultiEchoSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMultiEchoSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMultiEchoSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_splitMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "SplitMixedMultiframes", false, DataSourceUpdateMode.OnPropertyChanged);
			_splitMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "SplitMixedMultiframesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

			_showOriginalMixedMultiframeSeries.DataBindings.Add("Checked", _bindingSource, "ShowOriginalMixedMultiframeSeries", false, DataSourceUpdateMode.OnPropertyChanged);
			_showOriginalMixedMultiframeSeries.DataBindings.Add("Enabled", _bindingSource, "ShowOriginalMixedMultiframeSeriesEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
		}
    }
}
