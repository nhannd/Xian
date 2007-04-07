using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer.Comparers
{
	public class StudyDateComparer : StandardImageSetComparer
	{
		public StudyDateComparer()
		{

		}

		#region IComparer<IDisplaySet> Members

		protected override int Compare(ImageSop x, ImageSop y)
		{
			DateTime studyDate1;
			DateParser.Parse(x.StudyDate, out studyDate1);

			DateTime studyDate2;
			DateParser.Parse(y.StudyDate, out studyDate2);

			if (studyDate1 > studyDate2)
				return 1;
			else if (studyDate1 < studyDate2)
				return -1;
			else
				return 0;
		}

		#endregion
	}
}
