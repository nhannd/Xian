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
using ClearCanvas.Common;
using ClearCanvas.Desktop.View.WinForms;
using ClearCanvas.Desktop;
using System.Collections;

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
			_studyBrowserComponent.StudyTableChanged += OnStudyBrowserComponentOnStudyTableChanged;

			_studyTableView.Table = _studyBrowserComponent.StudyTable;
			_studyTableView.ToolbarModel = _studyBrowserComponent.ToolbarModel;
			_studyTableView.MenuModel = _studyBrowserComponent.ContextMenuModel;
			_studyTableView.SelectionChanged += new EventHandler(OnStudyTableViewSelectionChanged);
			_studyTableView.ItemDoubleClicked += new EventHandler(OnStudyTableViewDoubleClick);

			_bindingSource = new BindingSource();
			_bindingSource.DataSource = _studyBrowserComponent;

			_resultsTitleBar.DataBindings.Add("Text", _studyBrowserComponent, "ResultsTitle", true, DataSourceUpdateMode.OnPropertyChanged);

			this.DataBindings.Add("Enabled", _studyBrowserComponent, "IsEnabled");
		}

		private void OnStudyBrowserComponentOnStudyTableChanged(object sender, EventArgs e)
		{
			_studyTableView.Table = _studyBrowserComponent.StudyTable;
		}

		void OnStudyTableViewSelectionChanged(object sender, EventArgs e)
		{
			//The table view remembers the selection order, with the most recent being first.
			//We actually want that same order, but in reverse.
			_studyBrowserComponent.SetSelection(ReverseSelection(_studyTableView.Selection));
		}

		void OnStudyTableViewDoubleClick(object sender, EventArgs e)
		{
			_studyBrowserComponent.ItemDoubleClick();
		}

		private static ISelection ReverseSelection(ISelection selection)
		{
			ArrayList list = new ArrayList();

			if (selection != null && selection.Items != null)
			{
				foreach (object o in selection.Items)
					list.Add(o);

				list.Reverse();
			}

			return new Selection(list);
		}
	}
}
