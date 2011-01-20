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
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="SearchPanelComponent"/>
    /// </summary>
    public partial class SearchPanelComponentControl : ApplicationComponentUserControl
    {
        private readonly SearchPanelComponent _component;

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchPanelComponentControl(SearchPanelComponent component)
			: base(component)
        {
            InitializeComponent();

			this.AcceptButton = _searchButton;
            _component = component;

			ClearCanvasStyle.SetTitleBarStyle(_titleBar);

			_modalityPicker.SetAvailableModalities(_component.AvailableSearchModalities);

			_patientsName.DataBindings.Add("Value", component, "PatientsName", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Value", component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientID.DataBindings.Add("Value", component, "PatientID", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Value", component, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_titleBar.DataBindings.Add("Text", component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Maximum", component, "MaximumStudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Maximum", component, "MaximumStudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Value", component, "StudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Value", component, "StudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

        	this._referringPhysiciansName.DataBindings.Add("Value", component, "ReferringPhysiciansName", true,
        	                                               DataSourceUpdateMode.OnPropertyChanged);
			_modalityPicker.DataBindings.Add("CheckedModalities", component, "SearchModalities", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnSearchButtonClicked(object sender, EventArgs e)
		{
			try
			{
				_component.Search();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.MessageErrorQueryingOneOrMoreServers, _component.DesktopWindow);
			}
		}

		private void OnSearchLastWeekButtonClick(object sender, EventArgs e)
		{
			try
			{
				_component.SearchLastWeek();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.MessageErrorQueryingOneOrMoreServers, _component.DesktopWindow);
			}
		}

		private void OnSearchTodayButtonClicked(object sender, EventArgs e)
		{
			try
			{
				_component.SearchToday();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.MessageErrorQueryingOneOrMoreServers, _component.DesktopWindow);
			}
		}

		private void OnClearButonClicked(object sender, EventArgs e)
		{
			_component.Clear();
		}
	}
}
