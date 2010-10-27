#region License

// Copyright (c) 2010, ClearCanvas Inc.
// All rights reserved.
// http://www.clearcanvas.ca
//
// This software is licensed under the Open Software License v3.0.
// For the complete license, see http://www.clearcanvas.ca/OSLv3.0

#endregion

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	public partial class StudyFilterComponentPanel : UserControl
	{
		private readonly StudyFilterComponent _component;

		private StudyFilterComponentPanel()
		{
			InitializeComponent();
		}

		public StudyFilterComponentPanel(StudyFilterComponent component) : this()
		{
			_component = component;
			_component.FilterPredicatesChanged += new EventHandler(_component_FilterPredicatesChanged);

			ActionModelRoot toolbarActions = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms", StudyFilterTool.DefaultToolbarActionSite, _component.ExportedActions);
			ToolStripBuilder.ToolStripBuilderStyle defaultStyle = new ToolStripBuilder.ToolStripBuilderStyle();
			ToolStripBuilder.ToolStripBuilderStyle myStyle = new ToolStripBuilder.ToolStripBuilderStyle(ToolStripItemDisplayStyle.ImageAndText, defaultStyle.ToolStripAlignment, defaultStyle.TextImageRelation);
			ToolStripBuilder.BuildToolbar(_toolbar.Items, toolbarActions.ChildNodes, myStyle);

			_tableView.Table = component.Table;
			_tableView.MultiSelect = true;
			_tableView.ColumnFilterMenuStripClosed += new EventHandler(_tableView_ColumnFilterMenuStripClosed);
			_tableView.ContextActionModelDelegate = this.GetContextMenuModel;
			_tableView.ColumnFilterActionModelDelegate = this.GetColumnFilterMenuModel;
			_tableView.ReadOnly = true;
		}

		private ActionModelNode GetContextMenuModel(int row, int column)
		{
			IStudyItem activeItem = null;
			if (row >= 0 && row < _component.Items.Count)
				activeItem = _component.Items[row];

			StudyFilterColumn activeColumn = null;
			if (column >= 0 && column < _component.Columns.Count)
				activeColumn = _component.Columns[column];

			return _component.GetContextMenuActionModel(activeItem, activeColumn);
		}

		private ActionModelNode GetColumnFilterMenuModel(int row, int column)
		{
			return _component.Columns[column].FilterMenuModel;
		}

		private void _component_FilterPredicatesChanged(object sender, EventArgs e)
		{
			foreach (StudyFilterColumn column in (IEnumerable<StudyFilterColumn>) _component.Columns)
			{
				_tableView.SetColumnFilteringActive(_component.Columns.IndexOf(column), column.IsColumnFiltered);
			}
		}

		private void _tableView_ColumnFilterMenuStripClosed(object sender, EventArgs e)
		{
			// if the filter menu closes, refresh the component if it is now dirty
			_component.Refresh(false);
		}

		private void _tableView_SelectionChanged(object sender, EventArgs e)
		{
			_component.Selection.SuspendEvents();
			try
			{
				_component.Selection.Clear();
				object[] selection = _tableView.Selection.Items;
				if (selection != null)
				{
					foreach (object o in selection)
					{
						_component.Selection.Add(o as IStudyItem);
					}
				}
			}
			finally
			{
				_component.Selection.ResumeEvents(true);
			}
		}
	}
}