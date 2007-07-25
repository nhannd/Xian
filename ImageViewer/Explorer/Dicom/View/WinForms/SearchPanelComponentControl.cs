using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="SearchPanelComponent"/>
    /// </summary>
    public partial class SearchPanelComponentControl : CustomUserControl
    {
        private SearchPanelComponent _component;
		private BindingSource _bindingSource;

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchPanelComponentControl(SearchPanelComponent component)
        {
            InitializeComponent();

			this.AcceptButton = _searchButton;
            _component = component;

			ClearCanvasStyle.SetTitleBarStyle(_titleBar);

			_modalityPicker.SetAvailableModalities(_component.AvailableSearchModalities);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_lastName.DataBindings.Add("Value", _bindingSource, "LastName", true, DataSourceUpdateMode.OnPropertyChanged);
			_firstName.DataBindings.Add("Value", _bindingSource, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged);
			_firstName.DataBindings.Add("Enabled", _bindingSource, "AllowFirstName", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Value", _bindingSource, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientID.DataBindings.Add("Value", _bindingSource, "PatientID", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Value", _bindingSource, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_titleBar.DataBindings.Add("Text", _bindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Value", _bindingSource, "StudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Value", _bindingSource, "StudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Maximum", _bindingSource, "MaximumStudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Maximum", _bindingSource, "MaximumStudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_modalityPicker.DataBindings.Add("CheckedModalities", _bindingSource, "SearchModalities", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		private void OnSearchButtonClicked(object sender, EventArgs e)
		{
			try
			{
				_component.Search();
			}
			catch (Exception ex)
			{
				ExceptionHandler.Report(ex, SR.MessageUnableToQueryServer, _component.DesktopWindow);
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
				ExceptionHandler.Report(ex, SR.MessageUnableToQueryServer, _component.DesktopWindow);
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
				ExceptionHandler.Report(ex, SR.MessageUnableToQueryServer, _component.DesktopWindow);
			}
		}

		private void OnClearButonClicked(object sender, EventArgs e)
		{
			_component.Clear();
		}

		private void SearchPanelComponentControl_Load(object sender, EventArgs e)
		{

		}
    }
}
