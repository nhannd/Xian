using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.Common;

namespace ClearCanvas.ImageViewer.StudyManagement
{
	public class Series
	{
		private Study _parentStudy;
		private string _seriesInstanceUID;
		private SopCollection _sops = new SopCollection();

		internal Series(string seriesInstanceUID, Study parentStudy)
		{
			_seriesInstanceUID = seriesInstanceUID;
			_parentStudy = parentStudy;
		}

		public Study ParentStudy
		{
			get { return _parentStudy; }
		}

		public string SeriesInstanceUID
		{
			get { return _seriesInstanceUID; }
			set { _seriesInstanceUID = value; }
		}

		public SopCollection Sops
		{
			get { return _sops; }
		}

		public override string ToString()
		{
			return this.SeriesInstanceUID;
		}
	}
}
