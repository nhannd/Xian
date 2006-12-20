using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;
using ClearCanvas.Common.Utilities;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class SopEventArgs : CollectionEventArgs<Sop>
	{
		public SopEventArgs()
		{

		}

		public SopEventArgs(Sop sop)
		{
			base.Item  = sop;
		}

		public Sop Sop { get { return base.Item; } }
	}
}
