using System;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on study date.
	/// </summary>
	public class StudyDateComparer : DicomStudyComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="StudyDateComparer"/>.
		/// </summary>
		public StudyDateComparer()
		{

		}

		#region IComparer<IDisplaySet> Members

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s based on study date.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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
