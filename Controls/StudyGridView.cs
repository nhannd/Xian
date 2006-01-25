using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace ClearCanvas.Controls
{
	public partial class StudyGridView : UserControl
	{
		public StudyGridView()
		{
			InitializeComponent();
		}

		public DataGridView DataGridView
		{
			get { return _dataGridView; }
		}

		public ToolStrip ToolStrip
		{
			get { return _toolStrip; }
		}

		public void AddColumn(string columnHeaderText)
		{
			DataGridViewTextBoxColumn column = new DataGridViewTextBoxColumn();
			column.HeaderText = columnHeaderText;
			column.Name = columnHeaderText;

			this.DataGridView.Columns.Add(column);
		}
	}
}
