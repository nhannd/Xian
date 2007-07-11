#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using ClearCanvas.ImageViewer.StudyManagement;
using System.Diagnostics;
using ClearCanvas.ImageViewer.StudyManagement.Tests;
using ClearCanvas.ImageViewer.Tests;
using ClearCanvas.ImageViewer.Comparers;

namespace ClearCanvas.ImageViewer.Imaging.Tests
{
	[TestFixture]
	public class DicomPresentationImageSortTests
	{
		[TestFixtureSetUp]
		public void Init()
		{
		}

		[TestFixtureTearDown]
		public void Cleanup()
		{ 
		}

		[Test]
		public void TestSortingDicomImagesByInstanceNumber()
		{
			TestSortingDicomImagesByInstanceNumber(false);
		}

		[Test]
		public void TestSortingDicomImagesByInstanceNumberReverse()
		{
			TestSortingDicomImagesByInstanceNumber(true);
		}

		public void TestSortingDicomImagesByInstanceNumber(bool reverse)
		{ 
			PresentationImageCollection orderedCollection = new PresentationImageCollection();
			PresentationImageCollection nonOrderedCollection = new PresentationImageCollection();

			MockImageSop junkImageSop = NewMockImageSop("123", "1", 0);
			orderedCollection.Add(new StandardPresentationImage(junkImageSop));

			AppendCollection(NewDicomSeries("123", "1", 1, 25), orderedCollection);
			
			AppendCollection(NewDicomSeries("123", "10", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("123", "111", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("123", "456", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("123", "789", 1, 25), orderedCollection);

			//Note that the seriesUID are *not* in numerical order.  This is because
			//it is a string comparison that is being done.
			AppendCollection(NewDicomSeries("a", "1", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "11", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "12", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "6", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("a", "7", 1, 25), orderedCollection);

			AppendCollection(NewDicomSeries("b", "20", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "21", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "33", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "34", 1, 25), orderedCollection);
			AppendCollection(NewDicomSeries("b", "40", 1, 25), orderedCollection);

			//just put one of these at the end, it's enough.  We just want to see
			// that non-Dicom images get pushed to one end (depending on forward/reverse).
			orderedCollection.Add(new MockPresentationImage());

			if (reverse)
			{
				PresentationImageCollection reversedCollection = new PresentationImageCollection();
				for (int i = orderedCollection.Count - 1; i >= 0; --i)
					reversedCollection.Add(orderedCollection[i]);

				orderedCollection = reversedCollection;
			}

			Randomize(orderedCollection, nonOrderedCollection);

			Debug.WriteLine("NON-ORDERED COLLECTION (PRE-SORT)\n");
			Trace(nonOrderedCollection);
			
			//Be certain it is currently *not* in order.
			Assert.IsFalse(VerifyOrdered(orderedCollection, nonOrderedCollection));

			//Sort it.
			nonOrderedCollection.Sort(new InstanceNumberComparer(reverse));
			
			Debug.WriteLine("NON-ORDERED COLLECTION (POST-SORT)");
			Trace(nonOrderedCollection);

			//It should now be in the proper order.
			Assert.IsTrue(VerifyOrdered(orderedCollection, nonOrderedCollection));
		}

		void Trace(PresentationImageCollection collection)
		{ 
			foreach (PresentationImage image in collection)
			{
				if (image is StandardPresentationImage)
				{
					StandardPresentationImage dicomImage = (StandardPresentationImage)image;
					string line = string.Format("StudyUID: {0}, Series: {1}, Instance: {2}", dicomImage.ImageSop.StudyInstanceUID,
																			dicomImage.ImageSop.SeriesInstanceUID,
																			dicomImage.ImageSop.InstanceNumber);
					Debug.WriteLine(line);
				}
				else
				{
					Debug.WriteLine("** Non-Dicom Image **");
				}
			}
		}

		void Randomize(PresentationImageCollection orderedCollection, PresentationImageCollection nonOrderedCollection)
		{ 
			PresentationImageCollection tempCollection = new PresentationImageCollection();
			foreach (PresentationImage image in orderedCollection)
				tempCollection.Add(image);

			Random random = new Random();
			while (tempCollection.Count > 0)
			{
				int index = random.Next(tempCollection.Count);
				IPresentationImage randomImage = tempCollection[index];
				nonOrderedCollection.Add(randomImage);
				tempCollection.Remove(randomImage);
			}
		}

		public bool VerifyOrdered(
			PresentationImageCollection orderedCollection,
			PresentationImageCollection nonOrderedCollection)
		{
			Assert.AreEqual(orderedCollection.Count, nonOrderedCollection.Count);

			int index = 0;
			foreach (PresentationImage orderedImage in orderedCollection)
			{
				IPresentationImage nonOrderedImage = nonOrderedCollection[index];

				if (!(orderedImage is StandardPresentationImage) && !(nonOrderedImage is StandardPresentationImage))
				{
					++index;
					continue;
				}

				if (!(orderedImage is StandardPresentationImage) && (nonOrderedImage is StandardPresentationImage))
					return false;

				if ((orderedImage is StandardPresentationImage) && !(nonOrderedImage is StandardPresentationImage))
					return false;

				StandardPresentationImage dicomOrdered = orderedImage as StandardPresentationImage;
				StandardPresentationImage dicomNonOrdered = nonOrderedImage as StandardPresentationImage;

				if (dicomOrdered.ImageSop.StudyInstanceUID != dicomNonOrdered.ImageSop.StudyInstanceUID ||
					dicomOrdered.ImageSop.SeriesInstanceUID != dicomNonOrdered.ImageSop.SeriesInstanceUID ||
					dicomOrdered.ImageSop.InstanceNumber != dicomNonOrdered.ImageSop.InstanceNumber)
					return false;

				++index;
			}

			return true;
		}

		public void AppendCollection(List<PresentationImage> listImages, PresentationImageCollection collection)
		{
			foreach (PresentationImage image in listImages)
				collection.Add(image);
		}

		public List<PresentationImage> NewDicomSeries(string studyUID, string seriesUID, int startInstanceNumber, uint numberInstances)
		{
			List<PresentationImage> listImages = new List<PresentationImage>();
			int instanceNumber = (int)startInstanceNumber;
			while (instanceNumber < (startInstanceNumber + numberInstances))
			{
				listImages.Add(NewDicomImage(studyUID, seriesUID, instanceNumber));
				++instanceNumber;
			}

			return listImages;
		}

		public StandardPresentationImage NewDicomImage(string studyUID, string seriesUID, int instanceNumber)
		{
			return new StandardPresentationImage(NewMockImageSop(studyUID, seriesUID, instanceNumber));
		}

		internal MockImageSop NewMockImageSop(string studyUID, string seriesUID, int instanceNumber)
		{
			MockImageSop newImageSop = new MockImageSop();
			IMockImageSopSetters setters = (IMockImageSopSetters)newImageSop;

			setters.StudyInstanceUid = studyUID;
			setters.SeriesInstanceUid = seriesUID;
			setters.InstanceNumber = instanceNumber;

			return newImageSop;
		}
	}
}

#endif