using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
    /// <summary>
    /// Provides a Windows Forms user-interface for <see cref="SearchPanelComponent"/>
    /// </summary>
    public partial class SearchPanelComponentControl : UserControl
    {
        private SearchPanelComponent _component;
		private BindingSource _bindingSource;

        /// <summary>
        /// Constructor
        /// </summary>
        public SearchPanelComponentControl(SearchPanelComponent component)
        {
            InitializeComponent();

            _component = component;

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _component;

			_lastName.DataBindings.Add("Text", _bindingSource, "LastName", true, DataSourceUpdateMode.OnPropertyChanged);
			_firstName.DataBindings.Add("Text", _bindingSource, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged);
			_accessionNumber.DataBindings.Add("Text", _bindingSource, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_patientID.DataBindings.Add("Text", _bindingSource, "PatientID", true, DataSourceUpdateMode.OnPropertyChanged);
			_studyDescription.DataBindings.Add("Text", _bindingSource, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);
			_titleBar.DataBindings.Add("Text", _bindingSource, "Title", true, DataSourceUpdateMode.OnPropertyChanged);

			_searchButton.Click += new EventHandler(OnSearchButtonClicked);
			_clearButton.Click += new EventHandler(OnClearButonClicked);
		}

		void OnClearButonClicked(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_component.Clear();
			}
		}

		void OnSearchButtonClicked(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				try
				{
					_component.Search();
				}
				catch
				{
					Platform.ShowMessageBox("Unable to query server");
				}
			}
		}
    }
}
