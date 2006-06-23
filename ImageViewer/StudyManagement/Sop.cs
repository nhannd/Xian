using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public abstract class Sop
	{
		private Series _parentSeries;

		public Series ParentSeries
		{
			get { return _parentSeries; }
			internal set { _parentSeries = value; }
		}

		public abstract string SopInstanceUID { get; set; }

		public abstract string TransferSyntaxUID { get; set; }

		public override string ToString()
		{
			return this.SopInstanceUID;
		}
	}
}
