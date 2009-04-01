using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using ClearCanvas.Desktop.Actions;
using ClearCanvas.Desktop.View.WinForms;

namespace ClearCanvas.ImageViewer.Utilities.StudyFilters.View.WinForms
{
	public class StudyFilterTableView : TableView
	{
		public delegate ActionModelNode ActionModelGetterDelegate(int row, int column);

		private readonly IContainer _components;
		private readonly ContextMenuStrip _contextMenuStrip;
		private ActionModelGetterDelegate _contextActionModelDelegate;

		public StudyFilterTableView() : base()
		{
			base.DataGridView.ColumnHeaderMouseClick += DataGridView_ColumnHeaderMouseClick;
			base.DataGridView.CellMouseClick += DataGridView_CellMouseClick;

			_components = new Container();
			_contextMenuStrip = new ContextMenuStrip(_components);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing && _components != null)
			{
				_components.Dispose();
			}

			base.DataGridView.CellMouseClick -= DataGridView_CellMouseClick;
			base.DataGridView.ColumnHeaderMouseClick -= DataGridView_ColumnHeaderMouseClick;
			base.Dispose(disposing);
		}

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ActionModelGetterDelegate ContextActionModelDelegate
		{
			get { return _contextActionModelDelegate; }
			set { _contextActionModelDelegate = value; }
		}

		private void DataGridView_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ToolStripBuilder.Clear(_contextMenuStrip.Items);
				if (_contextActionModelDelegate != null)
				{
					ActionModelNode actionModel = _contextActionModelDelegate(e.RowIndex, e.ColumnIndex);
					if (actionModel != null)
						ToolStripBuilder.BuildMenu(_contextMenuStrip.Items, actionModel.ChildNodes);

					Rectangle r = base.DataGridView.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, true);
					_contextMenuStrip.Show(base.DataGridView.PointToScreen(new Point(e.Location.X + r.Left, e.Location.Y + r.Top)));
				}
			}
		}

		private void DataGridView_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.Button == MouseButtons.Right)
			{
				ToolStripBuilder.Clear(_contextMenuStrip.Items);
				if (_contextActionModelDelegate != null)
				{
					ActionModelNode actionModel = _contextActionModelDelegate(-1, e.ColumnIndex);
					if (actionModel != null)
						ToolStripBuilder.BuildMenu(_contextMenuStrip.Items, actionModel.ChildNodes);

					Rectangle r = base.DataGridView.GetColumnDisplayRectangle(e.ColumnIndex, true);
					_contextMenuStrip.Show(base.DataGridView.PointToScreen(new Point(e.Location.X + r.Left, e.Location.Y + r.Top)));
				}
			}
		}
	}
}