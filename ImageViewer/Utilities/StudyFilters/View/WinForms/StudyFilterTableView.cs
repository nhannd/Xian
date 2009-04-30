#region License

// Copyright (c) 2009, ClearCanvas Inc.
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