using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public class StudyDateComparer : DicomStudyComparer
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
