using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	public class InstanceNumberComparer : DicomImageSopComparer
	{
		public InstanceNumberComparer()
		{
		}

		public InstanceNumberComparer(bool reverse) : base(reverse)
		{
		}

		protected override int Compare(ImageSop x, ImageSop y)
		{
			int imageNumber1 = x.InstanceNumber;
			int imageNumber2 = y.InstanceNumber;

			if (imageNumber1 < imageNumber2)
				return this.ReturnValue; // x < y
			else if (imageNumber1 > imageNumber2)
				return (-this.ReturnValue); // x > y
			else
				return 0; // x == y
		}
	}
}
