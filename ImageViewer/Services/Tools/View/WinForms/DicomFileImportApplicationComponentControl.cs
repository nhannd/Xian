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
