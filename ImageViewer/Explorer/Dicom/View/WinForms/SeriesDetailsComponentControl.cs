#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.Explorer.Dicom.SeriesDetails;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
	/// Provides a Windows Forms user-interface for <see cref="SeriesDetailsComponent"/>.
    /// </summary>
    public partial class SeriesDetailsComponentControl : ApplicationComponentUserControl
    {
		private ISeriesDetailComponentViewModel _component;

        /// <summary>
        /// Constructor.
        /// </summary>
		public SeriesDetailsComponentControl(SeriesDetailsComponent component)
            :base(component)
        {
			_component = component;
            InitializeComponent();

        	_patientId.Value = _component.PatientId;
			_patientsName.Value = _component.PatientsName;
			_dob.Value = _component.PatientsBirthDate;
			_accessionNumber.Value = _component.AccessionNumber;
			_studyDate.Value = _component.StudyDate;
			_studyDescription.Value = _component.StudyDescription;

        	_seriesTable.Table = _component.SeriesTable;
        	_seriesTable.ToolbarModel = _component.ToolbarActionModel;
        	_seriesTable.MenuModel = _component.ContextMenuActionModel;

        	base.AcceptButton = _close;
			base.CancelButton = _close;
		}

		private void _close_Click(object sender, EventArgs e)
		{
			_component.Close();
		}

		private void _refresh_Click(object sender, EventArgs e)
		{
			_component.Refresh();
		}

		private void _seriesTable_SelectionChanged(object sender, EventArgs e)
		{
			_component.SetSeriesSelection(_seriesTable.Selection);
		}
    }
}
