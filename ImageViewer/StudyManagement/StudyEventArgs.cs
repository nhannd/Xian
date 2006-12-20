using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class StudyEventArgs : CollectionEventArgs<Study>
	{
		public StudyEventArgs ()
		{

		}

		public StudyEventArgs(Study study)
		{
			base.Item  = study;
		}

		public Study Study { get { return base.Item; } }

	}
}
