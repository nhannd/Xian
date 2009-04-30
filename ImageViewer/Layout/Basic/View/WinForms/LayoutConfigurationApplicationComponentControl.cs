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

using System.Windows.Forms;

namespace ClearCanvas.ImageViewer.Layout.Basic.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="LayoutSettingsApplicationComponent"/>
    /// </summary>
    public partial class LayoutConfigurationApplicationComponentControl : UserControl
    {
        private LayoutConfigurationApplicationComponent _component;

        public LayoutConfigurationApplicationComponentControl(LayoutConfigurationApplicationComponent component)
        {
            InitializeComponent();

            _component = component;

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component.LayoutConfigurations;
			_comboBoxModality.DataSource = bindingSource;
			_comboBoxModality.DisplayMember = "Text";

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
