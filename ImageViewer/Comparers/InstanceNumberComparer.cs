using System;
using System.Collections.Generic;
using ClearCanvas.Dicom;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public class InstanceNumberComparer : DicomSopComparer
	{
		public InstanceNumberComparer()
		{
		}

		public InstanceNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		private IEnumerable<IComparable> GetCompareValues(Sop sop)
		{
			yield return sop.InstanceNumber;
		}

		public override int Compare(Sop x, Sop y)
		{
			return Compare(GetCompareValues(x), GetCompareValues(y));
		}
	}
}