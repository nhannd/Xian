using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer.Comparers
{
	/// <summary>
	/// Compares two <see cref="ImageSop"/>s based on instance number.
	/// </summary>
	public class InstanceNumberComparer : DicomImageSopComparer
	{
		/// <summary>
		/// Initializes a new instance of <see cref="InstanceNumberComparer"/>.
		/// </summary>
		public InstanceNumberComparer()
		{
		}

		/// <summary>
		/// Initializes a new instance of <see cref="InstanceNumberComparer"/>.
		/// </summary>
		public InstanceNumberComparer(bool reverse)
			: base(reverse)
		{
		}

		/// <summary>
		/// Compares two <see cref="ImageSop"/>s based on instance number.
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		/// <returns></returns>
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
