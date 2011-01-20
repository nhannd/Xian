#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Windows.Forms;

using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.Ris.Client.Workflow.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="DicomSeriesEditorComponent"/>.
    /// </summary>
    public partial class DicomSeriesEditorComponentControl : ApplicationComponentUserControl
    {
        private readonly DicomSeriesEditorComponent _component;

        /// <summary>
        /// Constructor.
        /// </summary>
        public DicomSeriesEditorComponentControl(DicomSeriesEditorComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

			_studyInstanceUID.DataBindings.Add("Value", _component, "StudyInstanceUID", true, DataSourceUpdateMode.OnPropertyChanged);
			_seriesInstanceUID.DataBindings.Add("Value", _component, "SeriesInstanceUID", true, DataSourceUpdateMode.OnPropertyChanged);
			_seriesNumber.DataBindings.Add("Value", _component, "SeriesNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_seriesNumber.DataBindings.Add("ReadOnly", _component, "IsSeriesNumberReadOnly");
			_seriesDescription.DataBindings.Add("Value", _component, "SeriesDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_numberOfImages.DataBindings.Add("Value", _component, "NumberOfSeriesRelatedInstances", true, DataSourceUpdateMode.OnPropertyChanged);

			_acceptButton.DataBindings.Add("Enabled", _component, "AcceptEnabled", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void _acceptButton_Click(object sender, EventArgs e)
		{
			_component.Accept();
		}

		private void _cancelButton_Click(object sender, EventArgs e)
		{
			_component.Cancel();
		}
    }
}
