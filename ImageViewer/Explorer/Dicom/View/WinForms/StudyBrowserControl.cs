using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Common;
using ClearCanvas.Controls.WinForms;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Explorer.Dicom.View.WinForms
{
	public partial class StudyBrowserControl : UserControl
	{
		private StudyBrowserComponent _studyBrowserComponent;
		private BindingSource _bindingSource;

		public StudyBrowserControl(StudyBrowserComponent component)
		{
			Platform.CheckForNullReference(component, "component");
			InitializeComponent();

			_studyBrowserComponent = component;
			_studyBrowserComponent.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);

			_studyTableView.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			_studyTableView.DataSource = _studyBrowserComponent.StudyList;
			_studyTableView.ToolbarModel = _studyBrowserComponent.ToolbarModel;
			_studyTableView.MenuModel = _studyBrowserComponent.ContextMenuModel;
			_studyTableView.SelectionChanged += new EventHandler(OnStudyTableViewSelectionChanged);
			_studyTableView.ItemDoubleClicked += new EventHandler(OnStudyTableViewDoubleClick);
			_studySearchForm.SearchClicked += new EventHandler(OnSearchClicked);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _studyBrowserComponent;

			_studySearchForm.LastName.DataBindings.Add("Text", _bindingSource, "LastName", true, DataSourceUpdateMode.OnPropertyChanged);
			_studySearchForm.FirstName.DataBindings.Add("Text", _bindingSource, "FirstName", true, DataSourceUpdateMode.OnPropertyChanged);
			_studySearchForm.AccessionNumber.DataBindings.Add("Text", _bindingSource, "AccessionNumber", true, DataSourceUpdateMode.OnPropertyChanged);
			_studySearchForm.PatientId.DataBindings.Add("Text", _bindingSource, "PatientID", true, DataSourceUpdateMode.OnPropertyChanged);
			_studySearchForm.StudyDescription.DataBindings.Add("Text", _bindingSource, "StudyDescription", true, DataSourceUpdateMode.OnPropertyChanged);

			_searchTitleBar.DataBindings.Add("Text", _studyBrowserComponent, "SearchTitle", true, DataSourceUpdateMode.OnPropertyChanged);
			_resultsTitleBar.DataBindings.Add("Text", _studyBrowserComponent, "ResultsTitle", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void OnSelectedServerChanged(object sender, EventArgs e)
		{
			_studyTableView.DataSource = _studyBrowserComponent.StudyList;
		}

		void OnStudyTableViewSelectionChanged(object sender, EventArgs e)
		{
			_studyBrowserComponent.SetSelection(_studyTableView.CurrentSelection);
		}

		void OnStudyTableViewDoubleClick(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				_studyBrowserComponent.ItemDoubleClick();
			}
		}

		void OnSearchClicked(object sender, EventArgs e)
		{
			using (new CursorManager(this, Cursors.WaitCursor))
			{
				try
				{
					_studyBrowserComponent.Search();
				}
				catch
				{
					Platform.ShowMessageBox("Unable to query server");
				}
			}
		}
	}
}
