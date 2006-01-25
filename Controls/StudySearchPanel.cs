using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using ClearCanvas.Controls;

namespace ClearCanvas.Workstation.Dashboard.Local
{
	public partial class StudySearchPanel : UserControl
	{
		public StudySearchPanel()
		{
			InitializeComponent();
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
