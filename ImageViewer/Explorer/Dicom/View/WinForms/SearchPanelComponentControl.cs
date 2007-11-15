#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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
using System.Windows.Forms;
using ClearCanvas.Desktop;
using ClearCanvas.Desktop.View.WinForms;

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

			_studyDateFrom.DataBindings.Add("Maximum", _bindingSource, "MaximumStudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Maximum", _bindingSource, "MaximumStudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);

			_studyDateFrom.DataBindings.Add("Value", _bindingSource, "StudyDateFrom", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDateTo.DataBindings.Add("Value", _bindingSource, "StudyDateTo", true, DataSourceUpdateMode.OnPropertyChanged);
			
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
