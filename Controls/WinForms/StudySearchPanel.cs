using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls.WinForms;

namespace ClearCanvas.Controls.WinForms
{
	public partial class StudySearchPanel : UserControl
	{
		public StudySearchPanel()
		{
			InitializeComponent();
		}

		public string HeaderText
		{
			get { return _headerStripLabel.Text; }
			set { _headerStripLabel.Text = value; }
		}

		public StudySearchForm StudySearchForm
		{
			get { return _studySearchForm; }
		}

		public StudyGridView StudyGridView
		{
			get { return _studyGridView; }
		}
	}
}
