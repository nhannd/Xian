using System;
using System.Collections.Generic;
using System.Text;
using ClearCanvas.ImageViewer.StudyManagement;
using ClearCanvas.Dicom;

namespace ClearCanvas.ImageViewer
{
	public class ImageSetSortByStudyDate : IComparer<IImageSet>
	{
		public ImageSetSortByStudyDate()
		{

		}

		#region IComparer<IDisplaySet> Members

		public int Compare(IImageSet x, IImageSet y)
		{
			if (x.DisplaySets.Count == 0 ||
				y.DisplaySets.Count == 0)
				return 0;

			IDisplaySet displaySet1 = x.DisplaySets[0];
			IDisplaySet displaySet2 = y.DisplaySets[0];

			if (displaySet1.PresentationImages.Count == 0 ||
				displaySet2.PresentationImages.Count == 0)
				return 0;

			IImageSopProvider image1 = displaySet1.PresentationImages[0] as IImageSopProvider;
			IImageSopProvider image2 = displaySet2.PresentationImages[0] as IImageSopProvider;

			if (image1 == null)
			{
				if (image2 == null)
					return 0; // x == y
				else
					return -1; // x > y (because we want it at the end for non-reverse sorting)
			}
			else
			{
				if (image2 == null)
					return 1; // x < y (because we want it at the end for non-reverse sorting)
			}

			DateTime studyDate1;
			DateParser.Parse(image1.ImageSop.StudyDate, out studyDate1);

			DateTime studyDate2;
			DateParser.Parse(image2.ImageSop.StudyDate, out studyDate2);

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
