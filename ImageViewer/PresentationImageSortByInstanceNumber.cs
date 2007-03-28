using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	public class PresentationImageSortByInstanceNumber : IComparer<IPresentationImage>
	{
		private int _returnValue;

		public PresentationImageSortByInstanceNumber()
		{
			Reverse = false;
		}

		public PresentationImageSortByInstanceNumber(bool reverse)
		{
			this.Reverse = reverse;
		}

		public bool Reverse
		{
			get
			{ 
				return (_returnValue == 1); 
			}
			set
			{
				if (!value)
					_returnValue = -1;
				else
					_returnValue = 1;
			}
		}

		#region IComparer<IPresentationImage> Members

		public int Compare(IPresentationImage x, IPresentationImage y)
		{
			IImageSopProvider image1 = x as IImageSopProvider;
			IImageSopProvider image2 = y as IImageSopProvider;

			//just push non-DICOM images down to one end of the list.
			if (image1 == null)
			{
				if (image2 == null)
					return 0; // x == y
				else
					return (- _returnValue); // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (image2 == null)
					return _returnValue; // x < y (because we want it at the end for non-reverse sorting)
			}

			int studyUIDCompare = String.Compare(image1.ImageSop.StudyInstanceUID, image2.ImageSop.StudyInstanceUID);
			if (studyUIDCompare < 0)
			{
				return _returnValue; // x < y
			}
			else if (studyUIDCompare == 0)
			{
				int seriesUIDCompare = String.Compare(image1.ImageSop.SeriesInstanceUID, image2.ImageSop.SeriesInstanceUID);
				
				if (seriesUIDCompare < 0)
				{
					return _returnValue; // x < y
				}
				else if (seriesUIDCompare == 0)
				{
					int imageNumber1 = image1.ImageSop.InstanceNumber;
					int imageNumber2 = image2.ImageSop.InstanceNumber;

					if (imageNumber1 < imageNumber2)
						return _returnValue; // x < y
					else if (imageNumber1 > imageNumber2)
						return (-_returnValue); // x > y
					else
					{
						int acquisitionNumber1 = image1.ImageSop.AcquisitionNumber;
						int acquisitionNumber2 = image2.ImageSop.AcquisitionNumber;

						if (acquisitionNumber1 < acquisitionNumber2)
							return _returnValue;
						else if (acquisitionNumber1 > acquisitionNumber2)
							return -_returnValue;
						else
							return 0; // x == y
					}
				}
			}

			return (-_returnValue); // x > y
		}

		#endregion
	}
}
