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
using ClearCanvas.Desktop;

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

			ClearCanvasStyle.SetTitleBarStyle(_resultsTitleBar);

			_studyBrowserComponent = component;
			_studyBrowserComponent.SelectedServerChanged += new EventHandler(OnSelectedServerChanged);

			_studyTableView.ToolStripItemDisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
			_studyTableView.Table = _studyBrowserComponent.StudyList;
			_studyTableView.ToolbarModel = _studyBrowserComponent.ToolbarModel;
			_studyTableView.MenuModel = _studyBrowserComponent.ContextMenuModel;
			_studyTableView.SelectionChanged += new EventHandler(OnStudyTableViewSelectionChanged);
			_studyTableView.ItemDoubleClicked += new EventHandler(OnStudyTableViewDoubleClick);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _studyBrowserComponent;

			_resultsTitleBar.DataBindings.Add("Text", _studyBrowserComponent, "ResultsTitle", true, DataSourceUpdateMode.OnPropertyChanged);
		}

		void OnSelectedServerChanged(object sender, EventArgs e)
		{
            _studyTableView.Table = _studyBrowserComponent.StudyList;
		}

		void OnStudyTableViewSelectionChanged(object sender, EventArgs e)
		{
			_studyBrowserComponent.SetSelection(_studyTableView.Selection);
		}

		void OnStudyTableViewDoubleClick(object sender, EventArgs e)
		{
			_studyBrowserComponent.ItemDoubleClick();
		}
	}
}
