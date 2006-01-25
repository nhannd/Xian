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
			AddColumns();
		}

		public DataGridView DataGridView
		{
			get { return _dataGridView; }
		}

		public ToolStrip ToolStrip
		{
			get { return _toolStrip; }
		}

		private void AddColumns()
		{

		}
	}
}
