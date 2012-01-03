#region License

// Copyright (c) 2011, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.ComponentModel;
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;
using ProgressBarStyle=System.Windows.Forms.ProgressBarStyle;

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

			_component = component;

			_titleBar.DataBindings.Add("Text", component, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

			_modalityPicker.SetAvailableModalities(_component.AvailableSearchModalities);
			_modalityPicker.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_modalityPicker.DataBindings.Add("CheckedModalities", component, "SearchModalities", true, DataSourceUpdateMode.OnPropertyChanged);

			_patientsName.DataBindings.Add("Value", component, "PatientsName", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientsName.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_accessionNumber.DataBindings.Add("Value", component, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_patientID.DataBindings.Add("Value", component, "PatientID", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientID.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_studyDescription.DataBindings.Add("Value", component, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_studyDateFrom.DataBindings.Add("Maximum", component, "MaximumStudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Maximum", component, "MaximumStudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Value", component, "StudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Value", component, "StudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_studyDateTo.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_referringPhysiciansName.DataBindings.Add("Value", component, "ReferringPhysiciansName", true, DataSourceUpdateMode.OnPropertyChanged);
			_referringPhysiciansName.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_searchTodayButton.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_searchLastWeekButton.DataBindings.Add("Enabled", component, "IsSearchEnabled");
			_clearButton.DataBindings.Add("Enabled", component, "IsSearchEnabled");

			_component.PropertyChanged += OnComponentPropertyChanged;

			UpdateState();
			UpdateIcons();
		}

		protected override void OnCurrentUIThemeChanged()
		{
			base.OnCurrentUIThemeChanged();

			UpdateIcons();
		}

		private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "IsSearchEnabled")
			{
				UpdateState();
			}
		}

		private void UpdateState()
		{
			_searchButton.Text = _component.IsSearchEnabled ? SR.ButtonSearch : SR.ButtonCancelSearch;
			_progressBar.Style = _component.IsSearchEnabled ? ProgressBarStyle.Blocks : ProgressBarStyle.Marquee;
			_progressBar.Visible = _searchingLabel.Visible = !_component.IsSearchEnabled;
		}

		private void UpdateIcons()
		{
			var resourceResolver = new ApplicationThemeResourceResolver(GetType(), false);
			_searchButton.Image = resourceResolver.OpenImage(@"Icons.Search.png");
			_searchTodayButton.Image = resourceResolver.OpenImage(@"Icons.Today.png");
			_searchLastWeekButton.Image = resourceResolver.OpenImage(@"Icons.Last7Days.png");
			_clearButton.Image = resourceResolver.OpenImage(@"Icons.Clear.png");
		}

		private void OnSearchButtonClicked(object sender, EventArgs e)
		{
			if (_component.IsSearchEnabled)
				_component.Search();
			else
				_component.CancelSearch();
		}

		private void OnSearchLastWeekButtonClick(object sender, EventArgs e)
		{
			_component.SearchLastWeek();
		}

		private void OnSearchTodayButtonClicked(object sender, EventArgs e)
		{
			_component.SearchToday();
		}

		private void OnClearButonClicked(object sender, EventArgs e)
		{
			_component.Clear();
		}
	}
}