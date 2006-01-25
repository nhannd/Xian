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
	public partial class DetailViewControl : UserControl
	{
		public DetailViewControl()
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
