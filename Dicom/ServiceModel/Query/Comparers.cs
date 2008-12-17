using System;
using System.Collections.Generic;
using ClearCanvas.Dicom.Utilities;

namespace ClearCanvas.Dicom.ServiceModel.Query
{
	#region Study Comparers

	/// <summary>
	/// Sorts <see cref="StudyIdentifier"/>s and <see cref="StudyRootStudyIdentifier"/>s by Study Date/time,
	/// in reverse (most recent first).
	/// </summary>
	public class StudyDateTimeComparer : IComparer<StudyIdentifier>, IComparer<StudyRootStudyIdentifier>
	{
		public StudyDateTimeComparer()
		{
		}

		#region IComparer<StudyIdentifier> Members

		/// <summary>
		/// Compares two <see cref="StudyIdentifier"/>s.
		/// </summary>
		public int Compare(StudyIdentifier x, StudyIdentifier y)
		{
			DateTime? studyDateX = DateParser.Parse(x.StudyDate);
			DateTime? studyTimeX = TimeParser.Parse(x.StudyTime);

			DateTime? studyDateY = DateParser.Parse(y.StudyDate);
			DateTime? studyTimeY = TimeParser.Parse(y.StudyTime);

			DateTime? studyDateTimeX = studyDateX;
			if (studyDateTimeX != null && studyTimeX != null)
				studyDateTimeX = studyDateTimeX.Value.Add(studyTimeX.Value.TimeOfDay);

			DateTime? studyDateTimeY = studyDateY;
			if (studyDateTimeY != null && studyTimeY != null)
				studyDateTimeY = studyDateTimeY.Value.Add(studyTimeY.Value.TimeOfDay);

			if (studyDateTimeX == null)
			{
				if (studyDateTimeY == null)
					return Math.Sign(x.StudyInstanceUid.CompareTo(y.StudyInstanceUid));
				else
					return 1; // > because we want x at the end.
			}
			else if (studyDateY == null)
				return -1; // < because we want x at the beginning.

			//Return negative of x compared to y because we want most recent first.
			return -Math.Sign(studyDateTimeX.Value.CompareTo(studyDateTimeY));
		}

		#endregion

		#region IComparer<StudyRootStudyIdentifier> Members

		/// <summary>
		/// Compares 2 <see cref="StudyRootStudyIdentifier"/>s.
		/// </summary>
		public int Compare(StudyRootStudyIdentifier x, StudyRootStudyIdentifier y)
		{
			return Compare((StudyIdentifier) x, (StudyIdentifier) y);
		}

		#endregion
	}

	#endregion
}