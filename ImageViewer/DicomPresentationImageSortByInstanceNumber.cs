using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;

namespace ClearCanvas.ImageViewer
{
	//!! This code is subject to change.  In the future, we will likely enforce that each display
	//!! set (PresentationImageCollection, actually) may only contain PresentationImages 
	//!! that are the same type.  This will simplify the sorting significantly as we will no 
	//!! longer have to worry about the possibility of images of different types in the same collection.
	//!! If it becomes necessary later, we can introduce composite collections.
	//!! Also, when we add support for general image sorting, these will become extensions.

	public class DicomPresentationImageSortByInstanceNumber : IComparer<IPresentationImage>
	{
		private int _returnValue;

		public DicomPresentationImageSortByInstanceNumber()
		{
			Reverse = false;
		}

		public DicomPresentationImageSortByInstanceNumber(bool reverse)
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
			IImageSopProvider dicomX = x as IImageSopProvider;
			IImageSopProvider dicomY = y as IImageSopProvider;

			//just push non-DICOM images down to one end of the list.
			if (dicomX == null)
			{
				if (dicomY == null)
				{
					return 0; // x == y
				}
				else
				{
					return (- _returnValue); // x > y (because we want it at the end for non-reverse sorting)
				}
			}
			else
			{
				if (dicomY == null)
				{
					return _returnValue; // x < y (because we want it at the end for non-reverse sorting)
				}
			}

			int studyUIDCompare = String.Compare(dicomX.ImageSop.StudyInstanceUID, dicomY.ImageSop.StudyInstanceUID);
			if (studyUIDCompare < 0)
			{
				return _returnValue; // x < y
			}
			else if (studyUIDCompare == 0)
			{
				int seriesUIDCompare = String.Compare(dicomX.ImageSop.SeriesInstanceUID, dicomY.ImageSop.SeriesInstanceUID);
				if (seriesUIDCompare < 0)
				{
					return _returnValue; // x < y
				}
				else if (seriesUIDCompare == 0)
				{

					int imageNumberX = dicomX.ImageSop.InstanceNumber;
					int imageNumberY = dicomY.ImageSop.InstanceNumber;

					if (imageNumberX < imageNumberY)
						return _returnValue; // x < y
					else if (imageNumberX > imageNumberY)
						return (-_returnValue); // x > y
					else
						return 0; // x == y
				}
			}

			return (-_returnValue); // x > y
		}

		#endregion
	}
}
