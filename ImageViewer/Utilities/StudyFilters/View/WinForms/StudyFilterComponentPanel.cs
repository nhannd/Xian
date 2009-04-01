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

			ActionModelRoot toolbarActions = ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms", StudyFilterTool.DefaultToolbarActionSite, _component.ExportedActions);
			ToolStripBuilder.ToolStripBuilderStyle defaultStyle = new ToolStripBuilder.ToolStripBuilderStyle();
			ToolStripBuilder.ToolStripBuilderStyle myStyle = new ToolStripBuilder.ToolStripBuilderStyle(ToolStripItemDisplayStyle.ImageAndText, defaultStyle.ToolStripAlignment, defaultStyle.TextImageRelation);
			ToolStripBuilder.BuildToolbar(_toolbar.Items, toolbarActions.ChildNodes, myStyle);

			_tableView.Table = component.Table;
			_tableView.ContextActionModelDelegate = this.GetContextMenuModel;
			_tableView.ReadOnly = true;
		}

		private ActionModelNode GetContextMenuModel(int row, int column)
		{
			if (row >= 0)
				return null;
			return ActionModelRoot.CreateModel("ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms", StudyFilterTool.DefaultContextMenuActionSite, _component.ExportedActions);
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
						_component.Selection.Add(o as StudyItem);
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