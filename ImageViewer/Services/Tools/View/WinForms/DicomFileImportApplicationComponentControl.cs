#region License

// Copyright (c) 2006-2008, ClearCanvas Inc.
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

namespace ClearCanvas.ImageViewer.Services.Tools.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomFileImportApplicationComponent"/>
    /// </summary>
	public partial class DicomFileImportApplicationComponentControl : ApplicationComponentUserControl
    {
        private DicomFileImportApplicationComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public DicomFileImportApplicationComponentControl(DicomFileImportApplicationComponent component)
        {
            InitializeComponent();

			_component = component;

			_importTable.Table = _component.ImportTable;

			_importTable.ToolbarModel = _component.ToolbarModel;
			_importTable.MenuModel = _component.ContextMenuModel;

			_importTable.SelectionChanged += delegate { _component.SetSelection(_importTable.Selection); };

			BindingSource bindingSource = new BindingSource();
			bindingSource.DataSource = _component;

        	_importProgressControl.ButtonText = SR.LabelCancel;
			_importProgressControl.DataBindings.Add("StatusMessage", bindingSource, "SelectedStatusMessage", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("TotalToProcess", bindingSource, "SelectedTotalToProcess", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("TotalProcessed", bindingSource, "SelectedTotalProcessed", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("AvailableCount", bindingSource, "SelectedAvailableCount", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("FailedSteps", bindingSource, "SelectedFailedSteps", true, DataSourceUpdateMode.OnPropertyChanged);
			_importProgressControl.DataBindings.Add("ButtonEnabled", bindingSource, "SelectedCancelEnabled", true, DataSourceUpdateMode.OnPropertyChanged);

			_importProgressControl.ButtonClicked += delegate(object sender, EventArgs args) { _component.CancelSelected(); };
		}
	}
}
