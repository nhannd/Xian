using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public class AcquisitionNumberComparer : StandardPresentationImageComparer
	{
		public AcquisitionNumberComparer()
		{
		}

		public AcquisitionNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		protected override int Compare(ImageSop x, ImageSop y)
		{
			int acquisitionNumber1 = x.AcquisitionNumber;
			int acquisitionNumber2 = y.AcquisitionNumber;

			if (acquisitionNumber1 < acquisitionNumber2)
				return this.ReturnValue; // x < y
			else if (acquisitionNumber1 > acquisitionNumber2)
				return (-this.ReturnValue); // x > y
			else
				return 0; // x == y
		}
	}
}
