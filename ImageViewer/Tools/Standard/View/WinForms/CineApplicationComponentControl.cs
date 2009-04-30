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

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Tools.Standard.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="CineApplicationComponent"/>
    /// </summary>
    public partial class CineApplicationComponentControl : ApplicationComponentUserControl
    {
        private readonly CineApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public CineApplicationComponentControl(CineApplicationComponent component)
            :base(component)
        {
			_component = component;
			
			InitializeComponent();

			BindingSource source = new BindingSource();
        	source.DataSource = _component;
        	_startForwardButton.DataBindings.Add("Enabled", source, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
			_startReverseButton.DataBindings.Add("Enabled", source, "Enabled", true, DataSourceUpdateMode.OnPropertyChanged);
        	_stopButton.DataBindings.Add("Enabled", source, "Running", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Minimum", source, "MinimumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Maximum", source, "MaximumScale", true, DataSourceUpdateMode.OnPropertyChanged);
			_cineSpeed.DataBindings.Add("Value", source, "CurrentScaleValue", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void StartReverseButtonClicked(object sender, EventArgs e)
		{
			_component.Reverse = true;
			_component.StartCine();
		}

		private void StopButtonClicked(object sender, EventArgs e)
		{
			_component.StopCine();
		}

		private void StartForwardButtonClicked(object sender, EventArgs e)
		{
			_component.Reverse = false;
			_component.StartCine();
		}
    }
}
